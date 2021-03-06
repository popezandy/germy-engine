﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmIK : MonoBehaviour {


    #region Variables

    [Header("Options")]
    [Range(0,2)]
    public float elbowPower;
    public float refFollowSpeed =  0.2f;

    [Header("Joint References")]
    public Transform endEffector;
    public Transform elbowRef;
    public Transform centroidRef;
    public Transform upperRef;
    public Transform lowerRef;
    public Transform handRef;

    [Header("Spherical Constraints")]
    public SphericalConstraint upperConstraint;

    [Header("Model References")]
    public Transform upperModel;
    public Transform lowerModel;

    [Header("Model Offset")]
    public Vector3 upperOffset;
    public Vector3 lowerOffset;

    [HideInInspector]
    public Vector3 centroidPos;

    private Vector3 tempCentroid;
    private Vector3 tempUpper;
    private Vector3 tempLower;
    private Vector3 tempHand;

    private float centroidToUpperDist;
    private float upperToLowerDist;
    private float lowerToHandDist;

    #endregion

    #region Visualization

    private void OnDrawGizmos()
    {
        if (handRef!= null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(upperRef.position, centroidRef.position);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(lowerRef.position, upperRef.position);
            Gizmos.DrawLine(handRef.position, lowerRef.position);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(endEffector.position, handRef.position);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(elbowRef.position, lowerRef.position);
        }
    }

    #endregion

    #region Setup

    private void Awake()
    {
        SetModelPosition();
        SetupVariables();
    }

    private void SetupVariables()
    {
        centroidToUpperDist = Vector3.Distance(upperRef.position, centroidRef.position);
        upperToLowerDist = Vector3.Distance(lowerRef.position, upperRef.position);
        lowerToHandDist = Vector3.Distance(handRef.position, lowerRef.position);

        centroidPos = centroidRef.position;
        tempCentroid = centroidPos;
    }

    private void SetModelPosition()
    {
        upperRef.position = upperModel.position;
        lowerRef.position = lowerModel.position;
        tempHand = (tempLower + (tempHand - tempLower).normalized * lowerToHandDist);
    }

    #endregion

    #region FABRIK Algorithm

    /*
    private void LateUpdate()
    {
        SolveBackward(endEffector.position);
        SolveForward(centroidPos);
        SetTempPos();
    }
    */

    public Vector3 SolveBackward(Vector3 endEffector)
    {
        tempCentroid = centroidPos;

        tempHand = endEffector;

        Vector3 handToLower = (lowerRef.position - tempHand).normalized * lowerToHandDist;
        tempLower = tempHand + handToLower + (elbowRef.position - lowerRef.position).normalized * elbowPower;

        Vector3 lowerToUpper = (upperRef.position - tempLower).normalized * upperToLowerDist;
        tempUpper = tempLower + lowerToUpper;

        Vector3 upperToCentroid = (centroidRef.position - tempUpper).normalized * centroidToUpperDist;
        tempCentroid = tempUpper + upperToCentroid;

        return tempCentroid;
    }

    public void SolveForward(Vector3 rootPoint)
    {
        tempCentroid = rootPoint;

        Vector3 centroidToUpper = (tempUpper - tempCentroid).normalized * centroidToUpperDist;
        tempUpper = tempCentroid + centroidToUpper;

        Vector3 upperToLower = (tempLower - tempUpper).normalized * upperToLowerDist;
        tempLower = tempUpper + upperToLower;

        Vector3 lowerToHand = (tempHand - tempLower).normalized * lowerToHandDist;
        tempHand = tempLower + lowerToHand;

    }

    public void SetTempPos()
    {
        if (upperConstraint!=null)
        {

            tempUpper = upperConstraint.clampIfNeeded(tempUpper);
            tempLower = (tempUpper + (tempLower - tempUpper).normalized * upperToLowerDist);
            tempHand = (tempLower + (tempHand - tempLower).normalized * lowerToHandDist);

        }

        Vector3 cross = Vector3.Cross((elbowRef.position - tempLower).normalized, (tempUpper - tempLower).normalized).normalized;

        

        upperRef.position = Vector3.Lerp(upperRef.position,tempUpper,refFollowSpeed);
        lowerRef.position = Vector3.Lerp(lowerRef.position,tempLower,refFollowSpeed);
        handRef.position = Vector3.Lerp(handRef.position,tempHand,refFollowSpeed);

        upperModel.LookAt(lowerRef, cross);
        upperModel.Rotate(upperOffset);

        lowerModel.LookAt(handRef, cross);
        lowerModel.Rotate(lowerOffset);

        centroidPos = centroidRef.position;

    }
    #endregion


}
