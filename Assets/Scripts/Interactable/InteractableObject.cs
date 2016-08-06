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

    public static InteractableObject Get(GameObject a_goTarget)
    {
        return null;
    }

    private void Awake()
    {
        Debug.Log("Aywake");
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
        Debug.Log("Object Start");
    }

    /// <summary>
    /// We need to enable this later
    /// </summary>
    private void OnEnable()
    {
        //Randomize Movement
    }

    private void OnDisable()
    {
        Debug.Log("On Disable");
    }
    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 v3PlayerLocation = TestPlayer.transform.position;

        Vector3 v3Difference = v3PlayerLocation - gameObject.transform.position;

        body.AddForce(v3Difference.normalized);
    }
}
