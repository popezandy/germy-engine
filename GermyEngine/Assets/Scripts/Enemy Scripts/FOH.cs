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

    /*CheckIsInFOH has CheckIsDetected as a nested function. First check if target is in range. If it is, check if the target is sneaking.
    if either is untrue, isInFOH is false
    
         This class is physically usable but should be converted to an IState to improve reusability. The EnemyCtrl script isn't modular
         to call considering it's only for enemies. It would be better to work with IState, StateMachine, and InfoBuffer.
         What's more, because SearchFor implies a radius without an angle limit, it might be best to make one ISTATE with all
         possible detection methods. The constructor can ask for boolean values about which detection types the particular
         enemy has. It can then package all of that data and send it back to the enemy for processing.
         This works when all detection types have the same radius for an enemy, although it could also work if
         there were alternate constructors which included a custom radius for each detection type. It would unload
         a TON of code from each enemy while giving them the power to refine information to make complex decisions.
         */

    

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
