using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inv_ItemSettings : MonoBehaviour {

    public IActionSystem ItemLogic;
    public LayerMask latchLayer;
    public int useDistance;

    public void Run()
    {
        if (this.name == "ElasticBand")
        {
            ItemLogic = new inv_ElasticBand(this.transform.gameObject, latchLayer, useDistance);
            ItemLogic.Run();
            Debug.Log("Item Logic is Running");
        }
    }
}
