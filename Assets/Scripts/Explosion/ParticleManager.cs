using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour
{
	public static ParticleManager pm;

	public GameObject particlesPrefab;
	public GameObject meshGroup;
    public GameObject SomeBox;
	void Start ()
	{
		//Assign a particle renderer for each MeshRenderer in world
		foreach (var t in meshGroup.GetComponentsInChildren<DestructableObject>())
		{
			CreateAndAssignParticlesTo (t.gameObject);
		}
	}

	/** Need a particle for boom boom? Buy one here for free. -P */
	public void CreateAndAssignParticlesTo (GameObject explodey)
	{

		GameObject particles = (GameObject)Instantiate (particlesPrefab);
		particles.transform.SetParent (explodey.transform);
		particles.transform.localPosition = Vector3.zero;
		var shape = particles.GetComponent<ParticleSystem> ().shape;
        MeshRenderer renderer = explodey.GetComponent<MeshRenderer>();
        if(renderer != null)
        {
            shape.meshRenderer = renderer;
        }
	}

	void Awake ()
	{
		if (pm == null)
			pm = this;
		else if (pm != this)
			Destroy (this);
	}

}
