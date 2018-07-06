using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class CalfBehavior : MonoBehaviour {

    #region Class Description
    /*This is the script that dictates the state machine for Cure Calfs. 
   Cure Calfs are a grazing animal, with a fascination for pressure pads, germy napkins, other calf pheromones, and their home field. 
   Cure Calfs are not affected by germ clouds, but can be injured other things such as explosives or falling off of a cliff. 
   A calf can be picked up by most other game participants, notably the Player and Germy Jack.

   A calf is said to be in the winning position if the calf has been transported to the level's winner's circle. 

   STATES
   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   Grazing:
   passive patrol mode
   able to transition into being seduced or being carried.

   Returning Home:
   if obedience threshold > current distance > home position
   return home
   return to grazing if currentposition ~= homeposition
   transition to being seduced or being carried if either is true

   Being Seduced:
   if being seduced
   approach seductive force until reaching obstenance threshold or force dissipates


   Being Carried:
   if being carried
   no gravity, parent to player hands, disable all other behavior, enable carry animations
   if set down, transition to carry buffer

   Carry Buffer:
   unparent from player hands, reenable gravity, run state checks to decide return home or obedience.

   Obstenance:
   if distance to seductive item < obstenance threshold
   stay put
   cannot return home, cannot patrol
   can be seduced, can be carried

   Obedience:
   if distance to homeposition > obedience threshold
   can be seduced, can be carried
   cannot patrol, cannot return home

    */
    #endregion

    #region Variables

    private StateMachine stateMachine = new StateMachine();
    [SerializeField]
    private LayerMask lureLayer;
    [SerializeField]
    private float viewRange;
    [SerializeField]
    private string lureItemsTag;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.stateMachine.ChangeState(new SearchFor(this.lureLayer, this.gameObject, this.viewRange, this.lureItemsTag, this.LureFound));
    }

    private void Update()
    {
        this.stateMachine.ExecuteStateUpdate();
    }

    public void LureFound(SearchResults searchResults)
    {
        var foundLures = searchResults.AllHitObjectsWithRequiredTag;
        //choose which object to approach
    }

    #endregion


}
