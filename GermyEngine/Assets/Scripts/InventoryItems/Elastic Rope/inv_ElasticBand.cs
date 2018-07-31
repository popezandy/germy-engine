using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inv_ElasticBand : IActionSystem {

    GameObject ownerObject;
    LayerMask latchLayer;
    float useDistance;

    public inv_ElasticBand(GameObject OwnerObject, LayerMask LatchLayer, float UseDistance)
    {
        ownerObject = OwnerObject;
        latchLayer = LatchLayer;
        useDistance = UseDistance;
    }

	public void Run()
    {
        UseItem();
        Debug.Log("Use Item is running");
    }

    private void UseItem()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(ownerObject.transform.position, ownerObject.transform.forward);
        
        if (Physics.Raycast(ray, out hit, useDistance, latchLayer))
            {
            hit.transform.gameObject.AddComponent<ElasticBungee>();
            Debug.Log("you applied an elastic bungee to that!");
            }
    }

    private void RunCase(int Case)
    {

    }
}
