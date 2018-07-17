using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DetectQuery : IState
{
    private bool hasFOV, hasFOH, hasFOS;
    private float angleFOV;

    private Transform ownerTransform;
    private string prioretyTag;

    private List<Collider> setOfObjects = new List<Collider>();
    private List<Collider> nonPrioretyObjects = new List<Collider>();
    private Collider nonPrioretyTarget;

    public DetectQuery(bool hasFOV, bool hasFOH, bool hasFOS, float angleFOV,Transform ownerTransform, List<Collider> setOfObjects, string prioretyTag)
    {
        this.hasFOV = hasFOV;
        this.hasFOH = hasFOH;
        this.hasFOS = hasFOS;
        this.angleFOV = angleFOV;
        this.ownerTransform = ownerTransform;
        this.setOfObjects = setOfObjects;
        this.prioretyTag = prioretyTag;
    }



    public void Enter()
    {
        Debug.Log("Entered Detectquerystate");
        for (int i = 0; i < setOfObjects.Count; i++)
        {
            if (ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget == null)
            {

                if (hasFOV)
                {

                }


                if (hasFOH)
                {
                    if (setOfObjects[i].GetComponent<InfoBuffer>().isNoisy)
                    {
                        if (prioretyTag == setOfObjects[i].tag)
                        {
                            this.ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget = setOfObjects[i];
                        }

                        else
                        {
                            nonPrioretyObjects.Add(setOfObjects[i]);
                        }
                    }
                }



                if (hasFOS)
                {
                    if (setOfObjects[i].GetComponent<InfoBuffer>().isSmelly)
                    {
                        if (prioretyTag == setOfObjects[i].tag)
                        {
                            this.ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget = setOfObjects[i];
                        }
                    }
                }
            }
        }


        if (ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget == null)
        {
            for (int i = 0; i < nonPrioretyObjects.Count; i++)
            {
                Debug.Log("is this thing even running?");
                if (nonPrioretyTarget == null)
                {
                    nonPrioretyTarget = nonPrioretyObjects[i];
                    Debug.Log("it has set a new non priorety target");
                }
                else
                {
                    if (Vector3.Distance(nonPrioretyObjects[i].transform.position, ownerTransform.position) < Vector3.Distance(nonPrioretyTarget.transform.position, ownerTransform.position))
                    {
                        nonPrioretyTarget = nonPrioretyObjects[i];
                        
                    }
                }
            }
            ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget = nonPrioretyTarget;
        }
    }

    public void Execute()
    {
        Debug.Log("Entered Detectquerystate");
        for (int i = 0; i < setOfObjects.Count; i++)
        {
            if (ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget == null)
            {

                if (hasFOV)
                {

                }


                if (hasFOH)
                {
                    if (setOfObjects[i].GetComponent<InfoBuffer>().isNoisy)
                    {
                        if (prioretyTag == setOfObjects[i].tag)
                        {
                            this.ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget = setOfObjects[i];
                        }

                        else
                        {
                            nonPrioretyObjects.Add(setOfObjects[i]);
                        }
                    }
                }



                if (hasFOS)
                {
                    if (setOfObjects[i].GetComponent<InfoBuffer>().isSmelly)
                    {
                        if (prioretyTag == setOfObjects[i].tag)
                        {
                            this.ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget = setOfObjects[i];
                        }
                    }
                }
            }
        }


        if (ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget == null)
        {
            for (int i = 0; i < nonPrioretyObjects.Count; i++)
            {
                Debug.Log("is this thing even running?");
                if (nonPrioretyTarget == null)
                {
                    nonPrioretyTarget = nonPrioretyObjects[i];
                    Debug.Log("it has set a new non priorety target");
                }
                else
                {
                    if (Vector3.Distance(nonPrioretyObjects[i].transform.position, ownerTransform.position) < Vector3.Distance(nonPrioretyTarget.transform.position, ownerTransform.position))
                    {
                        nonPrioretyTarget = nonPrioretyObjects[i];
                        
                    }
                }
            }
            ownerTransform.GetComponent<InfoBuffer>().PrioretyTarget = nonPrioretyTarget;
        }
    }

    public void Exit()
    {
        
    }
}
