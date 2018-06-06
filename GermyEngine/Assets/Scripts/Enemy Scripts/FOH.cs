using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOH : MonoBehaviour {

    #region Variables

    [Header("This script defines an AI's FOHearing including size and target.")]
	public float maxRadius = 10;
    public bool isInFOH;

    private GameObject playerObject;
    private Transform playerTransform;

	private EnemyCtrl EnemyCtrl;
	private bool isSneaking;
    private bool isDetectable;

    #endregion

    #region Callbacks

    // Use this for initialization
    void Start () {
		GetStaticReferences();
	}
	
	// Update is called once per frame
	void Update () {
        CheckIsInFOH();
	}

    #endregion

    #region Custom Methods

    /*CheckIsInFOH actually has CheckIsDetected as a nested function. First check if target is in range. If it is, check if the target is sneaking.
    if either is untrue, isInFOH is false*/

    private void CheckIsInFOH ()
    {
        if ((transform.position - playerTransform.position).magnitude <= maxRadius)
        {
            isDetectable = true;
            GetDynamicReferences();
            CheckIsDetected();
        }
        else
        {
            isDetectable = false;
            isInFOH = false;
        }
    }

    private void CheckIsDetected()
    {
        if (isDetectable)
        {
            if (!isSneaking)
            {
                isInFOH = true;
            }
            else
            {
                isInFOH = false;
            }
        }
    }

	private void GetStaticReferences(){
		EnemyCtrl = gameObject.GetComponent<EnemyCtrl>();
        playerObject = EnemyCtrl.playerObject;
        playerTransform = playerObject.transform;
        isSneaking = playerObject.GetComponent<PlayerController>().isSneaking;
	}

    private void GetDynamicReferences()
    {
        isSneaking = playerObject.GetComponent<PlayerController>().isSneaking;
    }


	#endregion

}
