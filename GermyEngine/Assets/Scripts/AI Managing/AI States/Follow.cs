using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Follow : IState
{
    private GameObject OwnerGameObject;
    private Transform TargetTransform;
    private float FollowRadius;
    private NavMeshAgent navMeshAgent;



    public Follow(GameObject ownerGameObject, Transform targetTransform, float followRadius, NavMeshAgent navMeshAgent)
    {
        this.OwnerGameObject = ownerGameObject;
        this.TargetTransform = targetTransform;
        this.FollowRadius = followRadius;
        this.navMeshAgent = navMeshAgent;
    }

    public void Enter()
    {
        if (TargetTransform != null)
        {
            navMeshAgent.SetDestination(TargetTransform.position);
        }
        if (Vector3.Distance(TargetTransform.position, OwnerGameObject.transform.position) > FollowRadius)
        {
            navMeshAgent.SetDestination(OwnerGameObject.transform.position);

            OwnerGameObject.GetComponent<InfoBuffer>().targetIsFollowable = false;
            OwnerGameObject.GetComponent<InfoBuffer>().PrioretyTarget = null;
        }
}

    public void Execute()
    {
        

        if (TargetTransform != null)
        {
            navMeshAgent.SetDestination(TargetTransform.position);
        }
        if (Vector3.Distance(TargetTransform.position, OwnerGameObject.transform.position) > FollowRadius)
        {
            navMeshAgent.SetDestination(OwnerGameObject.transform.position);

            OwnerGameObject.GetComponent<InfoBuffer>().targetIsFollowable = false;
            OwnerGameObject.GetComponent<InfoBuffer>().PrioretyTarget = null;
        }
    }

    public void Exit()
    {

    }
}

