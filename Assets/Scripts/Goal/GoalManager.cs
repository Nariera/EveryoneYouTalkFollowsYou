using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalManager : MonoBehaviour {

    public List<Goal> ActiveGoals = new List<Goal>();
    public List<Goal> CompletedGoals = new List<Goal>();

    public void AddNewGoal(Goal goal)
    {
        if (goal.Completed)
        {
            Debug.LogError("You tried to ADD goal:" + goal.GoalText + " when it was already completed");
            return;
        }
        ActiveGoals.Add(goal);
    }

    public void CompleteGoal(Goal goal)
    {
        if (goal.Completed)
        {
            Debug.LogError("You tried to COMPLETED goal:" + goal.GoalText + " when it was already completed");
            return;
        }
        ActiveGoals.Remove(goal);
        CompletedGoals.Add(goal);
        goal.Completed = true;
    }

    public int TallyCompletedPoints()
    {
        int totalPoints = 0;
        foreach(Goal goal in CompletedGoals)
        {
            totalPoints++;
        }
        return totalPoints;
    }
}
