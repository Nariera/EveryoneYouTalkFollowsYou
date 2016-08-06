using UnityEngine;
using System.Collections;

public class SpinToWin : MonoBehaviour
{

	public int angularVelocity;

	void Update ()
	{
		transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x,
			transform.localEulerAngles.y,
			transform.localEulerAngles.z + angularVelocity);
	}
}
