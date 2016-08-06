using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject TestPlayer;

    [SerializeField]
    private Rigidbody body;
    private static Dictionary<GameObject, InteractableObject> library = new Dictionary<GameObject, InteractableObject>();

    public static void Follow(GameObject a_goTarget)
    {
        InteractableObject oInteractable;
        
        if(library.TryGetValue(a_goTarget, out oInteractable) && !oInteractable.isActiveAndEnabled)
        {
            Debug.Log(oInteractable.name + " is following!");
            oInteractable.enabled = true;
        }
    }


    //These two lines are to assign code that allow easy retrieval of this component
    private void Awake()
    {
        if (library.ContainsKey(this.gameObject))
        {
            //get rid of it,
            library.Remove(this.gameObject);
        }
        library.Add(this.gameObject, this);       
    }
    private void OnDestroy()
    {
        if (library.ContainsKey(this.gameObject))
        {
            library.Remove(this.gameObject);
        }
    }


    // Use this for initialization
    private void Start()
    {
        
    }

    /// <summary>
    /// Use this to call any On Activation events
    /// </summary>
    private void OnEnable()
    {
        //Randomize Movement?
    }

    private void OnDisable()
    {
        
    }
    // Update is called once per frame
    private void Update()
    {

    }

    /// <summary>
    /// Movement Script Here
    /// </summary>
    private void FixedUpdate()
    {
        //Drag Body Script
        Vector3 v3PlayerLocation = TestPlayer.transform.position;

        Vector3 v3Difference = v3PlayerLocation - gameObject.transform.position;

        body.AddForce(v3Difference * 2);
    }
}
