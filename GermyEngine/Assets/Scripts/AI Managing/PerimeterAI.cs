using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PerimeterAI : MonoBehaviour
{

    // Class Description
    /*
     This is a 3 tier perimeter definer for an empty perimeter object. The object will determine AI behavior based on how deep into the perimeter object the AI actor is in. 
     There are options for Cyllinder, Rectangular, and Triangular Prism.
         */
    [Header("Shape options. Choose 1.")]
    public bool cylinder;
    public bool rectangular;
    public bool triangular;

    public float height;
    public float patrolWidth;
    public float chaseWidth;

    private int shapeCase;
    private int logicCase;
    private float tri = 3.33333333333333333333333333333333f;
    public GameObject[] interactingAgent;
    public Transform perimeterCenter;
    public Transform Player;

    public bool debugScript;
    // Use this for initialization

    void Update()
    {
        ShapePreCheck();
        ShapeCheck();
        DrawShape();
        AIPerimeterLogic();
        DebugTransforms();
    }


    #region Shape Making

    private void ShapePreCheck()
    {
        if (cylinder) { rectangular = false; triangular = false; };
        if (rectangular) { cylinder = false; triangular = false; };
        if (triangular) { cylinder = false; rectangular = false; };

        if (height < 0) height = 0;
        if (patrolWidth < 1) patrolWidth = 1;
        if (chaseWidth < patrolWidth) chaseWidth = (patrolWidth + 1);
    }

    private void ShapeCheck()
    {
        if (cylinder) { shapeCase = 0; }
        else if (rectangular) { shapeCase = 1; }
        else if (triangular) { shapeCase = 2; };
    }

    private void DrawShape()
    {
        if (shapeCase == 0)
        {
            DrawCylinder();
        }
        if (shapeCase == 1)
        {
            DrawRectangle();
        }
        if (shapeCase == 2)
        {
            DrawTriangle();
        }
    }


    private void DrawCylinder()
    {
        Vector3 top = new Vector3(perimeterCenter.transform.position.x, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z);
        Vector3 bottom = perimeterCenter.transform.position;
        Vector3 patrolRadiusBottom = new Vector3(perimeterCenter.transform.position.x + patrolWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z);
        Vector3 chaseRadiusBottom = new Vector3(perimeterCenter.transform.position.x, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + chaseWidth / 2);
        Vector3 patrolRadiusTop = new Vector3(perimeterCenter.transform.position.x + patrolWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z);
        Vector3 chaseRadiusTop = new Vector3(perimeterCenter.transform.position.x, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z + chaseWidth / 2);

        Debug.DrawLine(top, bottom);
        Debug.DrawLine(bottom, patrolRadiusBottom);
        Debug.DrawLine(top, chaseRadiusTop);
        Debug.DrawLine(chaseRadiusTop, chaseRadiusBottom);
        Debug.DrawLine(patrolRadiusTop, patrolRadiusBottom);

        // pi r squared times height
    }
    private void DrawRectangle()
    {

        //patrol box
        Vector3 patrolTopLeftFront = new Vector3(perimeterCenter.transform.position.x - patrolWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z + patrolWidth / 2);
        Vector3 patrolTopRightFront = new Vector3(perimeterCenter.transform.position.x + patrolWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z + patrolWidth / 2);
        Vector3 patrolTopLeftBack = new Vector3(perimeterCenter.transform.position.x - patrolWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z - patrolWidth / 2);
        Vector3 patrolTopRightBack = new Vector3(perimeterCenter.transform.position.x + patrolWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z - patrolWidth / 2);
        Vector3 patrolBottomLeftFront = new Vector3(perimeterCenter.transform.position.x - patrolWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + patrolWidth / 2);
        Vector3 patrolBottomRightFront = new Vector3(perimeterCenter.transform.position.x + patrolWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + patrolWidth / 2);
        Vector3 patrolBottomLeftBack = new Vector3(perimeterCenter.transform.position.x - patrolWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - patrolWidth / 2);
        Vector3 patrolBottomRightBack = new Vector3(perimeterCenter.transform.position.x + patrolWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - patrolWidth / 2);

        Debug.DrawLine(patrolTopLeftFront, patrolTopRightFront);
        Debug.DrawLine(patrolTopLeftFront, patrolTopLeftBack);
        Debug.DrawLine(patrolTopLeftFront, patrolBottomLeftFront);
        Debug.DrawLine(patrolBottomRightBack, patrolBottomRightFront);
        Debug.DrawLine(patrolBottomRightBack, patrolBottomLeftBack);
        Debug.DrawLine(patrolBottomRightBack, patrolTopRightBack);

        //chase box
        Vector3 chaseTopLeftFront = new Vector3(perimeterCenter.transform.position.x - chaseWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z + chaseWidth / 2);
        Vector3 chaseTopRightFront = new Vector3(perimeterCenter.transform.position.x + chaseWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z + chaseWidth / 2);
        Vector3 chaseTopLeftBack = new Vector3(perimeterCenter.transform.position.x - chaseWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z - chaseWidth / 2);
        Vector3 chaseTopRightBack = new Vector3(perimeterCenter.transform.position.x + chaseWidth / 2, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z - chaseWidth / 2);
        Vector3 chaseBottomLeftFront = new Vector3(perimeterCenter.transform.position.x - chaseWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + chaseWidth / 2);
        Vector3 chaseBottomRightFront = new Vector3(perimeterCenter.transform.position.x + chaseWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + chaseWidth / 2);
        Vector3 chaseBottomLeftBack = new Vector3(perimeterCenter.transform.position.x - chaseWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - chaseWidth / 2);
        Vector3 chaseBottomRightBack = new Vector3(perimeterCenter.transform.position.x + chaseWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - chaseWidth / 2);

        Debug.DrawLine(chaseTopLeftFront, chaseTopRightFront);
        Debug.DrawLine(chaseTopLeftFront, chaseTopLeftBack);
        Debug.DrawLine(chaseTopLeftFront, chaseBottomLeftFront);
        Debug.DrawLine(chaseBottomRightBack, chaseBottomRightFront);
        Debug.DrawLine(chaseBottomRightBack, chaseBottomLeftBack);
        Debug.DrawLine(chaseBottomRightBack, chaseTopRightBack);
    }
    private void DrawTriangle()
    {
        //patrol box
        Vector3 patrolTopFront = new Vector3(perimeterCenter.transform.position.x + patrolWidth / tri, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z);
        Vector3 patrolTopRight = new Vector3(perimeterCenter.transform.position.x - patrolWidth / tri, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z + patrolWidth / tri);
        Vector3 patrolTopLeft = new Vector3(perimeterCenter.transform.position.x - patrolWidth / tri, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z - patrolWidth / tri);
        Vector3 patrolBottomFront = new Vector3(perimeterCenter.transform.position.x + patrolWidth / tri, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z);
        Vector3 patrolBottomRight = new Vector3(perimeterCenter.transform.position.x - patrolWidth / tri, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + patrolWidth / tri);
        Vector3 patrolBottomLeft = new Vector3(perimeterCenter.transform.position.x - patrolWidth / tri, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - patrolWidth / tri);

        Debug.DrawLine(patrolTopFront, patrolTopRight);
        Debug.DrawLine(patrolTopFront, patrolTopLeft);
        Debug.DrawLine(patrolTopFront, patrolBottomFront);
        Debug.DrawLine(patrolTopRight, patrolBottomRight);
        Debug.DrawLine(patrolTopLeft, patrolBottomLeft);

        //chase box
        Vector3 chaseTopFront = new Vector3(perimeterCenter.transform.position.x + chaseWidth / tri, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z);
        Vector3 chaseTopRight = new Vector3(perimeterCenter.transform.position.x - chaseWidth / tri, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z + chaseWidth / tri);
        Vector3 chaseTopLeft = new Vector3(perimeterCenter.transform.position.x - chaseWidth / tri, perimeterCenter.transform.position.y + height, perimeterCenter.transform.position.z - chaseWidth / tri);
        Vector3 chaseBottomFront = new Vector3(perimeterCenter.transform.position.x + chaseWidth / tri, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z);
        Vector3 chaseBottomRight = new Vector3(perimeterCenter.transform.position.x - chaseWidth / tri, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + chaseWidth / tri);
        Vector3 chaseBottomLeft = new Vector3(perimeterCenter.transform.position.x - chaseWidth / tri, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - chaseWidth / tri);

        Debug.DrawLine(chaseTopFront, chaseTopRight);
        Debug.DrawLine(chaseTopFront, chaseTopLeft);
        Debug.DrawLine(chaseTopFront, chaseBottomFront);
        Debug.DrawLine(chaseTopRight, chaseBottomRight);
        Debug.DrawLine(chaseTopLeft, chaseBottomLeft);
        Debug.DrawLine(chaseTopLeft, chaseTopRight);

    }

    #endregion


    #region AI Logic

    public void AIPerimeterLogic()
    {

        if (shapeCase == 0)
        {
            CylinderLogic();
        }
        if (shapeCase == 1)
        {
            RectangleLogic();
        }
        if (shapeCase == 2)
        {
            TriangleLogic();
        }
    }

    private void CylinderLogic()
    {
    }

    private void RectangleLogic()
    {
        Vector3 patrolLeft = new Vector3(perimeterCenter.transform.position.x - patrolWidth/2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z);
        Vector3 patrolRight = new Vector3(perimeterCenter.transform.position.x + patrolWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z);
        Vector3 patrolFront = new Vector3(perimeterCenter.transform.position.x, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + patrolWidth / 2);
        Vector3 patrolBack = new Vector3(perimeterCenter.transform.position.x, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - patrolWidth / 2);

        Vector3 chaseLeft = new Vector3(perimeterCenter.transform.position.x - chaseWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z);
        Vector3 chaseRight = new Vector3(perimeterCenter.transform.position.x + chaseWidth / 2, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z);
        Vector3 chaseFront = new Vector3(perimeterCenter.transform.position.x, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z + chaseWidth / 2);
        Vector3 chaseBack = new Vector3(perimeterCenter.transform.position.x, perimeterCenter.transform.position.y, perimeterCenter.transform.position.z - chaseWidth / 2);

        for (int i = 0; i < interactingAgent.Length; i++)
        {
            

            if (interactingAgent[i] != null)
            {
                interactingAgent[i].GetComponent<InfoBuffer>().PerimeterLocation = perimeterCenter.transform;

                if (
                interactingAgent[i].transform.position.x > patrolLeft.x &&
                interactingAgent[i].transform.position.x < patrolRight.x &&
                interactingAgent[i].transform.position.z < patrolFront.z &&
                interactingAgent[i].transform.position.z > patrolBack.z &&
                interactingAgent[i].transform.position.y > perimeterCenter.transform.position.y &&
                interactingAgent[i].transform.position.y < perimeterCenter.transform.position.y + height
                )
                {
                    interactingAgent[i].GetComponent<InfoBuffer>().PerimeterState = "patrol";
                }
                else if (
                interactingAgent[i].transform.position.x > chaseLeft.x &&
                interactingAgent[i].transform.position.x < chaseRight.x &&
                interactingAgent[i].transform.position.z < chaseFront.z &&
                interactingAgent[i].transform.position.z > chaseBack.z &&
                interactingAgent[i].transform.position.y > perimeterCenter.transform.position.y &&
                interactingAgent[i].transform.position.y < perimeterCenter.transform.position.y + height
                )
                {
                    interactingAgent[i].GetComponent<InfoBuffer>().PerimeterState = "chase";
                }
                else interactingAgent[i].GetComponent<InfoBuffer>().PerimeterState = "return";

                if (
                    Player.transform.position.x > patrolLeft.x &&
                    Player.transform.position.x < patrolRight.x &&
                    Player.transform.position.z < patrolFront.z &&
                    Player.transform.position.z > patrolBack.z &&
                    Player.transform.position.y > perimeterCenter.transform.position.y &&
                    Player.transform.position.y < perimeterCenter.transform.position.y + height
                )
                {
                    interactingAgent[i].GetComponent<InfoBuffer>().playerInPatrolArea = true;
                }
                else
                {
                    interactingAgent[i].GetComponent<InfoBuffer>().playerInPatrolArea = false;
                }
            }
        }
        

    }

    private void TriangleLogic()
    {
    }

    private void DebugTransforms()
    {
        if (debugScript)
        {
            for (int i = 0; i < interactingAgent.Length; i++)
            {
                Debug.Log(interactingAgent[i].name + " " + interactingAgent[i].transform.position);
                Debug.Log(perimeterCenter.name + " " + perimeterCenter.transform.position);
            }
        }
    }

    #endregion
}