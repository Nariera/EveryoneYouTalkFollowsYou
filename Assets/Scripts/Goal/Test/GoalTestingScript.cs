using UnityEngine;
using System.Collections;

/// <summary>
/// This is just for testing
/// -Alec
/// </summary>
public class GoalTestingScript : MonoBehaviour {

    GoalManager GM;
    public Goal[] goalToAdd;

    // Use this for initialization
    void Start () {
        GM = GameObject.FindGameObjectWithTag("GoalManager").GetComponent<GoalManager>();

        goalToAdd = GetComponentsInChildren<Goal>();
        foreach(Goal goal in goalToAdd)
        {
            GM.AddNewGoal(goal);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
