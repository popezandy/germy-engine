using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{

    /*
     This is purely a data class for enemies. Detection should all be done in a single detection IState
     based on which booleans are checked here. This can apply to all Enemy scripts and be required.
     The actual logic can be handled within the particular enemy script. 
         */

    public bool hasFOV;
    public bool hasFOH;
    public bool hasFOS;
    public bool hasCheckID;
    public bool activePatrol;

}   
