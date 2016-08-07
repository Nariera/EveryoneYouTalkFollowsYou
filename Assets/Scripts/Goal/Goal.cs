using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
	public event System.Action<Goal> OnSatisfied, OnFailed, OnReveal;

	protected void Satisfy ()
	{
		if (OnSatisfied != null)
		{
			OnSatisfied (this);
		}
	}

	protected void Fail ()
	{
		if (OnFailed != null)
		{
			OnFailed (this);
		}
	}

	protected void Reveal ()
	{
		if (OnReveal != null)
		{
			OnReveal (this);
			//Clear it
			OnReveal = null;
		}
	}

	protected void CheckForSatisfy (InteractableObject candidate)
	{
		if (candidate.gameObject == specificTarget)
		{
			Satisfy ();
		}
	}

	public void UpdateText ()
	{
		if (associatedUIObject)
			associatedUIObject.GetComponentInChildren<UnityEngine.UI.Text> ().text = goalText;
	}

	#region Junk

	public bool completed = false, cancelled = false;
	public string goalText = "Hey this is Placeholder text";
	public int pointValue = 1;

	public GameObject associatedUIObject;

	public GameObject specificTarget;

	DestructableObject targFail;
	InteractableObject targSucc;
	TrackableObject targSeen;

	void Start ()
	{
		if (specificTarget)
		{
			targFail = specificTarget.GetComponent<DestructableObject> ();
			targSucc = specificTarget.GetComponent<InteractableObject> ();
			targSeen = specificTarget.GetComponent<TrackableObject> ();

			if (targFail)
				targFail.destroyed += Fail;
			if (targSucc)
				InteractableObject.activated += CheckForSatisfy;
			if (targSeen)
				targSeen.seen += Reveal;
		}
	}

	#endregion
}
