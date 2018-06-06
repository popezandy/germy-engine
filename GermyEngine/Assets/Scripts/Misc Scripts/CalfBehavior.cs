using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalfBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
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
}
