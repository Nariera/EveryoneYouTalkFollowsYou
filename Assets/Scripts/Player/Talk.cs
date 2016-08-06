using UnityEngine;

public class Talk : MonoBehaviour
{
	[SerializeField]
	private Camera playerView;

	//Max distance of interaction
	private const float MAX_INTERACT_DISTANCE = 8.0f;
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		//Run Talk Script here
		if (Input.GetKeyUp ("e") || Input.GetMouseButtonDown (0))
		{
			RaycastHit rcHit;
			//Raycast from center of camera view to a max distance          
			if (Physics.Raycast (playerView.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2)), out rcHit, MAX_INTERACT_DISTANCE))
			{
				//Talk Here;
				GameObject goTarget = rcHit.collider.gameObject;

				//tell the object to follow
				InteractableObject.Follow (goTarget);
			}
          
		}
	}
}
