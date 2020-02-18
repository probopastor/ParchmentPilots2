using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFlight : MonoBehaviour
{
    bool finished = false;
    public int stroke = 1;
    public TextMeshProUGUI strokeText;
    public LayerMask affectedRayCastLayer;
    float fall = 0f;
    public float thrustForce = 1f;
    public float gravity = -1f;
    private float xForce = 0f;
    private float yForce = 0f;
    bool isThrown;
    private Rigidbody Rigidbody;
    Vector3 launchSpeed = new Vector3(0, 0, 1000);
    private Quaternion startRot;
    public string sceneName;
    public Scene currentScene;
    public string nextSceneName;
    bool hitGround = false;
    public GameObject player;
    Vector3 strokePosition = new Vector3(0f, 0f, 0f);
    Vector3 newTee = new Vector3();
    public Camera planeCam;
    public Vector3 thrownCamPos;
    private Vector3 camStartPos;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = gameObject.GetComponent<Rigidbody>();
        yForce = gravity;
        isThrown = false;
        startRot = gameObject.transform.rotation;
        camStartPos = planeCam.transform.localPosition;
        strokeText.text = "Stroke: " + stroke;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Period))
        {
            Time.timeScale *= 2;
        }
        if(Input.GetKeyDown(KeyCode.Comma))
        {
            Time.timeScale /= 2;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!isThrown)
            {
                isThrown = true;

                GetComponent<AimScript>().enabled = false;
                Rigidbody.isKinematic = false;
                Rigidbody.AddRelativeForce(launchSpeed);
                stroke += 1;

            }
        }
        else if (isThrown)
        {
            RaycastHit rayHit;

            /*
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, affectedRayCastLayer))
            {
                Rigidbody.velocity += (Rigidbody.velocity - Vector3.Exclude(transform.forward, Rigidbody.velocity)) * Time.deltaTime / 1.25f;
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayHit.distance, Color.yellow);
            }
            else
            {
                Rigidbody.velocity -= Vector3.up * Time.deltaTime * 4.75f;
            }*/


            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, affectedRayCastLayer))
            {


                Rigidbody.velocity += (Rigidbody.velocity - Vector3.Exclude(transform.forward, Rigidbody.velocity)) * Time.deltaTime / 1.25f;
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayHit.distance, Color.yellow);

                RaycastHit downHit;

                Physics.Raycast(transform.position, Vector3.down, out downHit, Mathf.Infinity, affectedRayCastLayer);

                if(downHit.collider != null)
                {
                    Debug.DrawRay(transform.position, Vector3.down * downHit.distance, Color.red);

                    Vector3 vector1 = rayHit.point - transform.position;
                    Vector3 vector2 = downHit.point - transform.position;



                    float angle = Mathf.Acos(Vector3.Dot(vector1.normalized, vector2.normalized));
                    print(angle * Mathf.Rad2Deg);
                }
            }
        }
        //print(transform.forward);
    }

    private void FixedUpdate()
    {
        if (isThrown)
        {
            float roll = Input.GetAxis("Horizontal");
            float tilt = Input.GetAxis("Vertical");

            float yaw = Input.GetAxis("Yaw") / 8;

            roll /= Time.timeScale;
            tilt /= Time.timeScale;
            yaw /= Time.timeScale;

            float tip = (transform.right + Vector3.up).magnitude - 1.414214f;
            yaw -= tip;

            if ((transform.forward + Rigidbody.velocity.normalized).magnitude < 1.4f)
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


            if (Input.GetKey(KeyCode.Space))
            {
                Rigidbody.AddForce(transform.forward * Time.deltaTime * 1000);
            }


#pragma warning disable CS0618 // Type or member is obsolete
            Vector3 vertVel = Rigidbody.velocity - Vector3.Exclude(transform.up, Rigidbody.velocity);
#pragma warning restore CS0618 // Type or member is obsolete
            fall = vertVel.magnitude;
            Rigidbody.velocity -= vertVel * Time.deltaTime;
            Rigidbody.velocity += vertVel.magnitude * transform.forward * Time.deltaTime / 10;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            hitGround = true;
            SetUpNewThrow(collision);

        }

        if (collision.gameObject.tag == "Finish")
        {
            finished = true;
            SceneManager.LoadScene(nextSceneName);
          
        }
        
    }
    void SetUpNewThrow(Collision collision)
    {
        if (!finished)
        {
            ContactPoint contact = collision.GetContact(0);
            newTee = contact.point;
            newTee.y += 5f;
            planeCam.transform.localPosition = camStartPos;
            strokeText.text = "Stroke: " + stroke;
            isThrown = false;
            GetComponent<AimScript>().enabled = true;
            gameObject.transform.position = newTee;
            player.transform.position = newTee;
            if (hitGround)
            {
                gameObject.transform.rotation = startRot;
            }


            hitGround = false;

            Rigidbody.isKinematic = true;
        }

    }
}
