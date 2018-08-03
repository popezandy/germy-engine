using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phy_grabPlayer : MonoBehaviour {

    public LayerMask layerMask;

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            other.transform.SetParent(this.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((layerMask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            other.transform.SetParent(null);
        }
    }
}
