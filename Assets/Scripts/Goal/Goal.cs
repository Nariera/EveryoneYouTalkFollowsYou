using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
	public bool completed = false;
	public string goalText = "Hey this is Placeholder text";
	public int pointValue = 1;

	public GameObject associatedUIObject;

	public void UpdateText ()
	{
		if (associatedUIObject)
			associatedUIObject.GetComponentInChildren<UnityEngine.UI.Text> ().text = goalText;
	}
}
