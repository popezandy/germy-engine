using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterShoes : MonoBehaviour {

    public LayerMask ground;

    public float maxTime;
    public float maxDistance = 15f;
    public float minDistance = .3f;
    private float boostSpeed;

    private float currentTime;
    private Rigidbody Rb;
    private bool deathFall;
    private bool isBoosting;

    private bool boostEnabled = true;

    private float yTransformBuffer;

    private void Start()
    {
        this.Rb = this.GetComponent<Rigidbody>();
        this.minDistance += this.GetComponent<PlayerSettings>().playerHeight/2;
        this.maxDistance += this.GetComponent<PlayerSettings>().playerHeight/2;
        this.boostSpeed = this.GetComponent<PlayerSettings>().boostSpeed;
    }

    void Update()
    {
        boostEnabled = this.GetComponent<InfoBuffer>().boostEnabled;

        if (boostEnabled)
        {
            if (Input.GetKeyDown(KeyCode.R) || isBoosting)
            {
                Countdown();
            }

            if (currentTime <=maxTime && isBoosting)
            {
                StartBoost();
            }
            else if (currentTime >= maxTime && isBoosting)
            {
                StopBoost();
            }
        }
        else
        {
            BoostCancel();
        }
        this.GetComponent<InfoBuffer>().isBoosting = isBoosting;
    }

    private void Countdown()
    {
        currentTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(this.transform.position, Vector3.down, maxDistance, ground))
            {
                if (hit.distance <= minDistance)
                {
                    currentTime = 0;
                    isBoosting = true;
                    yTransformBuffer = this.transform.position.y;
                }
            }
        }
    }

    public void StartBoost()
    {
        Debug.Log("logic is sound it's just broken movement code");
        this.Rb.useGravity = false;
        transform.Translate(0, 0, boostSpeed * Time.deltaTime);
        this.transform.position = new Vector3(this.transform.position.x, yTransformBuffer, this.transform.position.z);
    }

    public void StopBoost()
    {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(this.transform.position, Vector3.down, maxDistance+1, ground, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance > maxDistance)
            { deathFall = true; }
            else
            { deathFall = false; }
        }
        else
        {
            deathFall = true;
        }
            

        if (deathFall)
        {
            //deathAnimation
            this.transform.position = this.GetComponent<InfoBuffer>().lastplaceGrounded;
            this.currentTime = 0;
        }
        else
        {
            
        }
        isBoosting = false;
        this.Rb.useGravity = true;
    }

    public void BoostCancel()
    {
        if(isBoosting)
        {
            this.transform.position = this.transform.position;

            currentTime = 0;
            deathFall = false;
            this.Rb.useGravity = true;
            this.isBoosting = false;
   
        }
    }


}
