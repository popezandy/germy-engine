using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBuffer : MonoBehaviour
{

    public string PerimeterState;
    public bool toggleDebug;
    public bool playerInPatrolArea;

    private void Update()
    {
        if (toggleDebug)
        {
            if (PerimeterState == "patrol")
            {
                Debug.Log(PerimeterState);
                //if player hasn't been seen or heard since he last left the chase area and there are no sheep visible in the chase area, return to patrol point


                //if player is detected, chase player until he has left the chase area and can't be seen or heard. IF the player has been detected and is still in the patrol area, he is still
                //being pursued.
            }

            if (PerimeterState == "chase")
            {
                Debug.Log(PerimeterState);
                //if player can't be seen or heard and there are no sheep in chase area, return to patrol point
            }

            if (PerimeterState == "return")
            {
                Debug.Log(PerimeterState);
                //return to patrol point
            }
        }
    }
}
