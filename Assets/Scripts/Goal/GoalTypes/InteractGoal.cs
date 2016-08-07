using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class InteractGoal : Goal
{
    public InteractableObject Target { get; set; }

    public InteractGoal(InteractableObject a_oTarget)
    {
        Target = a_oTarget;
        GoalEvents.Instance.AddListener<InteractEvent>(IsInteracted);
        GoalText = "Interact with " + a_oTarget.name;
        //We assign ourselves to it
    }

    private void IsInteracted(InteractEvent e)
    {
        if (Target.Equals(e.Source))
        {
            Satisfy();
        }
    }

}

