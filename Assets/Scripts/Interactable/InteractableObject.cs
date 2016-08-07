using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractEvent : GoalEvent
{
	public string Source { get; set; }
}

[RequireComponent (typeof(TrackableObject))]
public sealed class InteractableObject : MonoBehaviour
{
	[SerializeField]
	private GameObject Player;

	[SerializeField]
	private Rigidbody Body;

	public string MovementType;
	private string _LastMovementType;

	private float LastMovementChange = 0.0f;
	private const float MOVEMENT_INTERVAL_MIN = 1.0f;

	//Skip running physics move if it is this close
	private const float MOVEMENT_STOP_MIN = 2.0f;
	private const float MOVEMENT_STOP_MAX = 80.0f;

	private MovementScript CurrentMovement;

	public bool onGround { get; private set; }

	public static System.Action<InteractableObject> activated;

	//Use this to get the interactableobject
	private static Dictionary<GameObject, InteractableObject> library = new Dictionary<GameObject, InteractableObject> ();

	public static void Follow (GameObject a_goTarget)
	{
		InteractableObject oInteractable;

		if (library.TryGetValue (a_goTarget, out oInteractable) && !oInteractable.isActiveAndEnabled)
		{
			oInteractable.enabled = true;
			if (activated != null)
				activated.Invoke (oInteractable);

			GoalEvents.Instance.Raise (new InteractEvent () {
				Source = oInteractable.name
			});

			//Add other tracking code if you want;
		}
	}

	public static InteractableObject Get (GameObject a_goTarget)
	{
		InteractableObject oInteractable;

		if (library.TryGetValue (a_goTarget, out oInteractable))
		{
			return oInteractable;
		} else
		{
			return null;
		}
	}
	//These two lines are to assign code that allow easy retrieval of this component
	private void Awake ()
	{
		if (library.ContainsKey (this.gameObject))
		{
			//get rid of it,
			library.Remove (this.gameObject);
		}
		library.Add (this.gameObject, this);
	}

	private void OnDestroy ()
	{
		if (library.ContainsKey (this.gameObject))
		{
			library.Remove (this.gameObject);
		}
	}

	private void Start ()
	{
		Player = GameObject.FindGameObjectWithTag ("Player");
		Body = GetComponent<Rigidbody> ();

		//find player if player is null
		if (Player == null)
		{

		}
		if (Body == null)
		{
			Debug.Log (name + " does not have a rigidbody attached to it.");
		}
	}

	void OnCollisionEnter (Collision coll)
	{
		if (coll.collider.tag == "Terrain" || coll.collider.attachedRigidbody == null)
		{
			onGround = true;
		}
	}

	void OnCollisionExit (Collision coll)
	{
		if (coll.collider.tag == "Terrain" || coll.collider.attachedRigidbody == null)
		{
			onGround = false;
		}
	}

	/// <summary>
	/// Use this to call any On Activation events
	/// </summary>
	private void OnEnable ()
	{
		CurrentMovement = null; //remove references

		CurrentMovement = Movement.GetScript (MovementType);

		_LastMovementType = MovementType;
	}

	private void OnDisable ()
	{

	}
	// Update is called once per frame
	private void Update ()
	{
		CheckMovementType ();
	}

	private void CheckMovementType ()
	{
		if (MovementType != _LastMovementType && Time.time - LastMovementChange > MOVEMENT_INTERVAL_MIN)
		{
			CurrentMovement = null;
			CurrentMovement = Movement.GetScript (MovementType);
			_LastMovementType = MovementType;
			LastMovementChange = Time.time;
		}
	}

	/// <summary>
	/// Movement Script Here
	/// </summary>
	private void FixedUpdate ()
	{
		float fDistance = Vector3.Distance (Player.transform.position, transform.position);
		if (fDistance >= MOVEMENT_STOP_MIN && fDistance <= MOVEMENT_STOP_MAX)
			CurrentMovement (Player, Body);
	}


}
