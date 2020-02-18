using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallActions : MonoBehaviour
{

    //Cache
    private Rigidbody rb;
    Vector3 Initialpos = Vector3.zero;
    Vector3 DistanceTraveled = Vector3.zero;
    [SerializeField] private Camera MainCamera = null;
    Vector3 MousePos = Vector3.zero;
    //Cache

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveBall();
    }

    void MoveBall()
    {
        //Better with Touch events instead of using mouse events for mobile usage.
        //I did not want to ruin the code view by adding more unneccesary code for the test.  

        //Nearplane approx. 7f distance to the ball.
        MousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 7f));

        if (Input.GetMouseButtonDown(0))
        {
            Initialpos = MousePos;
        }

        if (Input.GetMouseButton(0))
        {
            DistanceTraveled = MousePos - Initialpos;

            transform.position = Vector3.Slerp(transform.position,
                                          new Vector3(transform.position.x,
                                          transform.position.y,
                                          transform.position.z + DistanceTraveled.z),
                                          1f);
            Initialpos = MousePos;

        }

        transform.Translate(Vector3.left * Time.deltaTime * 5f);
        
        //Move camera here. Default x distance between camera and ball is set to 5f in the test scene.
        MainCamera.transform.position = new Vector3(transform.position.x + 5f, MainCamera.transform.position.y, MainCamera.transform.position.z);
    }

    //Normally i would do these in a different script but its such a small game. It suits better here. :)
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Floor")
        {
            //Reset the velocity and add same force to maintain the same momentum.
            rb.velocity = new Vector3(rb.velocity.x, 0,0);
            rb.AddForce(new Vector3(0, 525,0));
        }
        else if (collision.transform.tag == "Ring")
        {
            //Gameover - restart
            SceneManager.LoadScene(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Diamond")
        {
            //TODO: Add score
            Destroy(other.gameObject);
        }
    }
}
