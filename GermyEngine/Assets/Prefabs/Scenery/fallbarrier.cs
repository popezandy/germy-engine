using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallbarrier : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Vector3 castVector = other.GetComponent<InfoBuffer>().lastplaceGrounded;
        Ray ray = new Ray(castVector, Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.SphereCast(ray, 1f, out hit, 2f))
        {
            
        }

        other.transform.position = other.GetComponent<InfoBuffer>().lastplaceGrounded;
    }

    //spherecast buffer would prevent this fucking up 
}
