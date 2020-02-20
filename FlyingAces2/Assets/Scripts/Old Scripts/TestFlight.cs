using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFlight : MonoBehaviour
{
    #region variables
    [Tooltip("The camera object in the scene")]
    public Camera planeCam;

    [Tooltip("The plane object in the scene")]
    public GameObject player;

    [Tooltip("Number of strokes taken for the current level")]
    public int stroke = 1;

    [Tooltip("The text object that displays the stroke the player is on")]
    public TextMeshProUGUI strokeText;

    [Tooltip("The layer for the Raycasts to help with physics - Keep to ground only")]
    public LayerMask affectedRayCastLayer;

    [Tooltip("The scene the player is currently in")]
    public Scene currentScene;

    [Tooltip("The scene for the next level")]
    public string nextSceneName;

    [Tooltip("The force multiplyer for throwing")]
    public float thrustForce = 1000f;

    [Tooltip("Adjusts how much gravity affects the plane")]
    public float gravity = -1f;

    [Tooltip("The position the camera should be after the plane has been thrown")]
    public Vector3 thrownCamPos;

    public Quaternion startRot = new Quaternion();

    private float xForce = 0f;
    private float yForce = 0f;
    private float fall = 0f;

    private bool isThrown;
    private bool finished = false;
    private bool hitGround = false;

    private Rigidbody Rigidbody;  

    private Vector3 launchSpeed = new Vector3(0, 0, 1000);
    private Vector3 strokePosition = new Vector3(0f, 0f, 0f);
    private Vector3 newTee = new Vector3();
    private Vector3 camStartPos;
    #endregion

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
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!isThrown)
            {
                isThrown = true;

                GetComponent<Aiming>().enabled = false;
                Rigidbody.isKinematic = false;
                Rigidbody.useGravity = true;
                Rigidbody.AddRelativeForce(Vector3.forward * thrustForce);
                stroke += 1;

            }
        }
        else if (isThrown)
        {
            RaycastHit rayHit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, affectedRayCastLayer))
            { 
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayHit.distance, Color.yellow);

                RaycastHit downHit;

                Physics.Raycast(transform.position, Vector3.down, out downHit, Mathf.Infinity, affectedRayCastLayer);

                if(downHit.collider != null)
                {
                    Debug.DrawRay(transform.position, Vector3.down * downHit.distance, Color.red);

                    Vector3 vector1 = rayHit.point - transform.position;
                    Vector3 vector2 = downHit.point - transform.position;
                    float angle = Mathf.Acos(Vector3.Dot(vector1.normalized, vector2.normalized));
                    //print(angle * Mathf.Rad2Deg);

                    //var negativeForward = (Rigidbody.velocity - Vector3.Exclude(transform.forward, Rigidbody.velocity));

                    //Rigidbody.velocity += negativeForward * Time.deltaTime * (20 / (angle * Mathf.Rad2Deg));

                    if(vector2.magnitude < 1f && (angle * Mathf.Rad2Deg > 80 || angle * Mathf.Rad2Deg < 100))
                    {
                       Rigidbody.velocity += new Vector3(0, Rigidbody.velocity.x * (angle * Mathf.Rad2Deg / 100), 0);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isThrown)
        {
            float roll = Input.GetAxis("Horizontal");
            float tilt = Input.GetAxis("Vertical");

            float yaw = Input.GetAxis("Yaw") / 8;

            //roll /= Time.timeScale;
            //tilt /= Time.timeScale;
            //yaw /= Time.timeScale;

            float tip = (transform.right + Vector3.up).magnitude - 1.414214f;
            yaw -= tip;

            

            Vector3 changes = new Vector3();

            if ((transform.forward + Rigidbody.velocity.normalized).magnitude < 1.4f)
            {
                tilt += 0.3f;
            }

            if (tilt != 0)
            {
                changes += transform.right * tilt * Time.deltaTime * 40;
                //transform.Rotate(transform.right, tilt * Time.deltaTime * 40, Space.World);
            }
            if (roll != 0)
            {
                changes += transform.forward * roll * Time.deltaTime * -40;
                //transform.Rotate(transform.forward, roll * Time.deltaTime * -40, Space.World);
            }
            if (yaw != 0)
            {
                changes += transform.up * yaw * Time.deltaTime * 60;
                //transform.Rotate(Vector3.up, yaw * Time.deltaTime * 60, Space.World);
            }

            Quaternion newRotation = Quaternion.Euler(Rigidbody.rotation.eulerAngles + changes);

            Rigidbody.MoveRotation(newRotation);

#pragma warning disable CS0618 // Type or member is obsolete
            Vector3 vertVel = Rigidbody.velocity - Vector3.Exclude(transform.up, Rigidbody.velocity);
#pragma warning restore CS0618 // Type or member is obsolete
            fall = vertVel.magnitude;
            Rigidbody.velocity -= vertVel * Time.deltaTime;
            Rigidbody.velocity += vertVel.magnitude * transform.forward * Time.deltaTime / 10;


            var forwardVel = Rigidbody.velocity;
            forwardVel.y = 0;
            print(Rigidbody.velocity + "Up Down " + Rigidbody.velocity.y + "forward " + forwardVel.magnitude );
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
            GetComponent<Aiming>().enabled = true;
            gameObject.transform.position = newTee;
            player.transform.position = newTee;
            if (hitGround)
            {
                gameObject.transform.rotation = startRot;;
            }

            hitGround = false;

            Rigidbody.isKinematic = true;
        }

    }
}
