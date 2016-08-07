using UnityEngine;

public class Talk : MonoBehaviour
{
	[SerializeField]
	private Camera playerView;

	public UnityEngine.UI.Image cursor;
	private RectTransform cursorRect;

	float lerp = 0;
	GameObject lastObjHit;

	//Max distance of interaction
	private const float MAX_INTERACT_DISTANCE = 8.0f;

	void Start ()
	{
		if (cursor)
			cursorRect = cursor.GetComponent<RectTransform> ();
	}

	// Update is called once per frame
	void Update ()
	{

		RaycastHit rcHit;
		//Raycast from center of camera view to a max distance          
		if (Physics.Raycast (playerView.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2)), out rcHit, MAX_INTERACT_DISTANCE)
		    && InteractableObject.Get (rcHit.collider.gameObject))
		{
			//If can interact, indicate
			if (cursor)
			{
				if (InteractableObject.Get (rcHit.collider.gameObject).isActiveAndEnabled)
				{
					cursor.color = Color.magenta;

					//Reset
					lerp = 0;
				} else if (lastObjHit != rcHit.collider.gameObject)
				{
					//Set last obj we hit
					lastObjHit = rcHit.collider.gameObject;

					//And flash another color
					cursor.color = Color.yellow;

					lerp = -0.5f;
				} else if (lerp < 1)
				{
					lerp += Time.deltaTime * 4;

					if (lerp > 0)
						cursor.color = Color.cyan;
				}

				cursorRect.localScale = Vector3.one * (2 - lerp) / 2;
			}

			//Run Talk Script if player is interacting
			if (Input.GetKeyUp ("e") || Input.GetMouseButtonDown (0))
			{
				GoalEvents.Instance.Raise (new TalkEvent ());
				//Talk Here;
				GameObject goTarget = rcHit.collider.gameObject;

				//tell the object to follow
				InteractableObject.Follow (goTarget);
			}
			//Otherwise, indicate that it has been seen
			else
			{
				var seen = rcHit.collider.gameObject.GetComponent<TrackableObject> ();
				if (seen)
					seen.Seen ();
			}
		} else if (cursor)
		{
			if (lerp > 0)
				lerp -= Time.deltaTime;
			if (lerp < 0)
				lerp = 0;

			cursorRect.localScale = Vector3.one * (2 - lerp) / 2;
			cursor.color = Color.white;
		}
	}
}
