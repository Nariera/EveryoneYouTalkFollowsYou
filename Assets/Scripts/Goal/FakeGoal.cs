using UnityEngine;
using System.Collections;

/**Pretend goal to manage the center UI w/o causing script reference confusions -P */
public class FakeGoal : MonoBehaviour
{
	public UnityEngine.UI.Text textUI;
	public RectTransform starRect;

	Goal currentGoalToMimic;

	/**Change the text! And cascade animations -P */
	public void NewGoal (Goal goal)
	{
		textUI.text = goal.goalText;

		currentGoalToMimic = goal;

		GetComponent<Animator> ().SetTrigger ("Begin");

		Invoke ("MoveStar", 2.5f);
	}

	void MoveStar ()
	{
		StartCoroutine (StarMovement (1));
	}

	/**Hand-coded anim, bitchez (damn this Unity version) -P */
	IEnumerator StarMovement (float time)
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

		ToggleNewGoal ();
	}

	void ToggleNewGoal ()
	{
        currentGoalToMimic.associatedUIObject.SetActive(true);
        currentGoalToMimic.associatedUIObject.GetComponent<Animator>().SetTrigger("Begin");
		//currentGoalToMimic.gameObject.SetActive (true);
		//currentGoalToMimic.GetComponent<Animator> ().SetTrigger ("Begin");
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
