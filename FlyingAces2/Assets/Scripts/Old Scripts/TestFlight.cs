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

    [Tooltip("The planes center of mass at which torq should be applied")]
    public GameObject centerOfMassReference;

    [Tooltip("The strength of the force applied to the nose of the plane")]
    public Vector3 forceAtPos = new Vector3(0, 1, 0);

    [Tooltip("The time between each force applied to the nose of the plane. A smaller value makes upward tilt more difficult")]
    public int forceAppliedTimer = 10;

    [Header("Variables specific to plane type")]
    [Tooltip("The downward acceleration modifier for the planes depending on the angle of the plane")]
    public float angleAcceleration = 5f;

    [Tooltip("The rotational speed multiplier on the X axis")]
    public float tiltResponsivity = 40f;

    [Tooltip("The rotational speed multiplier on the Y axis")]
    public float rollResponsivity = -40f;

    [Tooltip("The rotational speed multiplier on the Z axis")]
    public float yawResponsivity = 60f;

    public float yForceAfterBounce = 100f;

    private int currentForceAppliedTimer = 0;
    private bool forceAppliedThisFrame;

    private float xForce = 0f;
    private float yForce = 0f;
    private float fall = 0f;

    private bool isThrown;
    private bool finished = false;
    private bool hitGround = false;

    private Rigidbody Rigidbody;
    private Animator anim;

    private Aiming aim;
    private ThrowingChargeBarController chargeBarController;

    private bool aiming = true;
    public bool throwing = false;

    private Vector3 launchSpeed = new Vector3(0, 0, 1000);
    private Vector3 strokePosition = new Vector3(0f, 0f, 0f);
    private Vector3 newTee = new Vector3();
    private Vector3 camStartPos;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = gameObject.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        aim = GetComponent<Aiming>();
        chargeBarController = GetComponent<ThrowingChargeBarController>();
        yForce = gravity;
        isThrown = false;
        startRot = gameObject.transform.rotation;
        camStartPos = planeCam.transform.localPosition;
        strokeText.text = "Stroke: " + stroke;
        chargeBarController.enabled = false;
        forceAppliedThisFrame = false;

        currentForceAppliedTimer = forceAppliedTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!isThrown)
            {
                if (!aiming && throwing)
                {
                    throwing = false;
                    isThrown = true;
                    anim.SetBool("isThrown", isThrown);
                    chargeBarController.chargeBar.gameObject.SetActive(false);
                    chargeBarController.enabled = false;
                    Rigidbody.isKinematic = false;
                    Rigidbody.useGravity = true;

                    Rigidbody.AddRelativeForce(Vector3.forward * thrustForce * chargeBarController.chargeBar.value);

                    stroke += 1;
                }
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

                if (downHit.collider != null)
                {
                    Debug.DrawRay(transform.position, Vector3.down * downHit.distance, Color.red);

                    Vector3 vector1 = rayHit.point - transform.position;
                    Vector3 vector2 = downHit.point - transform.position;
                    float angle = Mathf.Acos(Vector3.Dot(vector1.normalized, vector2.normalized));
                    print(angle * Mathf.Rad2Deg);

                    var negativeForward = (Rigidbody.velocity - Vector3.Exclude(transform.forward, Rigidbody.velocity));
                    if ((angle * Mathf.Rad2Deg) >= 1)
                    {
                        Rigidbody.velocity += negativeForward * Time.deltaTime * (angleAcceleration / (angle * Mathf.Rad2Deg));
                    }
                }
            }
            else
            {
                //if (!forceAppliedThisFrame)
                //{
                //    forceAppliedThisFrame = true;
                //    Rigidbody.AddForceAtPosition(forceAtPos, centerOfMassReference.transform.position);
                //}
                //else if (forceAppliedThisFrame)
                //{
                //    forceAppliedThisFrame = false;
                //}

                if(currentForceAppliedTimer <= 0)
                {
                    forceAppliedThisFrame = true;
                    Rigidbody.AddForceAtPosition(forceAtPos, centerOfMassReference.transform.position);
                    currentForceAppliedTimer = forceAppliedTimer;
                }
                else if(currentForceAppliedTimer > 0)
                {
                    currentForceAppliedTimer--;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Return) && aiming)
        {
            ChargeBar();
        }

        if ((Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace)) && throwing)
        {
            throwing = false;
            aiming = true;
            aim.enabled = true;
            chargeBarController.enabled = false;
            chargeBarController.chargeBar.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (isThrown)
        {
            float roll = Input.GetAxis("Horizontal");
            float tilt = Input.GetAxis("Vertical");

            float yaw = Input.GetAxis("Yaw") / 8;

            float tip = (transform.right + Vector3.up).magnitude - 1.414214f;
            yaw -= tip;

            Vector3 changes = new Vector3();

            if ((transform.forward + Rigidbody.velocity.normalized).magnitude < 1.4f)
            {
                tilt += 0.3f;
            }

            if (tilt != 0)
            {
                changes += transform.right * tilt * Time.deltaTime * tiltResponsivity;
            }
            if (roll != 0)
            {
                changes += transform.forward * roll * Time.deltaTime * rollResponsivity;
            }
            if (yaw != 0)
            {
                changes += transform.up * yaw * Time.deltaTime * yawResponsivity;
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
            print(Rigidbody.velocity + "Up Down " + Rigidbody.velocity.y + "forward " + forwardVel.magnitude);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            //Rigidbody.drag = 5f;
            anim.SetBool("Sliding", true);
        }

        if (collision.gameObject.tag == "Finish")
        {
            finished = true;
            SceneManager.LoadScene(nextSceneName);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            Rigidbody.AddForce(Vector3.down * yForceAfterBounce);
            anim.SetBool("Sliding", false);
            anim.SetBool("isThrown", isThrown);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground" && Rigidbody.velocity.z < 0.1f)
        {
            hitGround = true;
            anim.SetBool("Sliding", false);
            SetUpNewThrow(collision);

        }
    }

    void SetUpNewThrow(Collision collision)
    {
        if (!finished)
        {
            Rigidbody.useGravity = false;

            ContactPoint contact = collision.GetContact(0);
            newTee = contact.point;
            newTee.y += 15f;
            planeCam.transform.localPosition = camStartPos;
            strokeText.text = "Stroke: " + stroke;
            isThrown = false;
            aiming = true;
            anim.SetBool("isThrown", isThrown);
            aim.enabled = true;
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

    private void ChargeBar()
    {
        throwing = true;
        aiming = false;
        aim.enabled = false;
        chargeBarController.enabled = true;
        chargeBarController.chargeBar.gameObject.SetActive(true);
    }
}
