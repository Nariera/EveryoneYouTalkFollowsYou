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
    private float Size
    {
        get
        {
            if (Hitbox != null)
            {
                return Hitbox.bounds.size.magnitude;
            }
            else
            {
                return 1;
            }
        }
    }

    private void Start()
    {
        
        if (Body == null)
        {
            Body = GetComponent<Rigidbody>();
        }
        if (Hitbox == null)
        {
            Hitbox = GetComponent<Collider>();
        }
        Durability = Durability * Size;
    }

    private void Update()
    {
        if(Durability < 0)
        {
            Explode();
        }
    }

    /// <summary>
    /// Ayy...EXPLOSIONS!
    /// </summary>
    public void Explode()
    {
        var acolHits = Physics.OverlapSphere(transform.position, EXPLOSION_RADIUS_BASE);
        foreach(var oHit in acolHits)
        { 
            //ignore terrain
            if(oHit.tag == "Terrain")
            {
                continue;
            }
            Rigidbody oBody = oHit.GetComponent<Rigidbody>();
            if(oBody != null)
            {
            oBody.AddExplosionForce(EXPLOSION_FORCE_BASE * Size, transform.position, EXPLOSION_RADIUS_BASE * Size, EXPLOSION_UPWARD);
            }
        }
        ExplosionParticleFactory.Instance.Explode(transform.position, Size);
        Debug.Log(gameObject.name + " has exploded.");

        GameObject.Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision a_oCollision)
    {
        //no impact with ground...
        if (a_oCollision.collider.tag == "Terrain")
        {
            return;
        }
        //we want to determine the force of the other object;
        Vector3 v3ImpactForce = a_oCollision.impulse;

        Durability -= v3ImpactForce.magnitude * a_oCollision.collider.bounds.size.magnitude;
    }

}

