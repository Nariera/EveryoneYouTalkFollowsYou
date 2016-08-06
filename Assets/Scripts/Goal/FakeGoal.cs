using UnityEngine;
using System.Collections;

/**Pretend goal to manage the center UI w/o causing script reference confusions -P */
public class FakeGoal : MonoBehaviour
{
	public UnityEngine.UI.Text textUI;

	/**Change the text! */
	public void UpdateText (Goal goal)
	{
		textUI.text = goal.goalText;
	}

	void Start ()
	{
		//We're fake!
		GetComponent<Animator> ().SetBool ("Fake", true);
	}

	//Call update text w/ delegate!
	void OnEnable ()
	{
		if (GoalManager.gm)
			GoalManager.gm.onGoalAdded += UpdateText;
	}

	void OnDisable ()
	{
		if (GoalManager.gm)
			GoalManager.gm.onGoalAdded -= UpdateText;
	}
}
