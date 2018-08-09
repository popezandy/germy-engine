using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ptcl_germcloud : MonoBehaviour {

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<InfoBuffer>())
        {
            collision.gameObject.GetComponent<InfoBuffer>().inGas = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<InfoBuffer>())
        {
            collision.gameObject.GetComponent<InfoBuffer>().inGas = false;
        }
    }
}
