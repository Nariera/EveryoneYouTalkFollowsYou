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

		//You know, delegate ish -P
		if (onGoalComplete != null)
			onGoalComplete.Invoke (goal);
	}

	void Start ()
	{
		AddNewGoal (mainGoal);
	}

	public int TallyCompletedPoints ()
	{
		int totalPoints = 0;
//        foreach(Goal goal in CompletedGoals)
//        {
//            totalPoints++;
//        }
		//Easier: (unless you want to do something else w/ each goal object when tallying)
		totalPoints = completedGoals.Count;

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
