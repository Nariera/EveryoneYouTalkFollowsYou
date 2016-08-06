using UnityEngine;
using System.Collections;

/// <summary>
/// This is also just for testing.
/// -Alec
/// </summary>
public class GoalCompleter : MonoBehaviour {

    public bool completeGoal = false;
    bool done = false;
    public Goal goalToComplete;
    GoalManager GM;

	// Use this for initialization
	void Start () {
        GM = GameObject.FindGameObjectWithTag("GoalManager").GetComponent<GoalManager>();
	}

    void Update()
    {
        if (completeGoal && !done)
        {
            done = true;
            GM.CompleteGoal(goalToComplete);
        }
    }
}
