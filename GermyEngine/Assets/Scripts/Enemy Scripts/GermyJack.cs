using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]


[RequireComponent(typeof(InfoBuffer))]
[RequireComponent(typeof(EnemyDetectionComponents))]

public class GermyJack : MonoBehaviour {

    #region Static Variables

    private StateMachine detectMachine = new StateMachine();
    private StateMachine locationMachine = new StateMachine();

    

    [SerializeField]
    private string perimeterState;
    private Transform perimeterLocation;
    private List<Collider> foundObjects = new List<Collider>();

    private NavMeshAgent navMeshAgent;

    private Collider prioretyTarget;

    #endregion

    #region Dynamic Variables

    private bool isHeld;
    private bool followableCheck = true;

    #endregion




    #region CallBacks

    private void Start()
    {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();

        this.detectMachine.ChangeState(new SearchFor(this.GetComponent<EnemyDetectionComponents>().SearchLayer, this.gameObject,
            this.GetComponent<EnemyDetectionComponents>().detectionRadius, this.GetComponent<EnemyDetectionComponents>().SearchTag,
                this.SomethingFound));
    }

    private void Update()
    {

        SetDynamicVariables();

        if (!isHeld)
        {
            this.navMeshAgent.enabled = true;


            this.detectMachine.ExecuteStateUpdate();
            this.locationMachine.ExecuteStateUpdate();

            PossibleStateDecider();


        }

        if (isHeld)
        {
            this.navMeshAgent.enabled = false;
        }

    }

    #endregion

    #region Behavior Processing

    public void SomethingFound(SearchResults searchResults)
    {
        var foundTargets = searchResults.AllHitObjectsInSearchRadius;

        for (int i = 0; i < foundTargets.Length; i++)
        {
            foundObjects.Add(foundTargets[i]);
            Debug.Log("Object " + i + foundTargets[i].name);
        }
    }

    private void SetDynamicVariables()
    {
        isHeld = this.GetComponent<InfoBuffer>().isHeld;
        followableCheck = this.GetComponent<InfoBuffer>().targetIsFollowable;
        perimeterState = this.GetComponent<InfoBuffer>().PerimeterState;
        perimeterLocation = this.GetComponent<InfoBuffer>().PerimeterLocation;
        prioretyTarget = this.GetComponent<InfoBuffer>().PrioretyTarget;

    }

    private void PossibleStateDecider()
    {
        if (perimeterState == "patrol")
        {
            PatrolLogic();
        }

        if (perimeterState == "chase")
        {
            ChaseLogic();
        }
        if (perimeterState == "return")
        {
            ReturnLogic();
        }
    }

    //will possibly need a state machine that can take multiple catridges at once
    
        /*
          List of necessary states:
          1. SearchFor
          2. DetectionCheck (refines search results)
          3. Follow (can be altered in this script to chase quickly or just go grab something passively, no need to make two istates
          4. Return Home (can be a simple function)
          5. Passive Patrol (turning left and right)
          6. Active Patrol (following a set of waypoints)
         */
        
    private void PatrolLogic()
    {
        if (prioretyTarget != null)
        {
            Collider prioretyTargetBuffer = null;
            prioretyTargetBuffer = prioretyTarget;
            this.locationMachine.ChangeState(new Follow(this.gameObject, prioretyTarget.transform, this.GetComponent<EnemyDetectionComponents>().detectionRadius, navMeshAgent));

            if (this.GetComponent<InfoBuffer>().playerInPatrolArea && prioretyTargetBuffer.tag == this.GetComponent<EnemyDetectionComponents>().SearchTag)
            {
                prioretyTarget = prioretyTargetBuffer;
            }

            if (prioretyTarget.tag != this.GetComponent<EnemyDetectionComponents>().SearchTag)
            {
                this.detectMachine.ChangeState(new SearchFor(this.GetComponent<EnemyDetectionComponents>().SearchLayer, this.gameObject,
        this.GetComponent<EnemyDetectionComponents>().detectionRadius, this.GetComponent<EnemyDetectionComponents>().SearchTag, this.SomethingFound));
                if (foundObjects.Count !=0)
                {
                    for (int i = 0; i<foundObjects.Count; i++)
                    {
                        if (foundObjects[i].tag == this.GetComponent<EnemyDetectionComponents>().SearchTag)
                        {
                            this.GetComponent<InfoBuffer>().PrioretyTarget = foundObjects[i];
                            foundObjects.Clear();
                        }
                    }
                }
            }
        }

        else
        {
            navMeshAgent.SetDestination(perimeterLocation.transform.position);

            this.detectMachine.ChangeState(new SearchFor(this.GetComponent<EnemyDetectionComponents>().SearchLayer, this.gameObject,
                 this.GetComponent<EnemyDetectionComponents>().detectionRadius, this.GetComponent<EnemyDetectionComponents>().SearchTag, this.SomethingFound));
        }


        if (foundObjects.Count != 0)
        {
            this.detectMachine.ChangeState(new DetectQuery(this.GetComponent<EnemyDetectionComponents>().hasFOV, this.GetComponent<EnemyDetectionComponents>().hasFOV,
                this.GetComponent<EnemyDetectionComponents>().hasFOV, this.GetComponent<EnemyDetectionComponents>().viewAngle, this.transform, foundObjects,
                   this.GetComponent<EnemyDetectionComponents>().SearchTag));

            prioretyTarget = this.GetComponent<InfoBuffer>().PrioretyTarget;

            foundObjects.Clear();
        }


        else
        {
            //Basic Idle State
        }

        // this is why you can't fucking pick secondary objects but WHY SHOULD THAT BE THE CASE
       
        


            /*
            turn back and forth checking 
            check for player, calf, lures
             if a calf is in view but not in patrol area of perimeter
             walk to grab calf
             grab calf
             return home and drop calf
             if player is in view, or is in hearing and is noisy
             chase player until player is out of patrol range and out of sight and earshot
             if lure is noticed
             walk to lure
             */
        }

    private void ChaseLogic()
    {
        if (prioretyTarget == null)
        {
            navMeshAgent.SetDestination(perimeterLocation.transform.position);

            this.detectMachine.ChangeState(new SearchFor(this.GetComponent<EnemyDetectionComponents>().SearchLayer, this.gameObject,
                 this.GetComponent<EnemyDetectionComponents>().detectionRadius, this.GetComponent<EnemyDetectionComponents>().SearchTag, this.SomethingFound));
        }


        if (foundObjects.Count != 0)
        {
            this.detectMachine.ChangeState(new DetectQuery(this.GetComponent<EnemyDetectionComponents>().hasFOV, this.GetComponent<EnemyDetectionComponents>().hasFOV,
                this.GetComponent<EnemyDetectionComponents>().hasFOV, this.GetComponent<EnemyDetectionComponents>().viewAngle, this.transform, foundObjects,
                   this.GetComponent<EnemyDetectionComponents>().SearchTag));

            prioretyTarget = this.GetComponent<InfoBuffer>().PrioretyTarget;

            foundObjects.Clear();
        }

        if (prioretyTarget != null)
        {
            Collider prioretyTargetBuffer = prioretyTarget;
            this.detectMachine.ChangeState(new Follow(this.gameObject, prioretyTarget.transform, this.GetComponent<EnemyDetectionComponents>().detectionRadius, navMeshAgent));
        }
        else
        {
            //Basic Idle State
        }
        
        if (prioretyTarget != null && prioretyTarget.tag != this.GetComponent<EnemyDetectionComponents>().SearchTag)
        {
            this.detectMachine.ChangeState(new SearchFor(this.GetComponent<EnemyDetectionComponents>().SearchLayer, this.gameObject,
    this.GetComponent<EnemyDetectionComponents>().detectionRadius, this.GetComponent<EnemyDetectionComponents>().SearchTag, this.SomethingFound));
        }
        /*
     if player can be seen or heard, chase.
     if player can no longer be seen, return to starting position.
     if returning to starting position, look for player and calves
     if calf is in sight and in perimeter range
     walk to grab galf
     grab calf
     return home and drop calf
     if player is in sight and in perimeter range
     resume chase
     */
    }

    private void ReturnLogic()
    {
        /*
         return to starting position
         */

        navMeshAgent.SetDestination(perimeterLocation.position);
        this.GetComponent<InfoBuffer>().PrioretyTarget = null;
    }




    #endregion

}
