using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GoalManager : MonoBehaviour
{
	#region GenerateGoal

	private void GeneratePremadeGoal ()
	{
		//Load via text file
		TextAsset txtPremadeGoal = Resources.Load<TextAsset> ("InteractGoals");
		string txtGoals = txtPremadeGoal.text;
		string[] asGoals = txtGoals.Split (new string[] { Environment.NewLine }, StringSplitOptions.None);
		foreach (string sGoal in asGoals)
		{
			string[] sData = sGoal.Split (',');
			if (sData.Length == 2)
			{
				var oPremadeGoal = CreateInteractGoal (sData [1]);
				oPremadeGoal.TargetName = sData [1];
				oPremadeGoal.goalText = sData [0] + " " + sData [1];
				AddNewGoal (oPremadeGoal);
			}
		}
	}

	private void GenerateBorrowedGoal ()
	{
		var BorrowedPrefab = Resources.LoadAll<GameObject> ("BorrowedFollowers");
		foreach (var oBorrowedPrefab in BorrowedPrefab)
		{
			var oBorrowedGoal = CreateInteractGoal (oBorrowedPrefab.name);
			oBorrowedGoal.goalText = "Interact with " + oBorrowedPrefab.name;
			AddNewGoal (oBorrowedGoal);
		}
        
	}

	private void GenerateHiddenGoal ()
	{

		//So we want to generate somethingg based off of a function
		//NeverMove
        
	}

	InteractGoal CreateInteractGoal (string s = "")
	{
		GameObject go = new GameObject ();
		go.name = "Goal (" + s + ")";
		go.transform.SetParent (transform);
		InteractGoal iGoal = go.AddComponent<InteractGoal> ();
		iGoal.TargetName = "";
		iGoal.OnSatisfied += GoalSatisfied;
		iGoal.OnFailed += GoalFailed;
		iGoal.OnReveal += AddNewGoal;

		return iGoal;
	}

	#endregion

	private void GoalSatisfied (Goal a_oGoal)
	{ 
		a_oGoal.OnSatisfied -= GoalSatisfied; //shouldn't error since there's no other way it can get here
		CompleteGoal (a_oGoal);
	}

	void GoalFailed (Goal goal)
	{
		goal.OnFailed -= GoalFailed;
		CancelGoal (goal);
	}
    

	//Static ref. -P
	public static GoalManager gm;

	public List<Goal> allGoals = new List<Goal> ();
	public List<Goal> activeGoals = new List<Goal> ();
	public List<Goal> cancelledGoals = new List<Goal> ();
	public List<Goal> completedGoals = new List<Goal> ();

	public Goal mainGoal;

	public GameObject subGoalCollection;

	List<GameObject> goalUIPool = new List<GameObject> ();

	/**delegates called when goals do shit, if we need those -P */
	public System.Action<Goal> onGoalAdded, onGoalCancel, onGoalComplete;

	/**Call this when you're ready to put the goal on the UI, not when you want a goal to track*/
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
		goal.cancelled = true;

		StartCoroutine (WaitToCancel (goal));

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

		StartCoroutine (WaitToComplete (goal));

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

	IEnumerator WaitToCancel (Goal goal)
	{
		yield return new WaitUntil (() => goal.associatedUIObject != null && goal.associatedUIObject.activeSelf);

		//Anim time -P
		goal.associatedUIObject.GetComponent<Animator> ().SetTrigger ("Cancelled");

		//Queue return to pool -P
		StartCoroutine (ReturnToGoalObjectPool (goal.associatedUIObject, 5));
	}

	IEnumerator WaitToComplete (Goal goal)
	{
		yield return new WaitUntil (() => goal.associatedUIObject != null && goal.associatedUIObject.activeSelf);

		//Anim time -P
		goal.associatedUIObject.GetComponent<Animator> ().SetTrigger ("Completed");

		//Queue return to pool -P
		StartCoroutine (ReturnToGoalObjectPool (goal.associatedUIObject, 5));
	}

	/**Moar pool management -P */
	IEnumerator ReturnToGoalObjectPool (GameObject obj, float time)
	{

		yield return new WaitForSeconds (time);

		obj.SetActive (false);
	}


	void Start ()
	{
		//Prep for premade goals
		foreach (var t in FindObjectsOfType<Goal>())
		{
			allGoals.Add (t);
			t.OnSatisfied += GoalSatisfied;
			t.OnFailed += GoalFailed;
			t.OnReveal += AddNewGoal;
		}

		AddNewGoal (mainGoal);
		//GenerateBorrowedGoal ();
		//GeneratePremadeGoal ();
		//GenerateHiddenGoal ();
	}

	public int TallyCompletedPoints ()
	{
		int totalPoints = 0;
		foreach (Goal goal in completedGoals)
		{
			totalPoints += goal.pointValue;
		}
		foreach (Goal goal in cancelledGoals)
		{
			totalPoints -= goal.pointValue;
		}

		return totalPoints;
	}

	void Awake ()
	{
		//Static ref init -P
		if (gm == null)
		{
			gm = this;
		} else if (gm != this)
		{
			Destroy (this);
		}

		//InteractableObject abc = Test.GetComponent<InteractableObject>();
		//InteractGoal oNewGoal = new InteractGoal(abc);
		//AddNewGoal(oNewGoal);
		//oNewGoal.OnSatisfied += GoalSatified;

	}

}
