using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
	public bool completed = false;
	public string goalText = "Hey this is Placeholder text";
	public int pointValue = 1;

	void Start ()
	{
		GetComponentInChildren<UnityEngine.UI.Text> ().text = goalText;
	}
}
