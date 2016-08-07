using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**Pretend goal to manage the center UI w/o causing script reference confusions -P */
public class FakeGoal : MonoBehaviour
{
	public UnityEngine.UI.Text textUI;
	public RectTransform starRect;
	Queue<Goal> waitingToAnim = new Queue<Goal> ();

	bool ready = true;

	/**Change the text! And cascade animations -P */
	public void NewGoal (Goal goal)
	{
		waitingToAnim.Enqueue (goal);
	}

	void Update ()
	{
		if (waitingToAnim.Count > 0 && ready)
		{
			BeginGoalAnim (waitingToAnim.Dequeue ());
			ready = false;
		}
	}

	void BeginGoalAnim (Goal goal)
	{
		textUI.text = goal.goalText;

		GetComponent<Animator> ().SetTrigger ("Begin");

		StartCoroutine (MoveStar (2.5f, goal));
	}

	IEnumerator MoveStar (float time, Goal goal)
	{
		yield return new WaitForSeconds (time);

		ready = true;
		StartCoroutine (StarMovement (1, goal));
	}

	/**Hand-coded anim, bitchez (damn this Unity version) -P */
	IEnumerator StarMovement (float time, Goal goal)
	{
		float elapsedTime = 0;
		
		while (elapsedTime < time)
		{
			//Add last frame -P
			elapsedTime += Time.unscaledDeltaTime;

			//Current lerp
			float lerp = elapsedTime / time;

			//Anim effects
			starRect.anchorMax = Vector2.Lerp (new Vector2 (0.5f, 0.5f), new Vector2 (0, 1), lerp);
			starRect.anchorMin = Vector2.Lerp (new Vector2 (0.5f, 0.5f), new Vector2 (0, 1), lerp);
			starRect.anchoredPosition = Vector2.zero;

			if (lerp < 0.25f)
				starRect.localScale = Vector3.one * (lerp * 4) / 10;
			else if (lerp > 0.75f)
				starRect.localScale = Vector3.one * (1 - lerp) / 10;
			else
				starRect.localScale = Vector3.one / 10;

			//Wait a frame -P
			yield return null;
		}

		ToggleNewGoal (goal);
	}

	void ToggleNewGoal (Goal goal)
	{
		goal.associatedUIObject.SetActive (true);
		goal.associatedUIObject.GetComponent<Animator> ().SetTrigger ("Begin");
	}

	void Start ()
	{
		//We're fake! -P
		GetComponent<Animator> ().SetBool ("Fake", true);
	}

	//Call update text w/ delegate! -P
	void OnEnable ()
	{
		if (GoalManager.gm)
			GoalManager.gm.onGoalAdded += NewGoal;
	}

	void OnDisable ()
	{
		if (GoalManager.gm)
			GoalManager.gm.onGoalAdded -= NewGoal;
	}
}
