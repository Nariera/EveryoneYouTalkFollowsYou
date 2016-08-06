using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Movement Delegate that object runs
/// </summary>
/// <param name="a_oTarget">Target the object is going towards</param>
/// <param name="a_oBody">Object's Rigidbody</param>
public delegate void MovementScript (GameObject a_oTarget,Rigidbody a_oBody);

public static class Movement
{
	private static float MIN_JUMPING_INTERVAL = 2;
	private static float MAX_JUMPING_INTERVAL = 5;

	private static Dictionary<GameObject, float> LastUpdateLibrary = new Dictionary<GameObject, float> ();

	//Pool of movement scripts
	private static Dictionary<string, Func<MovementScript>> MovementPool = CreateMovementList ();

	//Creates the list the first time
	private static Dictionary<string, Func<MovementScript>> CreateMovementList ()
	{
		Dictionary<string, Func<MovementScript>> kyMovementPool = new Dictionary<string, Func<MovementScript>> ();

		Type oMovementType = typeof(Movement);
		var aoMovementMethods = oMovementType.GetMethods (BindingFlags.Public | BindingFlags.Static).Where (o => o.ReturnType == typeof(MovementScript) && o.Name != "GetScript");
		foreach (var oMethod in aoMovementMethods)
		{
			string sMethodName = oMethod.Name;
            
			var oDelegate = Delegate.CreateDelegate (typeof(Func<MovementScript>), oMethod) as Func<MovementScript>;
			//Func<MovementScript> oDelegate = () => oMethod.Invoke(null, null) as MovementScript;
			kyMovementPool.Add (sMethodName, oDelegate);
		}
		return kyMovementPool;
	}

    /// <summary>
    /// Get a movement script
    /// </summary>
    /// <param name="a_sMethodName"></param>
    /// <returns></returns>
    public static MovementScript GetScript(string a_sMethodName = "")
    {
        Func<MovementScript> oReturnDelegate;
        if (a_sMethodName != "" && MovementPool.TryGetValue(a_sMethodName, out oReturnDelegate))
        {
            return oReturnDelegate();
        }
        else
        {
            //TODO: THIS CODE IS TERRIBLE
            oReturnDelegate = MovementPool.Values.First();
            int nChosenKey = UnityEngine.Random.Range(0, MovementPool.Count);
            int nCurrentKey = 0;
            foreach (var oDelegate in MovementPool.Values)
            {
                oReturnDelegate = oDelegate;
                if (nCurrentKey == nChosenKey)
                {
                    break;
                }
                nCurrentKey++;
            }
            return oReturnDelegate();
        }
    }
    //Doesn't work
    //private static IEnumerable<Func<MovementScript>> GetRandomValue()
    //{
    //    System.Random rand = new System.Random();
    //    List<Func<MovementScript>> oValues = Enumerable.ToList(MovementPool.Values);
    //    int nSize = MovementPool.Count;
    //    while (true)
    //    {
    //        yield return oValues[rand.Next(nSize)];
    //    }
    //}

	#region Updates

	/// <summary>
	/// Return last updated time
	/// </summary>
	/// <param name="a_goGameObject">Gameobject for movement</param>
	/// <returns></returns>
	private static float CheckUpdate (GameObject a_goGameObject)
	{
		float fUpdateTime;
		if (!LastUpdateLibrary.TryGetValue (a_goGameObject, out fUpdateTime))
		{
			LastUpdateLibrary.Add (a_goGameObject, 0);
			fUpdateTime = 0.0f;
		}
		return fUpdateTime;
	}

	/// <summary>
	/// Set updated time to the current time;
	/// </summary>
	/// <param name="a_goGameObject"></param>
	private static void UpdateTime (GameObject a_goGameObject)
	{
		float fUpdateTime;
		if (LastUpdateLibrary.TryGetValue (a_goGameObject, out fUpdateTime))
		{
			LastUpdateLibrary [a_goGameObject] = Time.time;
		} else
		{
			LastUpdateLibrary.Add (a_goGameObject, Time.time);
		}
	}

	#endregion

	public static MovementScript DragBody ()
	{
		float fMagnitude = UnityEngine.Random.Range (1, 3);
		return (a_oTarget, a_oBody) => DragBodyScript (a_oTarget, a_oBody, fMagnitude);
	}

	private static void DragBodyScript (GameObject a_oTarget, Rigidbody a_oBody, float a_fMagnitude)
	{
		Vector3 v3PlayerLocation = a_oTarget.transform.position;
		Vector3 v3MovingObjectLocation = a_oBody.gameObject.transform.position;

		Vector3 v3Difference = v3PlayerLocation - v3MovingObjectLocation;

		a_oBody.AddForce (v3Difference * a_fMagnitude, ForceMode.Force);
	}

	#region JumpToward

	public static MovementScript JumpToward ()
	{
		float fAngle = UnityEngine.Random.Range (45, 90) * Mathf.Deg2Rad;
		float fJumpInterval = UnityEngine.Random.Range (MIN_JUMPING_INTERVAL, MAX_JUMPING_INTERVAL);
		return (a_oTarget, a_oBody) => JumpTowardScript (a_oTarget, a_oBody, fJumpInterval, fAngle);
	}

	private static void JumpTowardScript (GameObject a_oTarget, Rigidbody a_oBody, float a_fInterval, float a_fAngle)
	{
		float fLastUpdate = CheckUpdate (a_oBody.gameObject);
		if (Time.time - fLastUpdate > a_fInterval)
		{
			Vector3 v3PlayerLocation = a_oTarget.transform.position;
			Vector3 v3MovingObjectLocation = a_oBody.gameObject.transform.position;

			Vector3 v3PlanePlayerLocation = new Vector3 (v3PlayerLocation.x, 0, v3PlayerLocation.z);
			Vector3 v3PlaneMovingLocation = new Vector3 (v3MovingObjectLocation.x, 0, v3MovingObjectLocation.z);

			//delta x between two object
			float fGroundDistance = Vector3.Distance (v3PlanePlayerLocation, v3PlaneMovingLocation);

			//height distance - y
			float fHeightDistance = v3MovingObjectLocation.y - v3PlayerLocation.y;

			float fInitialVelocity = (1 / Mathf.Cos (a_fAngle)) * Mathf.Sqrt ((0.5f * Physics.gravity.magnitude * Mathf.Pow (fGroundDistance, 2)) / (fGroundDistance * Mathf.Tan (a_fAngle) + fHeightDistance));

			Vector3 v3Velocity = new Vector3 (0, fInitialVelocity * Mathf.Sin (a_fAngle), fInitialVelocity * Mathf.Cos (a_fAngle));

			float fAngleDifference = Vector3.Angle (Vector3.forward, v3PlanePlayerLocation - v3PlaneMovingLocation);
			Vector3 v3FinalVelocity = Quaternion.AngleAxis (fAngleDifference, Vector3.up) * v3Velocity;

			//apply motion
			a_oBody.AddForce (v3FinalVelocity * a_oBody.mass, ForceMode.Impulse);

			//update the last updated time
			UpdateTime (a_oBody.gameObject);
		}

	}

	#endregion

	//Pat was here

	#region TwinkleToes

	public static MovementScript TwinkleToes ()
	{
		return (target, dancer) => TwinkleToesScript (target, dancer);
	}

    private static void TwinkleToesScript (GameObject target, Rigidbody dancer)
    {
        InteractableObject feet = InteractableObject.Get (dancer.gameObject);

        int choice = UnityEngine.Random.Range (0, 4);
        //Supes stoked when close
        float excitement = 100 / Vector3.Distance (target.transform.position, dancer.transform.position);

        //Calm when together
        excitement = excitement > 50 ? 0 : excitement;

        //Hop
        if (choice == 0 && feet.onGround)
        {
            dancer.AddForce (dancer.mass * 2 * excitement * Vector3.up);
        }

        //Spin
        if (choice == 0 && feet.onGround)
        {
            dancer.AddRelativeTorque (dancer.mass * 10 * excitement * Vector3.up);
        }

        //Skip
        if (choice != 0 && feet.onGround)
        {
            var dir = target.transform.position - dancer.transform.position;

            dancer.AddForce (dancer.mass * excitement * (Vector3.up));
            dancer.AddForce (dancer.mass * excitement * (dir));
        }
    }

    #endregion

    #region LubeCannon

    #endregion
}

