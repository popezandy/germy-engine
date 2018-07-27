using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRB : MonoBehaviour
{
    #region Class Description

    /*Lorem ipsum parmitsum canipsum waginger the blinger faninginer shplanininer*/

    #endregion

    #region ToDo

    /* Desired Script Traits-
     * 1. Boosting with 3 second no gravity time, a check for ground below, fall animation if no ground
     * 2. #Walking#, Boosting!!, #Sneaking#
     * 3. #Picking up and putting down goal item#
     * 4. Self turning with arrow keys*
     * 5. Camera turning with Q and E*
     * 6. Wall hugging!!!###
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

    public Transform leftShoulder;
    public Transform rightShoulder;

    public Transform leftHand;
    public Transform rightHand;

    public Transform LE;
    public Transform RE;

    [HideInInspector]
    public bool isSneaking, isSmelly, hasID, isInCover, disableControl, disableInput;
    //detection booleans

    //grabbing variables
    private bool isHolding;
    private bool isHeldOverhead;

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
    private Rigidbody rigidBody = new Rigidbody();

    #endregion

    #region Callbacks

    private void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!disableControl)
        {

            isBoostingCheck();
            isWalkingCheck();
            SneakCheck();
            //Gravity();
            SimpleMove();
            Jump();
            FinalMove();
            GroundChecking();
            //CollisionCheck();
            PickUp();
            setDynamicReferences();
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

        if (isHolding && isInCover)
        {
            Drop();
        }

        disableControlCheck();
    }

    #endregion

    private void setDynamicReferences()
    {
        this.GetComponent<InfoBuffer>().isNoisy = !isSneaking;
        this.GetComponent<InfoBuffer>().isInCover = isInCover;
        this.GetComponent<PlayerSettings>().isGrabbable = isGrabbable;
        this.GetComponent<PlayerSettings>().grabRange = grabRange;
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
    
    private bool grounded;
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

    #region Collision

    private void CollisionCheck()
    {
        Collider[] overlaps = new Collider[4];
        int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(sphereCol.center), sphereCol.radius, overlaps, discludePlayer, QueryTriggerInteraction.UseGlobal);

        for (int i = 0; i < num; i++)
        {
            Transform t = overlaps[i].transform;
            Vector3 dir;
            float dist;

            if (Physics.ComputePenetration(sphereCol, transform.position, transform.rotation, overlaps[i], t.position, t.rotation, out dir, out dist))
            {
                Vector3 penetrationVector = dir * dist;
                Vector3 velocityProjected = Vector3.Project(velocity, -dir);
                transform.position = transform.position + penetrationVector;
                colVelocity -= velocityProjected;
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
                rigidBody.AddForce(this.transform.up * jumpForce, ForceMode.Impulse);
                isSneaking = false;
            }
        }
        else
        {
            
        }

        
    }

    #endregion

    #region Pickup Items

    private void PickUp()
    {
        if (Input.GetButtonDown("grab"))
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
            Drop();
        }

    }

    private void CheckObject()
    {
        float grabFloat = 1f - (playerHeight / 2);
        Vector3 grabRay = transform.TransformPoint(0, grabFloat, 0);
        Ray ray = new Ray(grabRay, transform.forward);
        RaycastHit hit;
        Debug.Log("check vector is at height " + grabFloat);

        if (Physics.Raycast(ray, out hit, grabRange, isGrabbable))
        {

            grabObject = hit.transform.gameObject;

            grabObject.GetComponent<InfoBuffer>().isHeld = true;

            isHolding = true;
            this.GetComponent<InfoBuffer>().isHolding = true;

        }

    }

    private void AnalyzeObject()
    {
        if (grabObject != null)
        {
            if (grabObject.GetComponent<ObjectGrabbable>())
            {


                if (grabObject.GetComponent<ObjectGrabbable>().leftHandle != null && grabObject.GetComponent<ObjectGrabbable>().rightHandle != null)
                {
                    if (grabObject.GetComponent<ObjectGrabbable>().isOverhead)
                    { GrabCaseChooser(1); }
                    else
                    { GrabCaseChooser(2); }
                }
                else if (grabObject.GetComponent<ObjectGrabbable>().leftHandle == null && grabObject.GetComponent<ObjectGrabbable>().rightHandle == null)
                {
                    if (grabObject.GetComponent<ObjectGrabbable>().isOverhead)
                    { GrabCaseChooser(3); }
                    else
                    { Debug.Log("Handleless objects must display true for /isoverhead/ bool. For one handed objects with no target handle, just drag the object's transform into the default (left) transform slot on its ObjectGrabbable script."); }
                }
                else if (grabObject.GetComponent<ObjectGrabbable>().leftHandle != null && grabObject.GetComponent<ObjectGrabbable>().rightHandle == null)
                {
                    if (!grabObject.GetComponent<ObjectGrabbable>().isOverhead)
                    { GrabCaseChooser(4); }
                    else
                    { Debug.Log("A one handed object cannot be overhead at this time. Change the /isoverhead/ bool."); }
                }
                else
                {
                    { Debug.Log("Something went wrong. Make sure the grabbable object's target transform is in the default (left) transform slot on its ObjectGrabbable script."); }
                }


            }
            else { Debug.Log("This object has no ObjectGrabbable script."); }
        }
        else { Debug.Log("Somehow the grab object was not assigned."); }
    }

    private void GrabCaseChooser(int casenumber)
    {
        /*
         Case Breakdown-
         1. Overhead object with both handles.
         2. Twohanded item which is not held overhead.
         3. Overhead object with no handles.
         4. One handed object with our without handles.
         */

        if (casenumber == 1)
        {
            grabObject.transform.LookAt(new Vector3(this.transform.position.x, grabObject.transform.position.y, this.transform.position.z));
            grabObject.transform.Rotate(0, -90, 0);

            print("Pick Up Two Handed Object");
            grabTargetLeft = grabObject.GetComponent<ObjectGrabbable>().leftHandle.position;
            grabTargetRight = grabObject.GetComponent<ObjectGrabbable>().rightHandle.position;
        }


        if (casenumber == 2)
        {

        }


        if (casenumber == 3)
        {
            Ray ray = new Ray(transform.position, grabObject.transform.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, isGrabbable))
            {
                grabTargetLeft = new Vector3((hit.transform.position.x - 1), hit.point.y, hit.transform.position.z);
                grabTargetRight = new Vector3((grabTargetLeft.x + 1.8f), hit.point.y, grabTargetLeft.z);
            }
        }


        if (casenumber == 4)
        {

        }


    }

    private void SetTargetIK()
    {
        LE.position = grabTargetLeft;
        RE.position = grabTargetRight;
    }

    /**/
    IEnumerator distanceChecker()
    {
        yield return new WaitUntil(() => (grabObject.GetComponent<ObjectGrabbable>().leftHandle.position - leftHand.position).magnitude >= .05f);

        grabObject.transform.SetParent(LE);
        //I believe there is an issue with the 3d Object's handles.

        PickUpAnimation();
    }

    private void PickUpAnimation()
    {
        if (isHolding && !isHeldOverhead)
        {
            LE.position = new Vector3(leftShoulder.position.x, transform.position.y + 2.5f, leftShoulder.position.z);
            RE.position = grabObject.GetComponent<ObjectGrabbable>().rightHandle.position;
            isHeldOverhead = true;

        }
        else return;

        //if tag = curecalf, disable gravity, pickup animation, move calf above head, child to player, isHolding = true
        //if tag = pickup, pocket animation, destroy item, add to inventory
        //else ignore
        // disable input while picking up animation is playing.
    }

    private void Drop()
    {
        float bodyFromGround = 0.5f - (playerHeight / 2);
        float setInFrontDist = 1.5f;
        Vector3 dropPoint = transform.TransformPoint(0, bodyFromGround, setInFrontDist);

        grabObject.transform.SetParent(null);
        grabObject.transform.position = dropPoint;
        grabObject.GetComponent<InfoBuffer>().isHeld = false;
        grabObject = null;

        isHolding = false;
        this.GetComponent<InfoBuffer>().isHolding = false;


        isHeldOverhead = false;

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

    #region Cover System
    /* Cover System will be utilized in another script. The basic form
     * of the cover system will look like the following:
     * If the player is holding sneak, and they approach a wall with
     * reasonable intent, they will snap to that wall, drop the calf
     * if they are carrying it, and turn to face parallel but away
     * from the wall in question.
     * 
     * It will do this by first sending out a raycast, checking whether
     * there is a box collider in front of the player. If there is,
     * check tags, and then check the angle of collision:
     * 
     * Vector3.Angle(normal,rayName)
     * 
     * If the raycast is successful through tag check, the player is rotated
     * according to the following formula-
     * 
     * ((90 + |x|)*(|x|/x)) degrees.
     * 
     * The player will then snap to the point of intersection, plus
     * a buffer unit in the forward direction.
     * 
     * As long as sneak is held, the player can only move left or
     * right along the wall. A checker object will come out of the
     * wall at a buffer distance to the direction the player
     * is trying to walk, will raycast in 3 spots toward the wall
     * and if any detect a collision with a box collider of the
     * correct type and at the correct angle and distance, the checker
     * will set it's current position as the next position for the
     * player to be able to move toward, and make another check. If
     * the check fails, new temporary position shouldn't update, allowing
     * the player to only walk to the last successful check. 
     * 
     * The raycast should also still rotate the player based on the 
     * relative rotation of the wall.
    */
    #endregion
    //todo

    #region Disable Control

    private void disableControlCheck()
    {
        if (isBoosting || isInCover) { disableControl = true; }
        else { disableControl = false; }
    }

    #endregion


}
