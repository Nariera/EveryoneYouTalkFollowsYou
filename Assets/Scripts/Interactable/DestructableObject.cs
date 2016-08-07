using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class DestructableObject : MonoBehaviour
{
	private Rigidbody Body;
	private Collider Hitbox;

	public float Durability = DURABILITY_BASE;

	private const float DURABILITY_BASE = 50.0f;

	private const float EXPLOSION_FORCE_BASE = 100.0f;
	private const float EXPLOSION_RADIUS_BASE = 5.0f;
	private const float EXPLOSION_UPWARD = 1.25f;

	bool exploded;

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

	private static bool ExplosionInstantiateLock = false;
	//ayyy...lol..don't do this at home...seriously
	private void Awake ()
	{
		if (!ExplosionInstantiateLock)
		{
			ExplosionInstantiateLock = true;
			ExplosionParticleFactory.Instance.ToString ();
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
	}

	private void Update ()
	{
		if (Durability < 0 && !exploded)
		{
			GoalEvents.Instance.Raise (new DestroyEvent () {
				Name = gameObject.name
			});
			exploded = true;
			var audio = GetComponentInChildren<AudioSource> ();
			if (audio)
				audio.Play ();
			var particle =	GetComponentInChildren<ParticleSystem> ();
			particle.transform.SetParent (ParticleManager.pm.transform);
			particle.Play ();

			StartCoroutine (DestroyOnParticleLoss (particle));
			//Explode ();
		}
	}

	System.Collections.IEnumerator DestroyOnParticleLoss (ParticleSystem target)
	{
		GetComponent<Renderer> ().enabled = false;

		yield return new WaitUntil (() => target.isStopped);

        
		Destroy (target.gameObject);
		Destroy (gameObject);
	}

	/// <summary>
	/// Ayy...EXPLOSIONS!
	/// </summary>
	public void Explode ()
	{
		var acolHits = Physics.OverlapSphere (transform.position, EXPLOSION_RADIUS_BASE);
		foreach (var oHit in acolHits)
		{ 
			//ignore terrain
			if (oHit.tag == "Terrain")
			{
				continue;
			}
			Rigidbody oBody = oHit.GetComponent<Rigidbody> ();
			if (oBody != null)
			{
				oBody.AddExplosionForce (EXPLOSION_FORCE_BASE * Size, transform.position, EXPLOSION_RADIUS_BASE * Size, EXPLOSION_UPWARD);
			}
		}
		ExplosionParticleFactory.Instance.Explode (transform.position, Size);

       
		GameObject.Destroy (this.gameObject);
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

		Durability -= v3ImpactForce.magnitude * mass;
	}

}

