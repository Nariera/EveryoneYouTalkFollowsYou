﻿using UnityEngine;
using UnityEngine.UI;

public class DogEvent : GoalEvent
{

}

public sealed class MasterControl : MonoBehaviour
{
	private const bool DEBUG_COMMAND = false;

	private const float DESTRUCTION_DELAY = 0.05f;

	private const float VORTEX_XZ_MIN = -450f;
	private const float VORTEX_XZ_MAX = 450f;
	private const float VORTEX_Y_MIN = 0f;
	private const float VORTEX_Y_MAX = 50f;
	private const float VORTEX_STRENGTH = 10f;

	private const float CONSECRATION_GRAVITY = 10f;
	private const float CONSECRATION_DURATION = 5f;


	[SerializeField]
	private GameObject World;
	[SerializeField]
	private GameObject Player;

	private bool VortexActive = false;
	private GameObject VortexTarget = null;
	private Vector3 VortexRandom = Vector3.zero;

	private Vector3 VortexPoint
	{
		get
		{
			if (VortexTarget != null)
			{
				return VortexTarget.transform.position;
			} else
			{
				return VortexRandom;
			}
		}
	}

	private bool ConsecrationActive = false;
	private float ConsecrationTime = 0.0f;
	private Vector3 PreviousGravity = Vector3.zero;

	public bool Usable
	{
		get
		{
			return (World != null) && (Player != null) && _Usable;
		}
	}

	[SerializeField]
	private bool _Usable = true;

	[SerializeField]
	private Goal MainGoal;
	private bool Triggered = false;
	private bool SuicideTriggered = false;

	private void Start ()
	{
		GoalEvents.Instance.AddListener<DogEvent> (DogEnd);
		if (World == null)
		{
			World = GameObject.Find ("World Objects");
		}
		if (Player == null)
		{
			Player = GameObject.FindGameObjectWithTag ("Player");
		}
	}

	private void DogEnd (DogEvent e)
	{
		EndGame ();
	}

	private void OnTriggerEnter (Collider a_oEnter)
	{
		if (a_oEnter.gameObject.tag == "Player" && !Triggered && Usable)
		{
			if (MainGoal != null)
			{
				GoalManager.gm.CompleteGoal (MainGoal);
			}
			EndGame ();
			int rand = Random.Range (0, 3);
			switch (rand)
			{
			case 0:
				BlowEverythingUp ();
				break;
			case 1:
				EnableVortex ();
				break;
			case 2:
				Consecration ();
				break;
			default:
				break;
			}
			Triggered = true;

		}


	}

	private void Update ()
	{
		if (DEBUG_COMMAND && Usable)
		{
			if (Input.GetKeyUp ("k"))
			{
				BlowEverythingUp ();
			}
			if (Input.GetKeyUp ("l"))
			{
				EnableVortex ();
			}
			if (Input.GetKeyUp ("o"))
			{
				Consecration ();
			}
		}

		if (Input.GetKeyUp (KeyCode.Escape))
			Application.Quit ();

		if (Usable)
		{
			float PlayerYAxis = Player.transform.position.y;
			if (PlayerYAxis < -100 && !SuicideTriggered)
			{
				SuicideTriggered = true;
				Goal Suicide = gameObject.AddComponent<Goal> ();
				Suicide.goalText = "Don't you hate infinite falling glitch?";
				Suicide.pointValue = 0;
				GoalManager.gm.AddNewGoal (Suicide);
				GoalManager.gm.CompleteGoal (Suicide);
			}
		}

	}

	private void FixedUpdate ()
	{
		if (VortexActive)
		{
			if (VortexTarget != null)
			{
				Vortex (VortexTarget.transform.position, VORTEX_STRENGTH);
			} else
			{
				Vortex (VortexRandom, VORTEX_STRENGTH);
			}
		}
		if (ConsecrationActive)
		{
			if (ConsecrationTime < CONSECRATION_DURATION)
			{
				ConsecrationTime += Time.deltaTime;
			} else
			{
				Physics.gravity = PreviousGravity;
				ConsecrationActive = false;

			}
		}
	}

	private void DisableMovement (bool a_bEnable = false)
	{
		for (int i = 0; i < World.transform.childCount; i++)
		{
			GameObject goPoorSap = World.transform.GetChild (i).gameObject;
			InteractableObject oInter = InteractableObject.Get (goPoorSap);
			oInter.enabled = a_bEnable;
		}
	}

	private void BlowEverythingUp ()
	{
		for (int i = 0; i < World.transform.childCount; i++)
		{
			try
			{
				GameObject goPoorSap = World.transform.GetChild (i).gameObject;
				DestructableObject oDestructable = DestructableObject.Get (goPoorSap);
				if (oDestructable != null)
				{
					oDestructable.Kill ();
				}
			} catch
			{
				//do nothing
			}

		}
	}

	private void EnableVortex ()
	{
		VortexActive = true;
		int nRandom = Random.Range (0, 2);
		switch (nRandom)
		{
		case 0:
			{
				VortexTarget = Player;
				break;
			}
		case 1: //could assign something here
		case 2: //could assign something here too
		default:
			{
				Vector3 v3RandomLocation = new Vector3 (
					                           UnityEngine.Random.Range (VORTEX_XZ_MIN, VORTEX_XZ_MAX),
					                           UnityEngine.Random.Range (VORTEX_Y_MIN, VORTEX_Y_MAX),
					                           UnityEngine.Random.Range (VORTEX_XZ_MIN, VORTEX_XZ_MAX)
				                           );
				VortexTarget = null;
				VortexRandom = v3RandomLocation;
				break;
			}
		}
		DisableMovement ();
	}

	private void Vortex (Vector3 a_v3Loc, float a_fPullMultiplier)
	{
		for (int i = 0; i < World.transform.childCount; i++)
		{
			GameObject goPoorSap = World.transform.GetChild (i).gameObject;
			DestructableObject oInter = DestructableObject.Get (goPoorSap);
			if (oInter != null && oInter.Body != null)
			{
				Vector3 v3Diff = a_v3Loc - goPoorSap.transform.position;
				oInter.Body.velocity = v3Diff.normalized * a_fPullMultiplier;
			}


		}
	}

	private void Consecration ()
	{
		ConsecrationActive = true;
		DisableMovement ();
		PreviousGravity = Physics.gravity;
		ConsecrationTime = 0.0f;
		Physics.gravity = new Vector3 (0, CONSECRATION_GRAVITY, 0);
	}

	public Text abd;
	public Image acd;

	private void EndGame ()
	{
		abd.enabled = true;
		acd.enabled = true;

		int score = GoalManager.gm.TallyCompletedPoints ();
		abd.text = score.ToString () + "\n";

		if (score < 0)
		{
			abd.text += "lol\nhow noble\nmuch fun";
		} else if (score > 20)
		{
			abd.text += "\"cause no harm\"\nwhat was so hard about that\nyou sick fuck";
		} else
		{
			abd.text += "i guess you played";
		}

		abd.text += "\n\n\n" + "Alec Asperslag\n" + "Stan Tsai\n" + "and Patrick Scott";
	}
}

