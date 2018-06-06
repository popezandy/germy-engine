using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour {

    #region Variables

    private Transform playerTransform = null;

	public float maxAngle;
	public float maxRadius;

	public bool isInFOV = false;
	private EnemyCtrl EnemyCtrl;

    #endregion

    #region Visualization

    private void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(transform.position, maxRadius);

		Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
		Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

		Gizmos.color = Color.magenta;
		Gizmos.DrawRay(transform.position, fovLine1);
		Gizmos.DrawRay(transform.position, fovLine2);

		if (!isInFOV)
			Gizmos.color = Color.black;
		else
			Gizmos.color = Color.cyan;

        if (playerTransform != null)
        {
            Gizmos.DrawRay(transform.position, (playerTransform.position - transform.position).normalized * maxRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
        }
	}

    #endregion

    #region CallBacks

    private void Awake()
	{
		GetPlayer();
	}
	private void Update()
	{
		isInFOV = inFOV(transform, playerTransform, maxAngle, maxRadius);
	}

    #endregion

    #region Functions

    private void GetPlayer()
	{
		EnemyCtrl = gameObject.GetComponent<EnemyCtrl>();
        playerTransform = EnemyCtrl.playerObject.transform;
	}

    public static bool inFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {

        Collider[] overlaps = new Collider[20];
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overlaps);

        for (int i = 0; i < count + 1; i++)
        {
            if (overlaps[i] != null)
            {
                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - checkingObject.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                    if (angle <= maxAngle)
                    {

                        Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == target)
                                return true;
                        }

                    }
                }
            }

        }

        return false;
    }

    #endregion
}