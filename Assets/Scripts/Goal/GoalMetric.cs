using System.Collections.Generic;
using UnityEngine;

public class TalkEvent : GoalEvent
{

}

public class DestroyEvent : GoalEvent
{
	public string Name;
}

public class GoalMetric : MonoBehaviour
{
	[SerializeField]
	private GameObject WorldContainer;

	#region Timer

	private Dictionary<float, Goal> TimerLibrary = new Dictionary<float, Goal> ();
	private const float TIMER_DELAY = 5.0f;

	private void CheckTimer ()
	{
		Stack<float> RemoveBuffer = new Stack<float> ();
		foreach (KeyValuePair<float, Goal> kvTimer in TimerLibrary)
		{
			if (kvTimer.Key < Time.time)
			{
				GoalManager.gm.CompleteGoal (kvTimer.Value);
				RemoveBuffer.Push (kvTimer.Key);
			}
		}
		while (RemoveBuffer.Count > 0)
		{
			TimerLibrary.Remove (RemoveBuffer.Pop ());
		}
	}

	private void AddTimer (float a_fTimer, Goal a_oGoal)
	{
		if (TimerLibrary.ContainsKey (a_fTimer))
		{
			AddTimer (a_fTimer + 1, a_oGoal);
		} else
		{
			TimerLibrary.Add (a_fTimer, a_oGoal);
		}
	}

	#endregion

	private void OnEnable ()
	{
		GoalEvents.Instance.AddListener<TalkEvent> (NoTalk);
		GoalEvents.Instance.AddListener<DestroyEvent> (DestroyedObject);
		//initialize all events here
	}

	void Start ()
	{
		startCount = WorldContainer.transform.childCount;
	}


	private void Update ()
	{
		CheckTimer ();
		//NoTalk
		if (!TalkDisqualified)
		{
			if (TotalTalkUpdate > NO_TALKING_MIN)
			{
				Goal oNoTalk = new Goal () {
					goalText = "More talking less thinking!",
					pointValue = 100
				};
				GoalManager.gm.AddNewGoal (oNoTalk);
				AddTimer (Time.time + TIMER_DELAY, oNoTalk);
				TalkDisqualified = true;
			} else
			{
				TotalTalkUpdate += Time.deltaTime;
			}
		}

		//Destroy
		if (!DestroyDisqualified)
		{
			if (DestroyedTotal > DESTROYED_MIN)
			{
                
				Goal oDestroy = new Goal () {
					goalText = "Explosions...Explosions everywhere.",
					pointValue = 100
				};
				GoalManager.gm.AddNewGoal (oDestroy);
				AddTimer (Time.time + TIMER_DELAY, oDestroy);
				DestroyDisqualified = true;
			}
		}

		//Doggy
		if (!DogDisqualified)
		{
			if ((float)WorldContainer.transform.childCount / (float)startCount < 0.3f)
			{
				Goal oGoal = gameObject.AddComponent<Goal> ();
				oGoal.goalText = "Man's Best Friend";
				oGoal.pointValue = 100;
				GoalManager.gm.AddNewGoal (oGoal);
				AddTimer (Time.time + TIMER_DELAY, oGoal);
				DogDisqualified = true;
			}
		}



	}

	//No Talking
	private const float NO_TALKING_MIN = 60f;
	private float TotalTalkUpdate = 0.0f;
	private bool TalkDisqualified = false;

	private void NoTalk (TalkEvent e)
	{
		TalkDisqualified = true;
	}

	//MostObject Destroyed
	private const int DESTROYED_MIN = 20;
	private int DestroyedTotal = 0;
	private bool DestroyDisqualified = false;

	private void DestroyedObject (DestroyEvent e)
	{
		DestroyedTotal += 1;
	}

	//Man's Best Friend
	private const string BEST_FRIEND_NAME = "FluffyDog";
	private bool DogDisqualified = false;
	private int startCount;

	private void CheckDogDeath (DestroyEvent e)
	{
		if (e.Name.Contains ("FluffyDog"))
		{
			DogDisqualified = true;
		}
	}
}

