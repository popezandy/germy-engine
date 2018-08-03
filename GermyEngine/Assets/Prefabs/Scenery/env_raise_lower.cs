using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class env_raise_lower : MonoBehaviour {

    public GameObject button;

    public Transform origin;
    public Transform destination;
    public float speed = 5f;
    public float minDist;

    public bool cycle;

    private Vector3 origOrigin;
    private Vector3 currentDest;

    private void Start()
    {
        origOrigin = new Vector3(origin.transform.position.x, origin.transform.position.y, origin.transform.position.z);
    }
    private void Update()
    {
        
        if (button.GetComponent<button>())
        {
            bool buttonPressed = button.GetComponent<button>().btn_pressed;

            if (buttonPressed)
            {
                if (!cycle)
                {
                    HeadToDestination();
                }
                else
                {
                    CycleDest();
                }
            }
            else
            {
                if (!cycle)
                {
                    HeadToOrigin();
                }
                else
                {
                    Stop();
                }
            }
        }
        else { Debug.Log("Connected Object doesn't have a button script"); }
    }

    private void HeadToDestination()
    {
        float currentDistance = Vector3.Distance(this.transform.position, destination.transform.position);
        if (currentDistance > minDist)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination.transform.position, Time.deltaTime * speed);
        }
        else
        {
            transform.position = destination.transform.position;
        }
    }

    private void HeadToOrigin()
    {
        float currentDistance = Vector3.Distance(this.transform.position, origOrigin);
        if (currentDistance > minDist)
        {
            transform.position = Vector3.MoveTowards(transform.position, origOrigin, Time.deltaTime * speed);
        }
        else
        {
            transform.position = origOrigin;
        }
    }

    private void CycleDest()
    {
        float distToOrigin = Vector3.Distance(this.transform.position, origOrigin);
        float distToDest = Vector3.Distance(this.transform.position, destination.transform.position);

        if (distToDest < minDist)
        {
            currentDest = origOrigin;
        }

        if (distToOrigin < minDist)
        {
            currentDest = destination.transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentDest, Time.deltaTime * speed);
    }

    private void Stop()
    {
        transform.position = transform.position;
    }
}
