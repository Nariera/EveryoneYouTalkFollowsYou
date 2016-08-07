using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class ExplosionParticleFactory
{
    public static ExplosionParticleFactory Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new ExplosionParticleFactory();
            }
            return _Instance;
        }
    }
    private static ExplosionParticleFactory _Instance;

    private GameObject ParticleContainer;
    private GameObject ParticlePrefab;
    private Stack<ExplosionParticle> ParticlePool = new Stack<ExplosionParticle>();

    private const float DISPERSION_STRENGTH = 200f;
    private const float DISPERSION_RADIUS = 5f;
    private const float DISPERSION_OFFSET = 0.125f;
    private const int PARTICLE_COUNT_BASE = 20;
    private const string PARTICLE_PATH = "Explosion Particle";

    private ExplosionParticleFactory()
    {
        ParticleContainer = new GameObject();
        ParticleContainer.name = "Particle Container";
        ParticleContainer.transform.position = Vector3.zero;
        ParticlePrefab = Resources.Load<GameObject>(PARTICLE_PATH);
    }

    /// <summary>
    /// Create an explosion of particle at the position
    /// </summary>
    /// <param name="a_v3Position"></param>
    /// <param name="a_fSize"></param>
    public void Explode(Vector3 a_v3Position, float a_fSize)
    {
        int nParticleCount = (int) (a_fSize * PARTICLE_COUNT_BASE) ;
        for(int i = 1; i <= nParticleCount; i++)
        {
            ExplosionParticle oParticle = GetParticle();
            Vector3 v3ParticlePosition = a_v3Position;
            v3ParticlePosition.x += UnityEngine.Random.Range(-DISPERSION_OFFSET, DISPERSION_OFFSET);
            v3ParticlePosition.y += UnityEngine.Random.Range(-DISPERSION_OFFSET, DISPERSION_OFFSET);
            v3ParticlePosition.z += UnityEngine.Random.Range(-DISPERSION_OFFSET, DISPERSION_OFFSET);
            oParticle.transform.position = v3ParticlePosition;
            oParticle.gameObject.SetActive(true);
            oParticle.Body.AddExplosionForce(DISPERSION_STRENGTH, a_v3Position, DISPERSION_RADIUS);
        }
    }

    private ExplosionParticle GetParticle()
    {
        if(ParticlePool.Count == 0)
        {
            //we instantiate a new one
            GameObject goNewParticle = GameObject.Instantiate<GameObject>(ParticlePrefab);
            ExplosionParticle oParticle = goNewParticle.GetComponent<ExplosionParticle>();
            oParticle.gameObject.transform.parent = ParticleContainer.transform;
            oParticle.gameObject.SetActive(false);
            oParticle.OnExpire += ParticleExpire;

            return oParticle;
        }
        else
        {
            return ParticlePool.Pop();
        }
    }

    private void ParticleExpire(ExplosionParticle a_oParticle)
    {
        a_oParticle.gameObject.SetActive(false);
        ParticlePool.Push(a_oParticle);
    }
}

