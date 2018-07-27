using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : IActionSystem{

    private bool isHolding;
    private bool isInCover;
    private float playerHeight;
    private float grabRange;
    public LayerMask grabbableLayer;
    private bool isHeldOverhead;
    private Vector3 grabTargetLeft;
    private Vector3 grabTargetRight;

    private Vector3 grabTargetLeftAnim;
    private Vector3 grabTargetRightAnim;

    private GameObject grabObject;

    private float animTime;

    private InfoBuffer infoBuffer;
    private IKRef iKRef;

    private GameObject ownerObject;
    private Transform ownerTransform;

    AnimationIK animation;
    private bool animationRunning;

    //IK ANIMATION TARGETING REQUIRES EXTRACTION OF IK MOVEMENT
    #region Constructor
    public PickUp(GameObject OwnerObject, IKRef IKRef, InfoBuffer InfoBuffer, LayerMask GrabbableLayer, float grabRange, float playerHeight, float animTime)
    {
        this.ownerObject = OwnerObject;
        this.ownerTransform = OwnerObject.transform;
        this.iKRef = IKRef;
        this.infoBuffer = InfoBuffer;
        this.grabRange = grabRange;
        this.playerHeight = playerHeight;
        this.animTime = animTime;
        this.isHolding = InfoBuffer.isHolding;
        this.isHeldOverhead = InfoBuffer.isHeldOverhead;
        this.grabObject = infoBuffer.grabObject;
        //It would be more swift to put player settings into their own monobehavior so that we could call a transform, and IK ref, an InfoBuffer, and a PlayerSettings, rather than 6 things calling 3 would be better.
    }
    #endregion


    #region Logic
    public void Run()
    {
        Debug.Log("Run successful");
        if (!isHolding)
        {
            Debug.Log("Run happened, isHolding is false");
            CheckObject();

            if (isHolding)
            {
                Debug.Log("is HOlding is now True");
                AnalyzeObject();
                SetIKTarget();
            }
            else
            {
                ownerObject.GetComponent<PlayerAction>().grabAction.ClearState();
            }
        }
        else if (isHolding)
        {
            PickUpAnimation();
            Drop();
        }

        if (isHolding && isInCover)
        {
            Drop();
        }

    }
    #endregion

    #region Object Analysis
    private void CheckObject()
    {
        float grabFloat = 1f - (playerHeight / 2);
        Vector3 grabRay = ownerTransform.TransformPoint(0, grabFloat, 0);
        Ray ray = new Ray(grabRay, ownerTransform.forward);
        RaycastHit hit = new RaycastHit();
        Debug.Log("grabRay" + grabRay + ownerTransform.forward + grabRange + grabbableLayer);
        Debug.DrawRay(grabRay, ownerTransform.forward, Color.black, 5f);

        if (Physics.Raycast(ray, out hit, grabRange))
            //FOR NO REASON THIS DOES NOT WORK
        {
            if (hit.transform.GetComponent<ObjectGrabbable>() != null)
            {
                infoBuffer.grabObject = hit.transform.gameObject;

                grabObject.GetComponent<InfoBuffer>().isHeld = true;

                ownerObject.GetComponent<InfoBuffer>().isHolding = true;
                isHolding = true;

                Debug.Log("Object checked alright" + hit.transform.name);
            }
        }
        else
        {
            Debug.Log("For some reason, I didn't hit anything.");
            return;
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
            grabObject.transform.LookAt(new Vector3(ownerTransform.position.x, grabObject.transform.position.y, ownerTransform.position.z));
            grabObject.transform.Rotate(0, -90, 0);

            Debug.Log("Pick Up Two Handed Object");
            grabTargetLeft = grabObject.GetComponent<ObjectGrabbable>().leftHandle.position;
            grabTargetRight = grabObject.GetComponent<ObjectGrabbable>().rightHandle.position;

            grabTargetLeftAnim = new Vector3(grabTargetLeft.x, grabTargetLeft.y + playerHeight / 2, grabTargetLeft.z);
            grabTargetRightAnim = new Vector3(grabTargetRight.x, grabTargetRight.y + playerHeight / 2, grabTargetRight.z);
        }


        if (casenumber == 2)
        {

        }


        if (casenumber == 3)
        {
            Ray ray = new Ray(ownerTransform.position, grabObject.transform.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, grabbableLayer))
            {
                grabTargetLeft = new Vector3((hit.transform.position.x - 1), hit.point.y, hit.transform.position.z);
                grabTargetRight = new Vector3((grabTargetLeft.x + 1.8f), hit.point.y, grabTargetLeft.z);
            }
        }


        if (casenumber == 4)
        {

        }


    }
    #endregion

    #region Drop
    private void Drop()
    {
        if (Input.GetButtonDown("grab") /*&& grabObject != null*/)
        {
            float bodyFromGround = 0.5f - (playerHeight / 2);
            float setInFrontDist = 1.5f;
            Vector3 dropPoint = ownerTransform.TransformPoint(0, bodyFromGround, setInFrontDist);

            grabObject.transform.SetParent(null);
            grabObject.transform.position = dropPoint;
            grabObject.GetComponent<InfoBuffer>().isHeld = false;
            grabObject = null;

            isHolding = false;
            infoBuffer.isHolding = false;


            isHeldOverhead = false;
            ownerObject.GetComponent<PlayerAction>().grabAction.ClearState();
            Debug.Log("drop happened");
        }
    }
    #endregion

    private void SetIKTarget()
    {
        ownerObject.GetComponent<IKRef>().LE.position = grabTargetLeft;
        ownerObject.GetComponent<IKRef>().RE.position = grabTargetRight;


        Debug.Log("IK TARGETS SET");
    }

    private void PickUpAnimation()
    {
            grabObject.transform.position = iKRef.leftHand.position;
            animation = new AnimationIK(ownerObject, iKRef, grabTargetLeftAnim, grabTargetRightAnim, iKRef.LeftElbowEffector.position, iKRef.RightElbowEffector.position, animTime);
            
            Debug.Log("Animation Started");
            animationRunning = true;

      if (animationRunning)
        {
            animation.Run();
        }
            
            //if tag = curecalf, disable gravity, pickup animation, move calf above head, child to player, isHolding = true
        //if tag = pickup, pocket animation, destroy item, add to inventory
        //else ignore
        // disable input while picking up animation is playing.
    }
}
