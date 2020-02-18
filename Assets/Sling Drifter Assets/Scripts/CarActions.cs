using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarActions : MonoBehaviour
{
    //Cache
    LineRenderer lr;
    Rigidbody2D rb;
    public Transform[] Barrels;
    public Transform[] Cencors;
    public float acceleration;
    public float steering;
    float MoveDirection = 0;
    //Cache

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();

        //Start speed
        rb.AddForce(Vector2.up * 10f);
    }


    void Update()
    {
        /*
        Performance could be improved by setting the closest barrel once then search
        for new barrels after distance is too far from the selected one
        instead of finding the closest barrel every frame.
        */

        if (Vector2.Distance(transform.position, GetClosestBarrel()) < 2.6f)
        {
            if (Input.GetMouseButton(0))
            {
                SetLineRenderer();

                if (GetClosestCencor())
                {
                    //RightCencor
                    MoveDirection = -1;
                }
                else
                {
                    //LeftCencor
                    MoveDirection = 1;
                }
            }
        }     
        
        if(Input.GetMouseButtonUp(0))
        {
            MoveDirection = 0;
            lr.enabled = false;
        }
    }

    void FixedUpdate()
    {
        CarMovement();
    }

    void CarMovement()
    {
        float v = 1f;
        Vector2 speed = transform.up * (v * acceleration);
        //Fixed forward speed
        rb.velocity = speed;

        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
        if (direction >= 0.0f)
        {
            rb.rotation += MoveDirection * steering * (rb.velocity.magnitude / 5.0f);

        }
        else
        {
            rb.rotation -= MoveDirection * steering * (rb.velocity.magnitude / 5.0f);

        }

        Vector2 forward = new Vector2(0.0f, 0.5f);
        float steeringRightAngle;
        if (rb.angularVelocity > 0)
        {
            steeringRightAngle = -90;
        }
        else
        {
            steeringRightAngle = 90;
        }

        Vector2 rightAngleFromForward = Quaternion.AngleAxis(steeringRightAngle, Vector3.forward) * forward;

        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(rightAngleFromForward.normalized));

        Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10.0f);

        rb.AddForce(rb.GetRelativeVector(relativeForce));
    }

    Vector2 GetClosestBarrel()
    {

        Vector2 bestTarget = Vector2.zero;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform potentialTarget in Barrels)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.position;
            }
        }

        return bestTarget;
    }

    bool GetClosestCencor()
    {

        // True - RightCencor || False - LeftCencor

        Vector2 bestTarget = Vector2.zero;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = GetClosestBarrel();
        Transform SelectedCencor = null;

        foreach (Transform potentialTarget in Cencors)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.position;
                SelectedCencor = potentialTarget;
            }
          
        }

        if(SelectedCencor.name == "CencorRight")
        {
            return true;
        }
        else
        {
            return false;
        }       
    }


    void SetLineRenderer()
    {
        lr.enabled = true;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, GetClosestBarrel());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      
        if (collision.transform.tag == "OffRoad")
        {
            SceneManager.LoadScene(1);
        }
    }
}

