using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour {

    public bool btn_pressed;
    public bool btn_works = true;

    public LayerMask layerMask;

    private void OnTriggerStay(Collider other)
    {
        if (btn_works && ((layerMask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer))
        {
            btn_pressed = true;
            this.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (btn_works && ((layerMask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer))
        {
            btn_pressed = false;
            this.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
