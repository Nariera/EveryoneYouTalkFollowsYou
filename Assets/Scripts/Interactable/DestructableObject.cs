using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class DestructableObject : MonoBehaviour
{
	public Rigidbody Body;
	private Collider Hitbox;

	public float Durability = DURABILITY_BASE;

	private const float DURABILITY_BASE = 50.0f;

	private const float EXPLOSION_FORCE_BASE = 100.0f;
	private const float EXPLOSION_RADIUS_BASE = 5.0f;
	private const float EXPLOSION_UPWARD = 1.25f;

	bool exploded;

	public System.Action destroyed;

	private float Size
	{
		get
		{
			if (Hitbox != null)
			{
				return Hitbox.bounds.size.magnitude * 4;
			} else
			{
				return 1;
			}
		}
	}

	private static Dictionary<GameObject, DestructableObject> Library = new Dictionary<GameObject, DestructableObject> ();

	public static DestructableObject Get (GameObject a_oKey)
	{
		if (Library.ContainsKey (a_oKey))
		{
			return Library [a_oKey];
		} else
		{
			return null;
		}
	}


	private void Start ()
	{

		if (Body == null)
		{
			Body = GetComponent<Rigidbody> ();
		}
		if (Hitbox == null)
		{
			Hitbox = GetComponent<Collider> ();
		}
		Durability = Durability * Size * Mathf.Sqrt (Body.mass);

		Library.Add (gameObject, this);
	}

	private void OnDestroy ()
	{
		if (Library.ContainsKey (gameObject))
		{
			Library.Remove (gameObject);
		}
	}

	private void Update ()
	{
		if (Durability < 0 && !exploded)
		{
			Kill ();
		}
	}

	public void Kill ()
	{
		GoalEvents.Instance.Raise (new DestroyEvent () {
			Name = gameObject.name
		});
		exploded = true;
		var audio = GetComponentInChildren<AudioSource> ();
		if (audio)
			audio.Play ();
		var particle = GetComponentInChildren<ParticleSystem> ();
		particle.transform.SetParent (ParticleManager.pm.transform);
		particle.Play ();

		if (destroyed != null)
			destroyed.Invoke ();

		StartCoroutine (DestroyOnParticleLoss (particle));
	}

	System.Collections.IEnumerator DestroyOnParticleLoss (ParticleSystem target)
	{
//		Renderer oRender = GetComponent<Renderer> ();
//		if (oRender != null)
//		{
//			GetComponent<Renderer> ().enabled = false;
//		}    
//		Hitbox.enabled = false;
		gameObject.SetActive (false);

		yield return new WaitUntil (() => target.isStopped);


		Destroy (target.gameObject);
		Destroy (gameObject);
	}

	private void OnCollisionEnter (Collision a_oCollision)
	{
		float mass = 1;

		//no impact with ground...
		if (a_oCollision.collider.tag == "Terrain")
		{
			mass = 0.1f;
		} else if (a_oCollision.collider.attachedRigidbody != null)
		{
			mass = a_oCollision.collider.attachedRigidbody.mass;
		}
		//we want to determine the force of the other object;
		Vector3 v3ImpactForce = a_oCollision.impulse;

		Durability -= v3ImpactForce.magnitude * mass / Mathf.Sqrt (Body.mass);
	}

}

