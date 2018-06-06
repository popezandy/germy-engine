using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{

    /*
	 * Further iterations of this script need to modify chase 
	 * to be on a seperate script. Also, chase needs to be
	 * controlled by constraints placed on movement of
	 * the enemy, including pathing and height from the ground.
	 * 
	 */

    #region Variables

    [Header("Rotation Controls")]
    public float maxRot = 45;
    public float turnSpeed = 6;
    public float moveSpeed = 3;

    private Vector3 velocity;

    [Header("Don't Fuck With This")]
    public bool curveRotation = false;
    private float curveRot;



    private FOV FieldOfView;
    private FOH FieldOfHearing;
    private bool targetDetected;

    public GameObject playerObject;
    private Transform playerTransform;

    #endregion

    #region Callbacks

    // Use this for initialization
    void Start()
    {
        FindComponents();
    }

    // Update is called once per frame
    void Update()
    {

        GetVariables();

        if (targetDetected)
        {

            ChaseIf();

        }
        else if (!targetDetected)
        {

            RotationPatrol();

        }

    }

    #endregion

    #region Custom Methods

    void GetVariables()
    {

        /*This is insufficient and requires a proper chase function. 
         and example would be target detected if any detection method turns true.
         Chase until player is outside of area of patrol, or until
         player is outside of pen and all detection is false, or until within a
         certain distance of the player. If any of the first 3 situations are met, transfer
         to return to start state. If 4th situation (distance to player) is met, transfer to kill player state*/ 
        playerTransform = playerObject.transform;
        targetDetected = (FieldOfView.isInFOV || FieldOfHearing.isInFOH);

        Vector3 displacementFromTarget = playerTransform.position - transform.position;
        Vector3 directionToTarget = displacementFromTarget.normalized;
        velocity = new Vector3(directionToTarget.x * moveSpeed, 0, directionToTarget.z * moveSpeed);
    }

    void FindComponents()
    {
        //Determines which components an enemy has and which he doesn't.

        if (gameObject.GetComponent<FOV>() != null)
        {
            FieldOfView = gameObject.GetComponent<FOV>();
        }

        if (gameObject.GetComponent<FOH>() != null)
        {
            FieldOfHearing = gameObject.GetComponent<FOH>();
        }

    }

    void RotationPatrol()
    {
        if (!curveRotation == true)
        {
            transform.rotation = Quaternion.Euler(0f, maxRot * Mathf.Sin(Time.time * turnSpeed), 0f);
        }
        else if (curveRotation == true)
        {
            transform.rotation = Quaternion.Euler(0f, maxRot * Mathf.Sin(Time.time * turnSpeed), 0f);
        }
    }

    void ChaseIf()
    {
        transform.position += (velocity * Time.deltaTime);
        transform.LookAt(playerTransform);
    }
    #endregion

}