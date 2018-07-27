using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLine : MonoBehaviour
{

    LineRenderer lineRenderer = new LineRenderer();
    public Transform targetTransform;
    public LayerMask alllColliders;
    public int maxKinks = 10;
    public float cornerBuffer = 0.02f;
    public float bandLength = 30;

    private Vector3 currentActualTargetPosition;
    private Vector3 ethanCenterMass;
    private float totalDistance;
    private Rigidbody rigidBody;
    private SpringJoint springJoint;

    // Use this for initialization
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        rigidBody = this.GetComponent<Rigidbody>();
        springJoint = this.GetComponent<SpringJoint>();

        ethanCenterMass = new Vector3(this.transform.position.x, this.transform.position.y /*+ 0.8f*/, this.transform.position.z);


        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(1, ethanCenterMass);
        lineRenderer.SetPosition(0, targetTransform.position);
        
    }

    // Update is called once per frame
    void Update()
    {
        ethanCenterMass = new Vector3(this.transform.position.x, this.transform.position.y + 0.8f, this.transform.position.z);
        springJoint.connectedAnchor = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        UnKinkChecker();
        KinkChecker();
        DistanceCalculator();
        InputManager();
        

    }


    private void UnKinkChecker()
    {
        if (lineRenderer.positionCount > 2)
        {

            int firstVertex = lineRenderer.positionCount - 1;
            int secondVertex = lineRenderer.positionCount - 3;

            int cornerVertex = lineRenderer.positionCount - 2;

            Vector3 firstLine = (lineRenderer.GetPosition(cornerVertex) - lineRenderer.GetPosition(firstVertex)).normalized;
            Vector3 secondLine = (lineRenderer.GetPosition(cornerVertex) - lineRenderer.GetPosition(secondVertex)).normalized;
            float angle = Vector3.Angle(firstLine, secondLine);

            RaycastHit shit = new RaycastHit();
            Ray ray = new Ray(lineRenderer.GetPosition(firstVertex), lineRenderer.GetPosition(secondVertex) - lineRenderer.GetPosition(firstVertex));

            if (Physics.Raycast(ray, out shit, Vector3.Distance(lineRenderer.GetPosition(firstVertex), lineRenderer.GetPosition(secondVertex)), alllColliders))
            {
                if (lineRenderer.positionCount > 3)
                {
                    if (Vector3.Distance(shit.point, lineRenderer.GetPosition(secondVertex)) < cornerBuffer * 2)
                    {
                        lineRenderer.positionCount -= 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, this.transform.position);
                    }
                }
                if (lineRenderer.positionCount == 3)
                {
                    if(shit.transform.name == targetTransform.name && (angle >= 170 || this.transform.position.y > lineRenderer.GetPosition(1).y + 1))
                    {
                        lineRenderer.positionCount -= 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, this.transform.position);
                    }
                }
            }
        }
    }

    private void KinkChecker()
    {
        if (currentActualTargetPosition != null)
        {
            if (lineRenderer.positionCount == 2)
            {
                lineRenderer.SetPosition(1, ethanCenterMass);
                lineRenderer.SetPosition(0, targetTransform.position);
                currentActualTargetPosition = targetTransform.position;
            }

            if (lineRenderer.positionCount <= maxKinks)
            {
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ethanCenterMass, (currentActualTargetPosition - ethanCenterMass).normalized, out hit, Vector3.Distance(ethanCenterMass, currentActualTargetPosition), alllColliders))
                {
                    if (lineRenderer.positionCount == 2 && hit.transform == targetTransform)
                    {
                        lineRenderer.SetPosition(1, ethanCenterMass);
                        lineRenderer.SetPosition(0, targetTransform.position);
                    }

                    if (Vector3.Distance(hit.point, currentActualTargetPosition) > cornerBuffer && lineRenderer.positionCount >= 2)
                    {
                        currentActualTargetPosition = hit.point;

                        int thisCurrentIndex = lineRenderer.positionCount - 1;
                        lineRenderer.positionCount++;

                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, ethanCenterMass);
                        lineRenderer.SetPosition(thisCurrentIndex, currentActualTargetPosition);
                    }
                    if (hit.transform.name == targetTransform.name && lineRenderer.positionCount == 3)
                    {
                        lineRenderer.positionCount -= 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, this.transform.position);
                    }
                }
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ethanCenterMass);
            }
        }

        if (lineRenderer.positionCount < 2)
        {
            lineRenderer.positionCount = 2;
        }
    }

    private void DistanceCalculator()
    {
        totalDistance = 0;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            if (i != 0)
            {
                totalDistance += Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i - 1));
            }
        }

        float thisline = Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), lineRenderer.GetPosition(lineRenderer.positionCount - 2));

        float preDistance = totalDistance - thisline;
        float maxRadius = bandLength - preDistance;

        if (thisline > maxRadius)
        {
            Vector3 fromOriginToObject = ethanCenterMass - lineRenderer.GetPosition(lineRenderer.positionCount - 2) ; //~GreenPosition~ - *BlackCenter*
            fromOriginToObject *= maxRadius / thisline; //Multiply by radius //Divide by Distance
            ethanCenterMass = lineRenderer.GetPosition(lineRenderer.positionCount - 2) + fromOriginToObject; //*BlackCenter* + all that Math
            springJoint.connectedAnchor = ethanCenterMass;
        }

    }

    /*
     * Vector3 fromPosition = source.transform.position;
     * Vector3 toPosition = destination.transform.position;
     * Vector3 direction = toPosition - fromPosition;
     * 
     * 
     * I want to be able to add a new vertice at any new stopping point, make that vertice the new target, and move all vertices to reflect that in the group.
 */
 private void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //this.springJoint.spring *= 10;
            bandLength /= 2;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //this.springJoint.spring /= 10;
            bandLength *= 2;
        }
    }
}
