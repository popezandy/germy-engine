using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class CoverSystem : MonoBehaviour {


        public float spaceFromWall = 0.1f;
        public float wallBuffer;

        [Range (0, 1)]
        public float minimumThickness = 0.5f;

        [Range (0, 1)]
        public float crouchAmount = 0.2f;

        private float checkerDistanceBuffer;
        private Vector3 checkerNormalBuffer;

        private Vector3 tempPosition;
        private Vector3 snapPosition;
        private Vector3 snapOrientation;
        private Vector3 newPosition;

        private Vector3 checkerPosition;
        private Vector3 checkerRotation;


        private bool isInCover;
        private bool endOfCover;

        private LayerMask coverMask;

        public void InitiateCoverSystem() {

            isInCover = gameObject.GetComponent<PlayerController>().isInCover;
            coverMask = gameObject.GetComponent<PlayerController>().isGrabbable;

            if (Input.GetButton("sneak"))
            {
                if (!isInCover)
                {
                    endOfCover = false;
                    BoxColliderCheck();
                }
                if (isInCover)
                {
                    SetCurrentPosition();
                    CheckNewPosition();
                    UpdateCurrentPosition();
                    CheckLeaveInput();
                }
            }
            else
            {
                isInCover = false;
                endOfCover = false;
                gameObject.GetComponent<PlayerController>().isInCover = false;
 
            }
        }

        private void BoxColliderCheck()
        {
            Vector3 crouchHeight = new Vector3(transform.position.x, (transform.position.y - crouchAmount), transform.position.z);
            Ray ray = new Ray(crouchHeight, transform.forward);
            RaycastHit tempHit = new RaycastHit();

            if (Physics.Raycast(ray, out tempHit, 4, coverMask))
            {
                checkerDistanceBuffer = tempHit.distance;
                checkerNormalBuffer = tempHit.normal;



                Vector3 leftCheck = transform.TransformPoint(-minimumThickness, -crouchAmount, 0);
                Vector3 rightCheck = transform.TransformPoint(minimumThickness, -crouchAmount, 0);

                Ray leftRay = new Ray(leftCheck, transform.forward);
                Ray rightRay = new Ray(rightCheck, transform.forward);

                RaycastHit leftHit = new RaycastHit();
                RaycastHit rightHit = new RaycastHit();


                
                Debug.DrawRay(leftCheck, transform.forward, Color.black);
                Debug.DrawRay(rightCheck, transform.forward, Color.black);

                if (Physics.Raycast(leftRay, out leftHit, checkerDistanceBuffer, coverMask) || Physics.Raycast(rightRay, out rightHit, checkerDistanceBuffer, coverMask))
                    {
                    if (leftHit.normal == checkerNormalBuffer || rightHit.normal == checkerNormalBuffer)
                    gameObject.GetComponent<PlayerController>().isInCover = true;
                    tempPosition = tempHit.point;
                    snapOrientation = tempHit.normal;
                }
            }
        }

        private void SetCurrentPosition()
        {
            snapPosition = tempPosition + (snapOrientation.normalized * spaceFromWall);            
            transform.position = snapPosition;
            transform.rotation = Quaternion.LookRotation(snapOrientation);
            
        }

        private void CheckNewPosition()
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                float inputDisplacement = wallBuffer * Input.GetAxis("Horizontal");
                Vector3 checkPos = new Vector3(transform.position.x + inputDisplacement, transform.position.y, transform.position.z);
                Ray ray = new Ray(checkPos, -transform.forward);
                RaycastHit checkHit = new RaycastHit();
                if (Physics.Raycast(ray, out checkHit, 1f, coverMask))
                {
                    endOfCover = false;
                    checkerPosition = checkHit.point;
                    checkerRotation = checkHit.normal;
                }
                else { endOfCover = true; }
            }
        } 

        private void UpdateCurrentPosition()
        {
            if (!endOfCover)
            {
                Debug.DrawLine(checkerPosition, transform.position, Color.green, 10f);
                tempPosition = checkerPosition;
                snapOrientation = checkerRotation;
            }
            else return;
        }

        private void CheckLeaveInput()
        {
            if (Input.GetButton("Jump"))
            {
                gameObject.GetComponent<PlayerController>().isInCover = false;
            }
                
        }
        /* If the player is holding sneak, and they approach a wall with
        * reasonable intent, they will snap to that wall, drop the calf
        * if they are carrying it, and turn to face parallel but away
        * from the wall in question.
        * 
        * It will do this by first sending out a raycast, checking whether
        * there is a box collider in front of the player. If there is,
        * check tags, and then check the angle of collision:
        * 
        * Vector3.Angle(normal,rayName)
        * 
        * If the raycast is successful through tag check, the player is rotated
        * according to the following formula-
        * 
        * ((90 + |x|)*(|x|/x)) degrees.
        * 
        * The player will then snap to the point of intersection, plus
        * a buffer unit in the forward direction.
        * 
        * As long as sneak is held, the player can only move left or
        * right along the wall. A checker object will come out of the
        * wall at a buffer distance to the direction the player
        * is trying to walk, will raycast in 3 spots toward the wall
        * and if any detect a collision with a box collider of the
        * correct type and at the correct angle and distance, the checker
        * will set it's current position as the next position for the
        * player to be able to move toward, and make another check. If
        * the check fails, new temporary position shouldn't update, allowing
        * the player to only walk to the last successful check. 
        * 
        * The raycast should also still rotate the player based on the 
        * relative rotation of the wall.
       */
    }
}
