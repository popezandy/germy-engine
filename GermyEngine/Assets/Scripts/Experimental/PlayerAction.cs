using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    #region Class Description

    /*Lorem ipsum parmitsum canipsum waginger the blinger faninginer shplanininer*/

    #endregion

    #region ToDo

    /* Goals for the new Player
 * 1. Rigid Body Based movement. The player should be a rigid body with basic movement for now, and rooted IK movement for later.
 * 2. Grab System - basic grab functionality with placeholder holding spot for now, IK hold for tomorrow
 * 3. Cover System - NOTES: THE COVER SYSTEM IS NOT PERFECTLY MODULAR. MODIFY THIS SCRIPT TO OUTPUT INFORMATION TO THE INFO BUFFER, and MODIFY COVER SYSTEM TO REFERENCE INFO BUFFER
 * 4. Grab system currently rotates player on unfavorable axis in certain conditions, MAJOR BUG
 * 
 * 
 */

    #endregion

    #region Variables

    [Header("Player Options")]
    public float playerHeight = 2f;
    public float grabRange;

    [Header("Movement Options")]

    public float trueMove;
    public float turnSpeed;
    public bool smooth;
    public float smoothSpeed;


    [Header("Jump Options")]
    public float jumpForce;
    public float jumpSpeed;
    public float jumpDecrease;
    public float incrementJumpFallSpeed = 0.1f;

    [Header("Layer Masks")]
    public LayerMask discludePlayer;
    public LayerMask isGrabbable;

    [Header("References")]
    public SphereCollider sphereCol;

    [HideInInspector]
    public bool isSneaking, isSmelly, hasID, isInCover, disableControl, disableInput;
    //detection booleans

    //grabbing variables
    private Vector3 grabTargetLeft;
    private Vector3 grabTargetRight;
    private GameObject grabObject;

    //Private bools
    private bool isWalking;

    //Private variables

    private float sneakSpeed = 0.5f;
    private float movementSpeed;

    //Movement Vectors
    private Vector3 velocity;
    private Vector3 move;
    private Vector3 colVelocity;

    //Experimental or Unimplemented Variables
    private bool isBoosting;
    private float boostSpeed = 2.5f;
    private float boostMaxTime = 3.0f;
    private float boostCountdown;
    private bool isSneakWalking = false;

    public IActionMachine grabAction = new IActionMachine();

    #endregion

    #region Callbacks

    private void Awake()
    {
    }

    private void Update()
    {
        if (!disableControl)
        {
            isBoostingCheck();
            isWalkingCheck();
            SneakCheck();
            SimpleMove();
            Jump();
            FinalMove();
            GroundChecking();
            PickUp();
            setDynamicReferences();

            if (grabAction.currentState != null)
            {
                grabAction.ExecuteStateUpdate();
            }
        }

        else if (isBoosting)
        {
            BoostFunctions();
        }

        if (isSneakWalking)
        {
            if (gameObject.GetComponent<Player.CoverSystem>() != null)
            {
                //call cover system
                gameObject.GetComponent<Player.CoverSystem>().InitiateCoverSystem();
            }

        }

        disableControlCheck();
    }

    #endregion

    private void setDynamicReferences()
    {
        this.GetComponent<InfoBuffer>().isNoisy = !isSneaking;
    }

    #region Movement Basic

    private void SimpleMove()
    {
        move = Vector3.ClampMagnitude(new Vector3(0, 0, Input.GetAxis("Vertical")), 1);
        transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);
        velocity += move;
    }

    private void FinalMove()
    {

        Vector3 vel = new Vector3(velocity.x * movementSpeed, velocity.y * trueMove, velocity.z * movementSpeed);
        vel = transform.TransformDirection(vel);
        transform.position += vel * Time.deltaTime;

        velocity = Vector3.zero;
    }

    #endregion

    #region Stealth Checks

    private void isWalkingCheck()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            isWalking = true;
        }

        else
        {
            isWalking = false;
        }
    }

    private void SneakCheck()
    {
        if (Input.GetButton("sneak"))
        {
            movementSpeed = trueMove * sneakSpeed;
            isSneaking = true;
        }
        else if (!isWalking)
        {
            isSneaking = true;
        }
        else
        {
            isSneaking = false;
            movementSpeed = trueMove;
        }

        if (isSneaking)
        {
            if (grounded)
            {
                if (Input.GetAxis("Vertical") != 0)
                {
                    isSneakWalking = true;
                }
                else
                {
                    isSneakWalking = false;
                }
            }
            else
            {
                isSneakWalking = false;
            }

        }
        else { isSneakWalking = false; }

    }

    #endregion

    #region Gravity/Grounding
    //Gravity Private Variables
    private bool grounded;
    //private float currentGravity = 0;


    private Vector3 liftPoint = new Vector3(0, 0, 0);
    private RaycastHit groundHit;
    private Vector3 groundCheckPoint = new Vector3(0, -0.87f, 0);


    private void GroundChecking()
    {
        Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
        RaycastHit tempHit = new RaycastHit();

        if (Physics.SphereCast(ray, 0.17f, out tempHit, 20, discludePlayer))
        {
            GroundConfirm(tempHit);
        }
        else
        {
            grounded = false;
        }
    }

    private void GroundConfirm(RaycastHit tempHit)
    {

        Collider[] col = new Collider[3];
        int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(groundCheckPoint), 0.57f, col, discludePlayer);

        grounded = false;

        for (int i = 0; i < num; i++)
        {

            if (col[i].transform == tempHit.transform)
            {
                groundHit = tempHit;
                grounded = true;

                if (inputJump == false)
                {
                    if (!smooth)
                    {
                        transform.position = new Vector3(transform.position.x, (groundHit.point.y + playerHeight / 2), transform.position.z);
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, (groundHit.point.y + playerHeight / 2), transform.position.z), smoothSpeed * Time.deltaTime);
                    }
                }

                break;

            }
        }

        if (num <= 1 && tempHit.distance <= 3.1f && inputJump == false)
        {

            if (col[0] != null)
            {
                Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 3.1f, discludePlayer))
                {
                    if (hit.transform != col[0].transform)
                    {
                        grounded = false;
                        return;
                    }
                }
            }
        }
    }
    #endregion

    #region Jumping

    private float jumpHeight = 0;
    private bool inputJump = false;

    private float fallMultiplier = -1;

    private void Jump()
    {
        bool canJump = false;

        canJump = !Physics.Raycast(new Ray(transform.position, Vector3.up), playerHeight, discludePlayer);

        if (grounded && jumpHeight > 0.2f || jumpHeight <= 0.2f && grounded)
        {
            jumpHeight = 0;
            inputJump = false;
            fallMultiplier = -1;
        }

        if (grounded && canJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                inputJump = true;
                transform.position += Vector3.up * 0.6f;
                jumpHeight += jumpForce;
                isSneaking = false;
            }
        }
        else
        {
            if (!grounded)
            {
                jumpHeight -= (jumpHeight * jumpDecrease * Time.deltaTime) + fallMultiplier * Time.deltaTime;
                fallMultiplier += incrementJumpFallSpeed;
            }
        }

        velocity.y += jumpHeight;
    }

    #endregion

    #region Pickup Items

    private void PickUp()
    {
        if (Input.GetButtonDown("grab"))
        {
            /*
        {
            if (!isHolding)
            {
                CheckObject();

                if (isHolding)
                {

                    AnalyzeObject();

                    SetTargetIK();

                    StartCoroutine(distanceChecker());


                }
                else return;
            }
            else if (isHolding)
            {
                Drop();
            }
        }

        if (isHolding && isInCover)
        {
            Drop(); */
            grabAction.ChangeState(new PickUp(this.gameObject, this.GetComponent<IKRef>(), this.GetComponent<InfoBuffer>(), isGrabbable, grabRange, playerHeight, 2.0f /*animation time*/ ));
            Debug.Log("It's running pickup");
        }

}

    #endregion // to do
    //beta

    #region Boost Functions

    private void isBoostingCheck()
    {
        /*if boost button has been pressed in the last MaxBoostTime seconds & player is able to boost, isBoosting = true
         * 
         */
    }

    private void BoostFunctions()
    {
        /*Gravity works unless the player leaves the ground.
         * PlayerSpeed = PlayerSpeed*BoostSpeed
         * if isBoosting = true and MaxBoostTime < timesincelastboostpress
         * run fall check. if distance > fallthreshold, deathanimation.
         */
    }

    #endregion
    // todo
    #region Disable Control

    private void disableControlCheck()
    {
        if (isBoosting || isInCover) { disableControl = true; }
        else { disableControl = false; }
    }

    #endregion



}
