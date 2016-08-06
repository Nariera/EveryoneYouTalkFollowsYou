using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public delegate void ParticleEvent(ExplosionParticle a_oParticle);

public class ExplosionParticle : MonoBehaviour
{
    
    private const float PARTICLE_EXPIRATION = 5f;

    [SerializeField]
    private float TimeAlive = 0f;

    public Rigidbody Body;

    public event ParticleEvent OnExpire;

    private void OnEnable()
    {
        TimeAlive = 0;
    }

    private void Update()
    {
        TimeAlive += Time.deltaTime;
        if (TimeAlive > PARTICLE_EXPIRATION)
        {
            if (OnExpire != null)
            {
                OnExpire(this);
            }
            else
            {
                //we do this since we can't get rid of it through the particle
                Destroy(this.gameObject);
            }
        }
    }

}

