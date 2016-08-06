﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private Rigidbody Body;

    private MovementScript CurrentMovement;

    //Use this to get the interactableobject
    private static Dictionary<GameObject, InteractableObject> library = new Dictionary<GameObject, InteractableObject>();

    public static void Follow(GameObject a_goTarget)
    {
        InteractableObject oInteractable;
        
        if(library.TryGetValue(a_goTarget, out oInteractable) && !oInteractable.isActiveAndEnabled)
        {
            Debug.Log(oInteractable.name + " is following!");
            oInteractable.enabled = true;

            //Add other tracking code if you want;
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

    private void Start()
    {
        //find player if player is null
        if(Player == null)
        {

        }
    }

    /// <summary>
    /// Use this to call any On Activation events
    /// </summary>
    private void OnEnable()
    {
        CurrentMovement = Movement.GetScript();
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
        CurrentMovement(Player, Body);
        //Vector3 v3PlayerLocation = Player.transform.position;
        //Vector3 v3MovingObjectLocation = gameObject.transform.position;
        //Vector3 v3Difference = v3PlayerLocation - v3MovingObjectLocation;
        ////Drag Body Script
        //if (false)
        //{
        //    Body.AddForce(v3Difference * 2, ForceMode.Impulse);
        //}
        //if(false)
        //{
        //    //Jump Script
        //    float fAngle = 45 * Mathf.Deg2Rad;

        //    Vector3 v3PlanePlayerLocation = new Vector3(v3PlayerLocation.x, 0, v3PlayerLocation.z);
        //    Vector3 v3PlaneMovingLocation = new Vector3(v3MovingObjectLocation.x, 0, v3MovingObjectLocation.z);

        //    //delta x between two object
        //    float fGroundDistance = Vector3.Distance(v3PlanePlayerLocation, v3PlaneMovingLocation);

        //    //height distance - y
        //    float fHeightDistance = v3MovingObjectLocation.y - v3PlayerLocation.y;

        //    float fInitialVelocity = (1 / Mathf.Cos(fAngle)) * Mathf.Sqrt((0.5f * Physics.gravity.magnitude * Mathf.Pow(fGroundDistance, 2)) / (fGroundDistance * Mathf.Tan(fAngle) + fHeightDistance));

        //    Vector3 v3Velocity = new Vector3(0, fInitialVelocity * Mathf.Sin(fAngle), fInitialVelocity * Mathf.Cos(fAngle));

        //    float fAngleDifference = Vector3.Angle(Vector3.forward, v3PlanePlayerLocation - v3PlaneMovingLocation);
        //    Vector3 v3FinalVelocity = Quaternion.AngleAxis(fAngleDifference, Vector3.up) * v3Velocity;
        //    Body.AddForce(v3FinalVelocity * Body.mass, ForceMode.Impulse);

        //    LastJump = Time.time;
        //}



    }
    private float LastJump = 0f;

}
