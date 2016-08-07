using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class InteractGoal : Goal
{
    public string TargetName { get; set; }

    public InteractGoal(string a_sName)
    {
        TargetName = a_sName;
        GoalEvents.Instance.AddListener<InteractEvent>(IsInteracted);
        //GoalText = "Interact with " + a_oTarget.name;
        //We assign ourselves to it
    }

    private void IsInteracted(InteractEvent e)
    {
        if (TargetName.Contains(e.Source))
        {
            Satisfy();
        }
    }

}

