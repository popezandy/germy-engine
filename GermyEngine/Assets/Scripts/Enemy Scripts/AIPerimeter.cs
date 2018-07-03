using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPerimeter : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
        ShapePreCheck();
        ShapeCheck();
        
	}
	
    void Update () { DrawShape(); }

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
        if (cylinder) { shapeCase = 0;}
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
        Vector3 top = new Vector3(transform.position.x, transform.position.y, transform.position.z + height);
        Vector3 bottom = transform.position;
        Vector3 patrolRadius = new Vector3(transform.position.x + patrolWidth / 2, transform.position.y, transform.position.z);
        Vector3 chaseRadius = new Vector3(transform.position.x + chaseWidth / 2, transform.position.y, transform.position.z);

        // pi r squared times height
    }
    private void DrawRectangle()
    {
        // width squared times height
        // this position + width/2 x, this position - width/2 x, this position + width/2 z, this position - width/2 z
        Vector3 patrolTopLeftFront = new Vector3(transform.position.x - chaseWidth / 2, transform.position.y + height, transform.position.z + chaseWidth / 2);
        Vector3 patrolTopRightFront = new Vector3(transform.position.x + chaseWidth / 2, transform.position.y + height, transform.position.z + chaseWidth / 2);
        Vector3 patrolTopLeftBack = new Vector3(transform.position.x - chaseWidth / 2, transform.position.y + height, transform.position.z - chaseWidth / 2);
        Vector3 patrolTopRightBack = new Vector3(transform.position.x + chaseWidth / 2, transform.position.y + height, transform.position.z - chaseWidth / 2);
        Vector3 patrolBottomLeftFront = new Vector3(transform.position.x - chaseWidth / 2, transform.position.y, transform.position.z + chaseWidth / 2);
        Vector3 patrolBottomRightFront = new Vector3(transform.position.x + chaseWidth / 2, transform.position.y, transform.position.z + chaseWidth / 2);
        Vector3 patrolBottomLeftBack = new Vector3(transform.position.x - chaseWidth / 2, transform.position.y, transform.position.z - chaseWidth / 2);
        Vector3 patrolBottomRightBack = new Vector3(transform.position.x + chaseWidth / 2, transform.position.y, transform.position.z - chaseWidth / 2);

        Debug.DrawLine(patrolTopLeftFront, patrolTopRightFront);
        Debug.DrawLine(patrolTopLeftFront, patrolTopLeftBack);
        Debug.DrawLine(patrolTopLeftFront, patrolBottomLeftFront);
        Debug.DrawLine(patrolBottomRightBack, patrolBottomRightFront);
        Debug.DrawLine(patrolBottomRightBack, patrolBottomLeftBack);
        Debug.DrawLine(patrolBottomRightBack, patrolTopRightBack);
    }
    private void DrawTriangle()
    {
        // one line goes straight ahead, one line goes -1 -1 and one line goes 1 -1
    }


}
