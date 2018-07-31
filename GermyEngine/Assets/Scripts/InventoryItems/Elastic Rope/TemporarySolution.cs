using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporarySolution : MonoBehaviour {

    public GameObject Player;
    public float length;
    public float springStrength;
    public LayerMask NoTreeNoPlayer;

	void Update () {
	
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = new Ray(Player.transform.position, Player.transform.forward);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit, 5))
            {
                if (hit.transform.name == this.name)
                {
                    if (!Player.GetComponent(typeof(ElasticBungee)))
                    {
                        Player.AddComponent<ElasticBungee>();
                        Player.GetComponent<ElasticBungee>().targetTransform = this.transform;
                        Player.GetComponent<ElasticBungee>().bandLength = length;
                        Player.GetComponent<ElasticBungee>().alllColliders = NoTreeNoPlayer;
                        Player.GetComponent<SpringJoint>().spring = 25;
                    }
                    else
                    {
                        Destroy(Player.GetComponent<ElasticBungee>());
                        Destroy(Player.GetComponent<LineRenderer>());
                        Destroy(Player.GetComponent<SpringJoint>());
                    }
                }
            }
        }

	}
}
