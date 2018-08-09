using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


	public bool lockCursor;
	public float mouseSensitivity = 10;
	//public float camSensitivity = 10;
	public Transform target;
	public float distFromTarget = 2;
	public Vector2 pitchMinMax = new Vector2(-40,85);
	//private bool isAiming = false;

	public float rotationSmoothTime = .12f;
    public float snapSmoothTime = 1f;
	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;

	float yaw;
	float pitch;


	[Header("Collision Vars")]

	[Header("Transparancy")]
	public bool changeTransparency = true;
	public MeshRenderer targetRenderer;

	[Header("Speeds")]
	public float moveSpeed = 5;
	public float returnSpeed = 9;
	public float wallPush = 0.7f;

	[Header("Distances")]
	public float closestDistanceToPlayer = 2;
	public float evenCloserDistanceToPlayer = 1;

	[Header("Mask")]
	public LayerMask collisionMask;

	private bool pitchLock = false;


	private void Start() {
		if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void LateUpdate() {

		/*
		Turn Camera with Q and E, if character is walking foreward (W) the snap back to original position
		if (!isAiming){
		if (Input.GetKey (KeyCode.E))
			yaw += 12 * camSensitivity * Time.deltaTime;
		else if (Input.GetKey (KeyCode.Q))
			yaw -= 12 * camSensitivity * Time.deltaTime;
	//	else if (Input.GetKey (KeyCode.W))
	//	yaw = 0;
		}
*/

        
		CollisionCheck(target.position - transform.forward * distFromTarget);
		WallCheck();

        BasicCamera();

        



	}

	private void WallCheck(){
		Ray ray = new Ray(transform.position, -transform.forward);
        // originally target.transform.position, -target.transform.forward;
		RaycastHit hit;

		if (Physics.SphereCast (ray, 0.2f, out hit, evenCloserDistanceToPlayer, collisionMask)) {
			pitchLock = true;
		} else {
			pitchLock = false;
		}
	}

	private void CollisionCheck (Vector3 retPoint) {

		RaycastHit hit;

		if (Physics.Linecast(target.position, retPoint,out hit, collisionMask)) {
			Vector3 norm = hit.normal * wallPush;
			Vector3 p = hit.point + norm;

			TransparencyCheck();

			if (Vector3.Distance(Vector3.Lerp (transform.position, p, moveSpeed * Time.deltaTime), target.position) <= evenCloserDistanceToPlayer)
            {

			} else {
				transform.position = Vector3.Lerp (transform.position, p, moveSpeed * Time.deltaTime);
			}

			return;

		}


		FullTransparency ();

		transform.position = Vector3.Lerp (transform.position, retPoint, returnSpeed * Time.deltaTime);
		pitchLock = false;
	}

	private void TransparencyCheck(){

		if (changeTransparency) {
			if (Vector3.Distance (transform.position, target.position) <= closestDistanceToPlayer) {

				Color temp = targetRenderer.sharedMaterial.color;
				temp.a = Mathf.Lerp (temp.a, 0.2f, moveSpeed * Time.deltaTime);

				targetRenderer.sharedMaterial.color = temp;

				//sharedMaterial(s) can be looped through

			} else {

				if (targetRenderer.sharedMaterial.color.a <= 0.99f) {
					
					Color temp = targetRenderer.sharedMaterial.color;
					temp.a = Mathf.Lerp (temp.a, 1, moveSpeed * Time.deltaTime);

					targetRenderer.sharedMaterial.color = temp;
				}
			}

		}

	}

	private void FullTransparency() {
		if (changeTransparency) {
			
			Color temp = targetRenderer.sharedMaterial.color;
			temp.a = Mathf.Lerp (temp.a, 1, moveSpeed * Time.deltaTime);

			targetRenderer.sharedMaterial.color = temp;

		}

	}

    private void BasicCamera()
    {

        if (!pitchLock)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch = pitchMinMax.y;

            currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);

        }

        if (!target.GetComponent<InfoBuffer>().disableControl)
        {
            
            transform.eulerAngles = currentRotation;

            /*
		    Vector3 e = transform.eulerAngles;
		    e.x = 0;

            
		    target.eulerAngles = e;
            */
        }
        else
            {
                AdvancedCamera();
            }
    }

    private void AdvancedCamera()
    {

        Vector3 temp = target.transform.position + (target.transform.forward.normalized * 8);
        temp.y = target.transform.position.y + 2;

        transform.position = Vector3.Lerp(transform.position, temp, snapSmoothTime * Time.deltaTime);
        transform.LookAt(target);
        //currentRotation = transform.eulerAngles;

        
        /*
        if (!pitchLock)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch = pitchMinMax.y;

            currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);

        }
        */
    }
}