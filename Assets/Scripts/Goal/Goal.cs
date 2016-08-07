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

	protected void Satisfy ()
	{
		if (OnSatisfied != null)
		{
			OnSatisfied (this);
		}
	}

	protected void Fail ()
	{
		if (OnFailed != null)
		{
			OnFailed (this);
		}
	}

	protected void Reveal ()
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

	protected void CheckForSatisfy (InteractableObject candidate)
	{
		if (candidate.gameObject == specificTarget)
		{
			Satisfy ();
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

	public GameObject specificTarget;

	public GoalAction action;

	[Tooltip ("This goal is off limits until these fuckers get completed.")] //TODO
	public List<Goal> prerequisiteGoals = new List<Goal> ();
	[Tooltip ("Those fuckers get cancelled when this appears.")]
	public List<Goal> goalsThisClosesOnReveal = new List<Goal> ();

	DestructableObject targDestr;
	InteractableObject targInter;
	TrackableObject targTrack;

	void Start ()
	{
		if (specificTarget)
		{
			targDestr = specificTarget.GetComponent<DestructableObject> ();
			targInter = specificTarget.GetComponent<InteractableObject> ();
			targTrack = specificTarget.GetComponent<TrackableObject> ();

			switch (action)
			{
			case GoalAction.Interact:
				if (targDestr)
					targDestr.destroyed += Fail;
				if (targInter)
					InteractableObject.activated += CheckForSatisfy;
				if (targTrack)
					targTrack.seen += Reveal;
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
				if (targTrack)
				{
					targTrack.seen += Reveal;
					targTrack.seen += Fail;
				}
				break;
			}

		}
	}

	#endregion
}
