using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBuffer : MonoBehaviour
{

    public string PerimeterState;
    public Transform PerimeterLocation;
    public Collider PrioretyTarget;

    public bool playerInPatrolArea;

    public bool isHeld;
    public bool isHolding;
    public bool isHeldOverhead;

    public bool disableControl;
    public bool boostEnabled;
    public bool isBoosting;

    public bool targetIsFollowable;

    public bool isNoisy;
    public bool isSmelly;
    public bool inGas;

    public bool toggleDebug;

    public GameObject grabObject;

    //CoverSystem

    public bool isInCover;
    public bool isGrounded;

    public Vector3 lastplaceGrounded;
}
