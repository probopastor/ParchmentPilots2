using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneController : MonoBehaviour
{
    public LayerMask affectedLayer;

    public float thrustForce = 1f;
    public float gravity = 1f;
    private float xForce = 0f;
    private float yForce = 0f;

    private Rigidbody rb;
    public float yBoundary = -10f;
    private bool isThrown;
    public Scene currentScene;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        currentScene = SceneManager.GetActiveScene();
        isThrown = false;
        yForce = gravity;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y < yBoundary)
        {
            SceneManager.LoadScene(currentScene.buildIndex);
        }
        if (!isThrown)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                isThrown = true;
                xForce = thrustForce;
                rb.AddForce(transform.forward);
            }
        }
        else
        {
            rb.AddForce(transform.forward);


            // rb.AddForce(0, yForce, 0);

            //RaycastHit rayHit;
            /*
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, affectedLayer))
            {
                //rb.velocity += (rb.velocity - Vector3.Exclude(transform.forward, rb.velocity)) * Time.deltaTime / 1.25f;

                xForce += yForce * Time.deltaTime;
                rb.AddRelativeForce(transform.forward * xForce);
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayHit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }
            else
            {
                xForce -= yForce * Time.deltaTime;
                rb.AddRelativeForce(transform.forward * xForce);
                //rb.velocity -= Vector3.up * Time.deltaTime * 4.75f;
            }*/
        }
    }

    private void FixedUpdate()
    {
        if(isThrown)
        {
            float roll = Input.GetAxis("Horizontal");
            float tilt = Input.GetAxis("Vertical");

            float yaw = Input.GetAxis("Yaw") / 8;

            roll /= Time.timeScale;
            tilt /= Time.timeScale;
            yaw /= Time.timeScale;

            float tip = (transform.right + Vector3.up).magnitude - 1.414214f;
            yaw -= tip;

            if ((transform.forward + rb.velocity.normalized).magnitude < 1.4f)
            {
                tilt += 0.3f;
            }

            if (tilt != 0)
            {
                transform.Rotate(transform.right, tilt * Time.deltaTime * 40, Space.World);
            }
            if (roll != 0)
            {
                transform.Rotate(transform.forward, roll * Time.deltaTime * -40, Space.World);
            }
            if (yaw != 0)
            {
                transform.Rotate(Vector3.up, yaw * Time.deltaTime * 60, Space.World);
            }
        }
    }
}
