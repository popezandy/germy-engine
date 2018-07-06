using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GermyUIHandler : MonoBehaviour {

    public GameObject player;
    public GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (player.GetComponent<InfoBuffer>().playerInPatrolArea || Vector3.Distance(player.transform.position, target.transform.position) <= target.GetComponent<FOH>().maxRadius)
        {
            gameObject.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }

    }
}
