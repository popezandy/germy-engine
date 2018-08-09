using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRB : MonoBehaviour
{
    #region Class Description

    /*Player Controller script that uses a Rigid Body for physics calculations. 
     Requires InfoBuffer, PlayerSettings,BoosterShoes,CoverSystem*/

    #endregion

    #region ToDo

    /* Desired Script Traits-
     * Currently implementing inventory system
    */

    #endregion

    #region Variables


    [Header("Movement Options")]

    public float trueMove;
    public float turnSpeed;
    public bool smooth;
    public float smoothSpeed;


    [Header("Jump Options")]
    public float jumpForce;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;


    bool jumpRequest;
    private bool inputJump = false;

    public float maxCount = 0.5f;
    float counter = 0f;


    [Header("Layer Masks")]
    public LayerMask discludePlayer;
    public LayerMask isGrabbable;

    [Header("References")]

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
    public float maxGroundBufferTime = 2;

    private bool isSneakWalking = false;
    private Rigidbody rigidBody = new Rigidbody();


    private float playerHeight;
    private float grabRange;

    public GameObject leftObject;
    public GameObject rightObject;
    public GameObject freeHand;



    [Header("Inventory")]
    public Inventory inventory;
    public InfoBuffer infoBuffer;

    #endregion

    #region Callbacks

    private void Start()
    {
        infoBuffer = this.GetComponent<InfoBuffer>();
        colorBuffer = GetComponent<Renderer>().material.color;
        rigidBody = this.GetComponent<Rigidbody>();
        this.GetComponent<InfoBuffer>().lastplaceGrounded = this.transform.position;
        breathOvertime += breathTime;
    }

    private void Update()
    {
        GulpCheck();
        if (!disableControl)
        {
            detectGroundSwitch();
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
            Breathing();
        }
        if (isBoosting)
        {
            BoostMode();
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

        setDynamicReferences();
        disableControlCheck();
        CountDown();
        HandLogic();
        CheckGas();
        
    }


    private void BoostMode()
    {
        detectGroundSwitch();
        isWalkingCheck();
        SimpleMove();
        FinalMove();
        GroundChecking();
    }


    private void FixedUpdate()
    {
        if (jumpRequest)
        {
            rigidBody.AddForce(this.transform.up * jumpForce, ForceMode.Impulse);
            jumpRequest = false;
        }

        if (rigidBody.velocity.y < 0 && !grounded)
        {
            rigidBody.velocity += transform.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime * -1;
        }
        else if (rigidBody.velocity.y > 0 && Input.GetKey(KeyCode.Space))
        {
            rigidBody.velocity += transform.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime * -1 * counter;
        }
    }

    private void setDynamicReferences()
    {
        infoBuffer.isNoisy = !isSneaking;
        infoBuffer.isInCover = isInCover;
        infoBuffer.isGrounded = grounded;
        this.GetComponent<PlayerSettings>().isGrabbable = isGrabbable;
        this.GetComponent<PlayerSettings>().grabRange = grabRange;
        this.GetComponent<PlayerSettings>().playerHeight = playerHeight;
        isBoosting = this.GetComponent<InfoBuffer>().isBoosting;
        infoBuffer = this.GetComponent<InfoBuffer>();
    }

    #endregion

    #region Movement

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

    #region Grounding

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

    private void Jump()
    {
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !this.GetComponent<InfoBuffer>().isHolding)
            {
                jumpRequest = true;
                inputJump = true;

                isSneaking = false;
                counter = maxCount;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && this.GetComponent<InfoBuffer>().isHolding)
            {
                jumpRequest = true;
                inputJump = true;

                isSneaking = false;
                counter = maxCount * .75f;
            }
        }
    }

    private void CountDown()
    {
        if (counter > 0)
        {
            counter -= Time.deltaTime;
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
            else if (isHolding && grounded)
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
    #region Disable Control

    private void disableControlCheck()
    {
        if (isBoosting || this.GetComponent<InfoBuffer>().isInCover) { disableControl = true; }
        else { disableControl = false; }
    }

    #endregion

    #region Record Safe Ground
    private float time;

    private void detectGroundSwitch()
    {
        time += Time.deltaTime;
        if (time > maxGroundBufferTime)
        {
            if (this.GetComponent<InfoBuffer>().isGrounded == true)
            {
                this.GetComponent<InfoBuffer>().lastplaceGrounded = this.transform.position;
            }
            time = 0;
        }
    }
    #endregion

    #region Inventory

    private void OnCollisionEnter(Collision hit)
    {
        IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
        if (item != null)
        {
            inventory.AddItem(item);
        }
        Debug.Log(inventory);
    }
    /*This currently picks up any item with an item script on it automatically. In the future,
     * an animation will play and then if the object collides with the hands, add it to the inventory.
     *Default pickup hand should always be the left hand, if left hand is filled right hand switch
     *
     */
    #endregion
    //beta
    #region Breathing
    float timePassed = 0f;
    public float breathTime = 5f;
    public float breathOvertime = 2f;
    public float gulpForAirtTime = 2f;
    public bool breathing = true;
    private bool gulpingForAir;

    Color colorBuffer;
    private void Breathing()
    {
        
        if ((Input.GetButton("leftuse") && freeHand == leftObject) || (Input.GetButton("rightuse") && freeHand == rightObject))
        {
            timePassed += Time.deltaTime;
            Debug.Log("Holding breath");
            GetComponent<Renderer>().material.color = Color.Lerp(colorBuffer, Color.blue, (timePassed/breathTime));
            breathing = false;
        }
        else
        {
            timePassed = 0;
            GetComponent<Renderer>().material.color = colorBuffer;
            breathing = true;
        }

        if (timePassed > breathTime)
        {
            float t = Mathf.PingPong(Time.time * 3f, 1.0f);
            GetComponent<Renderer>().material.color = Color.Lerp(Color.blue,Color.black,t); }
        if (timePassed > breathOvertime)
        {

            GetComponent<Renderer>().material.color = Color.black;
            breathing = true;
            Debug.Log("Gulp for air");
            disableControl = true;
            gulpingForAir = true;
            timePassed = 0;
        }
    }
    float newTime = 0;
    private void GulpCheck()
    {
        if (gulpingForAir)
        {
            disableControl = true;
            
            newTime += Time.deltaTime;
            if (newTime < gulpForAirtTime)
            {
                breathing = true;
                timePassed = 0;
            }
            else
            {
                breathing = false;
                gulpingForAir = false;
                newTime = 0;
                disableControl = false;
            }
        }
    }
    /*This should be used in conjunction with the items held mechanism instead of within the
     * player script. Any hand that is free should run the breathing command, with the default being
     *the right hand.
     *
     */

    private void CheckGas()
    {
        if (breathing && infoBuffer.inGas)
        {
            Debug.Log("You're in Gas and Breathing. You are now dead");
        }
    }

    #endregion
    //beta
    #region HandLogic

    private void HandLogic()
    {
        SetFreeHand();
        PlaceFreeHand();
    }

    private void SetFreeHand()
    {
        if (leftObject == null)
        {
            if (rightObject == freeHand)
            { rightObject = null; }
            freeHand.SetActive(true); leftObject = freeHand;
        }

        else
        {
            if (rightObject == null && leftObject != freeHand)
            { freeHand.SetActive(true); rightObject = freeHand; }
        }
        if(rightObject != freeHand && leftObject != freeHand)
        { freeHand.SetActive(false); }
    }

    private void PlaceFreeHand()
    {
        if (freeHand.activeSelf)
        {
            if (leftObject == freeHand)
            { freeHand.transform.position = leftHand.position; freeHand.transform.SetParent(leftHand); }

            if (rightObject == freeHand)
            { freeHand.transform.position = rightHand.position; freeHand.transform.SetParent(rightHand); }

            if (rightObject != freeHand && leftObject!= freeHand)
            { freeHand.SetActive(false); Debug.Log("this should never have happened"); }
        }
    }

    private void HandUse()
    {
        if (leftObject != null && leftObject != freeHand)
        {
            //item.leftbool = true
            //item.position = leftHand.position
            //run item

        }
        if (rightObject != null && rightObject != freeHand)
        {
            //item.leftbool = false
            //item.position = rightHand.position
            //run item 
        }
    }
    #endregion

    //Pickup Items and Breathing need to be removed. -269 lines of code, putting us at around 550.
    //Need to reduce the amount of variables in this class. 80 is too many.
    //With stealth removed we'd cut out another 70 lines of code.
}
