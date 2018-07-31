using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inv_Manager : MonoBehaviour {

    public GameObject leftHand;
    public GameObject rightHand;



    public float pickupRange;
    public LayerMask pickUpLayer;

    public List<GameObject> Inventory = new List<GameObject>();

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update () {
        PickUp();
        HandEquip();
        RunHandScripts();
    }

    private void PickUp()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(transform.position, transform.forward);
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (Physics.Raycast(ray, out hit, pickupRange, pickUpLayer))
            {
                Inventory.Add(hit.transform.gameObject);
                hit.transform.gameObject.SetActive(false);
            }
        }
    }

    private void HandEquip()
    {
        if (Inventory.Count > 0 && leftHand == null)
        {
            Instantiate(Inventory[0].transform.gameObject, this.transform);
            leftHand = Inventory[0];
        }
        if (Inventory.Count > 1)
        {
            Instantiate(Inventory[1].transform.gameObject, this.transform);
            rightHand = Inventory[1];
        }
    }

    private void RunHandScripts()
    {
        if (leftHand != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                leftHand.GetComponent<inv_ItemSettings>().Run();
                Debug.Log("You pressed F");
            }
        }
        if (rightHand != null)
        {
            rightHand.GetComponent<inv_ItemSettings>().Run();
        }
    }
}
