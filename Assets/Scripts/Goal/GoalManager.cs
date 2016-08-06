using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalManager : MonoBehaviour
{

	//Static ref. -P
	public static GoalManager gm;

	public List<Goal> activeGoals = new List<Goal> ();
	public List<Goal> cancelledGoals = new List<Goal> ();
	public List<Goal> completedGoals = new List<Goal> ();

	public Goal mainGoal;

	public GameObject subGoalCollection;

	List<GameObject> goalUIPool = new List<GameObject> ();

	/**delegates called when goals do shit, if we need those -P */
	public System.Action<Goal> onGoalAdded, onGoalCancel, onGoalComplete;

	public void AddNewGoal (Goal goal)
	{
		if (goal.completed)
		{
			Debug.LogError ("You tried to ADD goal:" + goal.goalText + " when it was already completed");
			return;
		}
		activeGoals.Add (goal);

		//Make sure the goal has a (fresh) associated UI element
		if (goal.associatedUIObject == null || goal.associatedUIObject.activeSelf)
			goal.associatedUIObject = GetGoalObject ();

		goal.UpdateText ();

		//If any methods to do when a goal is added, do them here -P
		if (onGoalAdded != null)
			onGoalAdded.Invoke (goal);
	}

	public void CancelGoal (Goal goal)
	{
		if (goal.completed)
		{
			Debug.LogError ("You tried to CANCEL goal:" + goal.goalText + " when it was already completed");
			return;
		}
		activeGoals.Remove (goal);
		cancelledGoals.Add (goal);
		goal.completed = true;

		//Anim time -P
		goal.associatedUIObject.GetComponent<Animator> ().SetTrigger ("Cancelled");

		//Queue return to pool -P
		StartCoroutine (ReturnToGoalObjectPool (goal.associatedUIObject, 5));

		//If any methods to call when goal is cancelled, do 'em, do 'em hard -P
		if (onGoalCancel != null)
			onGoalCancel.Invoke (goal);
	}

	public void CompleteGoal (Goal goal)
	{
		if (goal.completed)
		{
			Debug.LogError ("You tried to COMPLETE goal:" + goal.goalText + " when it was already completed");
			return;
		}
		activeGoals.Remove (goal);
		completedGoals.Add (goal);
		goal.completed = true;

		//Do the anim! -P
		goal.associatedUIObject.GetComponent<Animator> ().SetTrigger ("Completed");

		//Queue return to pool -P
		StartCoroutine (ReturnToGoalObjectPool (goal.associatedUIObject, 5));

		//You know, delegate ish -P
		if (onGoalComplete != null)
			onGoalComplete.Invoke (goal);
	}

	/**Pool management -P */
	GameObject GetGoalObject ()
	{
		if (goalUIPool.Exists (obj => !obj.activeSelf))
			return goalUIPool.Find (obj => !obj.activeSelf);
		else
		{
			var newObj = (GameObject)Instantiate (Resources.Load ("Goal UI Unit"));
			newObj.transform.SetParent (subGoalCollection.transform);

			return newObj;
		}
	}

	/**Moar pool management -P */
	IEnumerator ReturnToGoalObjectPool (GameObject obj, float time)
	{
		yield return new WaitForSeconds (time);

		obj.SetActive (false);
	}


	void Start ()
	{
		AddNewGoal (mainGoal);
	}

	public int TallyCompletedPoints ()
	{
		int totalPoints = 0;
		foreach (Goal goal in completedGoals)
		{
			totalPoints += goal.pointValue;
		}

		return totalPoints;
	}

	void Awake ()
	{
		//Static ref pseudo-init -P
		if (gm == null)
			gm = this;
		else if (gm != this)
			Destroy (this);
	}
}
