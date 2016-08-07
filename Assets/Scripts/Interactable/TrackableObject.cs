using UnityEngine;
using System.Collections;

public class TrackableObject : MonoBehaviour
{

	public System.Action seen;


	public void Seen ()
	{
		if (seen != null)
			seen.Invoke ();
	}
}