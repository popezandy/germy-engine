using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationIK
{
    private GameObject thisObject;

    private Transform LE;
    private Transform RE;

    private Transform leftElbowEffector;
    private Transform rightElbowEffector;

    private float animTime;

    private Vector3 LEVector;
    private Vector3 REVector;
    private Vector3 leftElbowVector;
    private Vector3 rightElbowVector;

    // THIS SCRIPT NEEDS TO BE ABLE TO BE FED INFORMATION ON THE ENTIRE SKELETON, WHICH MEANS THAT EVERY SCRIPT THAT CALLS FOR ANIMATION NEEDS ACCESS TO THE ENTIRE SKELETON. WHAT THE FUCK.


    public AnimationIK(GameObject thisObject, IKRef ikRef, Vector3 LEVector, Vector3 REVector, Vector3 leftElbowVector, Vector3 rightElbowVector, float AnimTime)
    {
        Debug.Log("this shit is running");
        this.LE = ikRef.LE;
        this.RE = ikRef.RE;

        this.leftElbowEffector = ikRef.LeftElbowEffector;
        this.rightElbowEffector = ikRef.RightElbowEffector;

        this.LEVector = LEVector;
        this.REVector = REVector;
        this.leftElbowVector = leftElbowVector;
        this.rightElbowVector = rightElbowVector;

        this.animTime = AnimTime;

        this.thisObject = thisObject;
    }

    public void Run()
    {
        thisObject.GetComponent<IKRef>().LE.position = Vector3.Lerp(LE.position, LEVector, animTime * Time.deltaTime);
        thisObject.GetComponent<IKRef>().RE.position = Vector3.Lerp(RE.position, REVector, animTime * Time.deltaTime);
        thisObject.GetComponent<IKRef>().LeftElbowEffector.position = Vector3.Lerp(leftElbowEffector.position, leftElbowVector, animTime * Time.deltaTime);
        thisObject.GetComponent<IKRef>().RightElbowEffector.position = Vector3.Lerp(rightElbowEffector.position, rightElbowVector, animTime * Time.deltaTime);
        
        //transform.position = Vector3.Lerp(transform.position, des, speed * Time.deltaTime);
    }
}
