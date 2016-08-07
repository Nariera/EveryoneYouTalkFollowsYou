using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class InteractGoal : Goal
{
	public string TargetName { get; set; }

	void Start ()
	{
		GoalEvents.Instance.AddListener<InteractEvent> (IsInteracted);
		//GoalText = "Interact with " + a_oTarget.name;
		//We assign ourselves to it
	}

	private void IsInteracted (InteractEvent e)
	{
		if (TargetName.Contains (e.Source) && IsListening)
		{
			Satisfy ();
		}
	}

}

