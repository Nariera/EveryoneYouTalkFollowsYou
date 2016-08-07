using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goal : MonoBehaviour
{
	public enum GoalAction
	{
		Interact,
		//Save has no win condition (yet ? maybe steal from goal metric w/ population left)
		Save,
		Kill,
		LookAt,
		Avoid
	}

	public event System.Action<Goal> OnSatisfied, OnFailed, OnReveal;
	event System.Action OnTargetClicked;

	bool goalEnded = false;

	protected void Satisfy ()
	{
		//We check for prereq goals being done before we ever consider satisfying or even revealing a locked off one
		if (!goalEnded && !prerequisiteGoals.Exists (obj => obj != null && !obj.completed))
		{
			if (OnSatisfied != null)
			{
				OnSatisfied (this);
			}
			
			foreach (var t in goalsThisClosesOnSatisfy)
			{
				GoalManager.gm.CancelGoal (t);
			}

			goalEnded = true;
		}
	}

	protected void Fail ()
	{
		//Can still fail early
		if (!goalEnded)
		{
			if (OnFailed != null)
			{
				OnFailed (this);
			}

			goalEnded = true;
		}
	}

	protected void Reveal ()
	{
		//See Satisfy for notes
		if (!goalEnded && !prerequisiteGoals.Exists (obj => obj != null && !obj.completed))
		{
			if (OnReveal != null)
			{
				OnReveal (this);
				//Clear it
				OnReveal = null;
			}

			foreach (var t in goalsThisClosesOnReveal)
			{
				GoalManager.gm.CancelGoal (t);
			}
		}
	}

	protected void CheckClickForTarget (InteractableObject candidate)
	{
		if (specificTargets.Contains (candidate.gameObject))
		{
			if (OnTargetClicked != null)
				OnTargetClicked.Invoke ();
		}
	}

	public void UpdateText ()
	{
		if (associatedUIObject)
			associatedUIObject.GetComponentInChildren<UnityEngine.UI.Text> ().text = goalText;
	}

	#region Junk

	public bool completed = false, cancelled = false;
	public string goalText = "Hey this is Placeholder text";
	public int pointValue = 1;

	public GameObject associatedUIObject;

	[Tooltip ("Each target can fulfill the events of the goal.")]
	public List<GameObject> specificTargets = new List<GameObject> ();

	public GoalAction action;

	[Tooltip ("If 0, it will stay more-or-less permanently. Otherwise, this is a time limit to clear space on the screen.")]
	public int secondsAvailable = 0;

	[Tooltip ("This goal is off limits until these fuckers get completed.")]
	public List<Goal> prerequisiteGoals = new List<Goal> ();
	[Tooltip ("Those fuckers get cancelled when this appears.")]
	public List<Goal> goalsThisClosesOnReveal = new List<Goal> ();
	[Tooltip ("Options locked.")]
	public List<Goal> goalsThisClosesOnSatisfy = new List<Goal> ();

	DestructableObject targDestr;
	InteractableObject targInter;
	TrackableObject targTrack;

	void Start ()
	{
		//Set delegates
		foreach (var t in specificTargets)
		{
			//Ignore null mistakes
			if (t == null)
				continue;

			targDestr = t.GetComponent<DestructableObject> ();
			targInter = t.GetComponent<InteractableObject> ();
			targTrack = t.GetComponent<TrackableObject> ();

			InteractableObject.activated += CheckClickForTarget;

			switch (action)
			{
			case GoalAction.Interact:
				if (targDestr)
					targDestr.destroyed += Fail;
				if (targTrack)
					targTrack.seen += Reveal;
				OnTargetClicked += Satisfy;
				break;
			case GoalAction.Kill:
				if (targDestr)
					targDestr.destroyed += Satisfy;
				if (targTrack)
					targTrack.seen += Reveal;
				break;
			case GoalAction.Save:
				if (targDestr)
					targDestr.destroyed += Fail;
				if (targTrack)
					targTrack.seen += Reveal;
				break;
			case GoalAction.LookAt:
				if (targTrack)
				{
					targTrack.seen += Reveal;
					targTrack.seen += Satisfy;
				}
				break;
			case GoalAction.Avoid:
				OnTargetClicked += Reveal;
				OnTargetClicked += Fail;
				break;
			}

			if (targTrack && secondsAvailable > 0)
			{
				targTrack.seen += StartTimer;
			}

		}
	}

	void StartTimer ()
	{
		//Only start timer when the goal's prereqs are ready
		if (!prerequisiteGoals.Exists (obj => obj != null && !obj.completed))
			StartCoroutine (Timer (secondsAvailable));
	}

	IEnumerator Timer (float seconds)
	{
		yield return new WaitForSeconds (seconds);

		if (action == GoalAction.Avoid)
		{
			Reveal ();
			Satisfy ();
		} else
			Fail ();
	}

	#endregion
}
