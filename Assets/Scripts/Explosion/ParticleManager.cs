using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour
{
	public static ParticleManager pm;

	public GameObject particlesPrefab;
	public GameObject meshGroup;

	void Start ()
	{
		//Assign a particle renderer for each MeshRenderer in world
		foreach (var t in meshGroup.GetComponentsInChildren<MeshRenderer>())
		{
			if (t.GetComponent<DestructableObject> () != null)
				CreateAndAssignParticlesTo (t);
		}
	}

	/** Need a particle for boom boom? Buy one here for free. -P */
	public void CreateAndAssignParticlesTo (MeshRenderer explodey)
	{
		GameObject particles = (GameObject)Instantiate (particlesPrefab);
		particles.transform.SetParent (explodey.transform);
		particles.transform.localPosition = Vector3.zero;
		var shape = particles.GetComponent<ParticleSystem> ().shape;
		shape.meshRenderer = explodey;
	}

	void Awake ()
	{
		if (pm == null)
			pm = this;
		else if (pm != this)
			Destroy (this);
	}

}
