using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLookMeter : MonoBehaviour {

	/*The next iteration of this script needs to prevent sub-distance-to-player from fucking my angles.
	 * 
	 * 
	 */

	#region Public Variables

	public Transform target;
	public Transform player;
	public float distFromTarget = 5;
	public bool whereToLook;

	#endregion

	#region Private Variables

	private Vector3 positionFromPlayer;
	private Vector3 newPosition;
	private Vector3 angleFromPlayer;
	private float trueDistFromTarget;

	[Header("True follows Player, False follows Enemy")]
	private Vector3 targetLook;

	#endregion

	#region Start/Update/Destroy

	void Start () {
		
	}

	void Update () {

		CalculateNewPosition();
		WhereToLook();

		transform.position = newPosition;
		transform.LookAt (targetLook);

	}

	#endregion

	#region Main Methods

	void CalculateNewPosition(){
		
		positionFromPlayer = (player.position - target.position);

		trueDistFromTarget = distFromTarget/positionFromPlayer.magnitude;
		

		angleFromPlayer = new Vector3((positionFromPlayer.x*trueDistFromTarget),0,(positionFromPlayer.z*trueDistFromTarget));

		newPosition = new Vector3 ((target.position.x + angleFromPlayer.x), target.position.y, (target.position.z + angleFromPlayer.z));

	}

	void WhereToLook(){
		if (!whereToLook) {
			targetLook = target.position;
		} else if (whereToLook) {
			targetLook = player.position;
		}
	}

	#endregion
}
