using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class waypointFollow : MonoBehaviour
{

    public Transform[] path;
    public float speed = 5.0f;
    public float reachDist = 1.0f;
    public int currentPoint = 0;
    private float dist;

    void Update()
    {
        for (int i = 0; i < path.Length; i++)
        {
            if (currentPoint < path.Length)
            {
                dist = Vector3.Distance(path[currentPoint].position, transform.position);
            } else
            {
                dist = Vector3.Distance(path[i].position, transform.position);
            }

            if (currentPoint < path.Length && reachDist < dist)
            {
                this.GetComponent<Rigidbody>().useGravity = false;
                this.GetComponent<Rigidbody>().isKinematic = true;

                transform.position = Vector3.MoveTowards(transform.position, path[currentPoint].position, Time.deltaTime * speed);
            }

            
            if (dist <= reachDist && currentPoint < path.Length)
            {
                currentPoint++;
            }
            if (dist <= reachDist && currentPoint == path.Length)
            {
                this.GetComponent<Rigidbody>().useGravity = true;
                this.GetComponent<Rigidbody>().isKinematic = false;
                this.GetComponent<waypointFollow>().enabled = false;
                Debug.Log("this happened");
                
            }

            
        }
    }

    void OnDrawGizmos()
    {
        if (path.Length > 0)
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] != null)
                {
                    Gizmos.DrawSphere(path[i].position, reachDist);
                }
            }
    }
}