﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFlight : MonoBehaviour
{
    #region variables
    [Tooltip("The camera object in the scene")]
    public Camera planeCam;

    [Tooltip("The possible planes the player may choose")]
    public GameObject[] planeObjects;

    [Tooltip("The UI panel for choosing planes")]
    public GameObject planeSelectPanel;

    [Tooltip("The height at which the plane is thrown on the first throw of the level")]
    public float startThrowHeight = 200f;

    [Tooltip("The height at which the plane is thrown at the start of a new throw")]
    public float throwHeight = 15f;

    [Tooltip("Number of strokes taken for the current level")]
    public int stroke = 1;

    [Tooltip("The text object that displays the stroke the player is on")]
    public TextMeshProUGUI strokeText;

    [Tooltip("The layer for the Raycasts to help with physics - Keep to ground only")]
    public LayerMask affectedRayCastLayer;

    public LayerMask skyLayer;

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

    [Header("Variables specific to plane type")]
    [Tooltip("The downward acceleration modifier for the planes depending on the angle of the plane")]
    public float angleAcceleration = 5f;

    [Tooltip("The rotational speed multiplier on the X axis")]
    public float tiltResponsivity = 40f;

    [Tooltip("The rotational speed multiplier on the Y axis")]
    public float rollResponsivity = -40f;

    [Tooltip("The rotational speed multiplier on the Z axis")]
    public float yawResponsivity = 60f;

    [Tooltip("The rotational speed multiplier on the X axis when aim is rotating while aiming")]
    public float xAimResponsitivity = 40f;

    [Tooltip("The rotational speed multiplier on the Z axis when aim is rotating while aiming")]
    public float yAimResponsitivity = 40f;

    [Tooltip("The planes center of mass at which torq should be applied")]
    public GameObject centerOfMassReference;

    [Tooltip("The strength of the force applied to the nose of the plane")]
    public Vector3 forceAtPos = new Vector3(0, 1, 0);

    [Tooltip("The time between each force applied to the nose of the plane. A smaller value makes upward tilt more difficult")]
    public int forceAppliedTimer = 10;
    private int currentForceAppliedTimer = 0;
    private bool forceAppliedThisFrame;

    public AudioSource SoundEffectSource;
    public AudioSource LongSoundEffectSource;

    public AudioClip confettiSound;
    public AudioClip windSound;

    public float windPitchMax = 4f;
    public float windPitchMin = 0.1f;

    private bool inSlideMode;

    //bool decreasePitch;
    //bool increasePitch;
    //public float windPitchMax = 1f;
    //public float windPitchMin = 1f;
    //public float defaultWindPitch = 0f;
    //public float windPitchIncrementDown = 0.1f;
    //public float windPitchIncrementUp = 0.1f;

    private bool playOnce;

    private GameObject thisPlane;

    private float xForce = 0f;
    private float yForce = 0f;
    private float fall = 0f;

    private bool isThrown;
    private bool finished = false;
    private bool hitGround = false;

    private Rigidbody Rigidbody;
    private Animator anim;

    private ThrowingChargeBarController chargeBarController;

    private bool aiming = true;
    public bool throwing = false;
    private bool planeSelect = false;

    private Vector3 launchSpeed = new Vector3(0, 0, 1000);
    private Vector3 strokePosition = new Vector3(0f, 0f, 0f);
    private Vector3 newTee = new Vector3();
    private Vector3 camStartPos;
    #endregion

    public float increaseWindPitchRate = 10.0f;
    public float startWindPitchRate = 20f;
    public float hitGroundWindPitchRate = 100f;

    private PauseManager pauseManager;

    // Start is called before the first frame update
    void Start()
    {
        //decreasePitch = false;
        //increasePitch = false;

        //LongSoundEffectSource.pitch = defaultWindPitch;
        pauseManager = GameObject.FindObjectOfType<PauseManager>();

        Rigidbody = gameObject.GetComponent<Rigidbody>();

        increaseWindPitchRate = startWindPitchRate;

        playOnce = false;
        inSlideMode = false;
        //thisPlane = planeObjects[0];

        planeSelect = false;
        planeSelectPanel.SetActive(false);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, startThrowHeight, gameObject.transform.position.z);
        RotateTowardsFinish();
        anim = GetComponent<Animator>();
        chargeBarController = GetComponent<ThrowingChargeBarController>();
        yForce = gravity;
        isThrown = false;
        camStartPos = planeCam.transform.localPosition;
        strokeText.text = "Stroke: " + stroke;
        chargeBarController.enabled = false;
        forceAppliedThisFrame = false;

        currentForceAppliedTimer = forceAppliedTimer;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && !isThrown)
        {
            if (!aiming && throwing && !planeSelect)
            {
                throwing = false;
                isThrown = true;
                anim.SetBool("isThrown", isThrown);
                chargeBarController.chargeBar.gameObject.SetActive(false);
                chargeBarController.enabled = false;
                Rigidbody.isKinematic = false;
                Rigidbody.useGravity = true;

                aiming = false;
                Rigidbody.AddRelativeForce(Vector3.forward * thrustForce * chargeBarController.chargeBar.value);

                stroke += 1;
            }
        }
        else if(!isThrown && aiming)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                planeSelectPanel.SetActive(true);
                planeSelect = true;
                aiming = false;
            }
        }
        else if(!isThrown && !aiming && planeSelect)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                planeSelectPanel.SetActive(false);
                aiming = true;
                planeSelect = false;
            }
        }
        else if (isThrown)
        {
            RaycastHit rayHit;
            if(!playOnce)
            {
                LongSoundEffectSource.clip = windSound;
                LongSoundEffectSource.Play();
                //StartCoroutine("IncreaseAirPitch");
                playOnce = true;
            }

            //The small the plane's angle towards the ground, the faster the plane will accelerate downwards. First raycast points from nose of plane.
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, affectedRayCastLayer))
            {
                AngleAcceleration(rayHit);
                //decreasePitch = false;
                //increasePitch = true;
            }
            else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, skyLayer))
            {
                //Determines if force should be applied at the center of mass of the plane this frame when the plane does not face the ground
                ForceAtCenterOfMass();

                //decreasePitch = true;
                //increasePitch = false;
            }
            else
            {
                ForceAtCenterOfMass();
                //decreasePitch = false;
                //increasePitch = false;
            }
        }

        if((isThrown || inSlideMode) && !pauseManager.isPaused)
        {
            if(inSlideMode)
            {
                increaseWindPitchRate = 100f;
            }

            var velSound = Rigidbody.velocity.magnitude / increaseWindPitchRate;

            LongSoundEffectSource.pitch = Mathf.Clamp(velSound, windPitchMin, windPitchMax);
        }
        else if(pauseManager.isPaused)
        {
            LongSoundEffectSource.Stop();
            playOnce = false;
        }

        if (Input.GetKeyUp(KeyCode.Return) && aiming)
        {
            ChargeBar();
        }

        if ((Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace)) && throwing)
        {
            throwing = false;
            aiming = true;
            chargeBarController.enabled = false;
            chargeBarController.chargeBar.gameObject.SetActive(false);
        }

        AimLogic();

    }

    private void FixedUpdate()
    {
        MovePlane();    
    }

    public void SelectPlane()
    {

        //planeObjects[i]
    }

    /// <summary>
    /// Allows the player to move the plane once it is thrown.
    /// </summary>
    private void MovePlane()
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
        }
    }

    /// <summary>
    /// Determines the angle between the plane and the ground when plane flies towards the ground.
    /// The smaller this angle, the faster the plane will accelerate towards the ground.
    /// </summary>
    /// <param name="rayHit"></param>
    private void AngleAcceleration(RaycastHit rayHit)
    {
        RaycastHit downHit;

        //Raycast points down from the plane to get the angle towards the ground.
        Physics.Raycast(transform.position, Vector3.down, out downHit, Mathf.Infinity, affectedRayCastLayer);

        //The angle between the forward and down raycasts are calculated to determine the angle of the plane's flight.
        if (downHit.collider != null)
        {
            Vector3 vector1 = rayHit.point - transform.position;
            Vector3 vector2 = downHit.point - transform.position;
            float angle = Mathf.Acos(Vector3.Dot(vector1.normalized, vector2.normalized));
            var negativeForward = (Rigidbody.velocity - Vector3.Exclude(transform.forward, Rigidbody.velocity));

            //Velocity is added to the plane the sharper its angle towards the ground.
            if ((angle * Mathf.Rad2Deg) >= 1)
            {
                Rigidbody.velocity += negativeForward * Time.deltaTime * (angleAcceleration / (angle * Mathf.Rad2Deg));
            }
        }
    }

    
    //private IEnumerator IncreaseAirPitch()
    //{
    //    Debug.Log("Increased 1: " + LongSoundEffectSource.pitch);

    //    if(increasePitch && !decreasePitch)
    //    {
    //        Debug.Log("Increased 2: ");

    //        if (LongSoundEffectSource.pitch > windPitchMax)
    //        {
    //            LongSoundEffectSource.pitch = windPitchMax;

    //            Debug.Log("Increased 3: " + LongSoundEffectSource.pitch);
    //        }
    //        else
    //        {                
    //            LongSoundEffectSource.pitch += windPitchIncrementDown;

    //            Debug.Log("Increased 4: " + LongSoundEffectSource.pitch);
    //        }

    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine("IncreaseAirPitch");
    //    }
    //    else if(decreasePitch && !increasePitch)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine("DecreaseAirPitch");
    //    }
    //    else if((decreasePitch && increasePitch) || (!decreasePitch && !increasePitch))
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine("StabalizeAirPitch");
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(0.01f);
    //        StartCoroutine("IncreaseAirPitch");
    //    }
    //}

    //private IEnumerator StabalizeAirPitch()
    //{
    //    Debug.Log("Stabalize 1: " + LongSoundEffectSource.pitch);

    //    if ((!decreasePitch && !increasePitch) || (decreasePitch && increasePitch))
    //    {
    //        if ((LongSoundEffectSource.pitch > defaultWindPitch) && (LongSoundEffectSource.pitch >= (defaultWindPitch - 0.3f)) && (LongSoundEffectSource.pitch <= (defaultWindPitch + 0.3f)))
    //        {
    //            Debug.Log("Stabalize 2: " + LongSoundEffectSource.pitch);

    //            LongSoundEffectSource.pitch -= windPitchIncrementDown;
    //            yield return new WaitForSeconds(0.1f);
    //            StartCoroutine("StabalizeAirPitch");
    //        }
    //        else if((LongSoundEffectSource.pitch < defaultWindPitch) && (LongSoundEffectSource.pitch >= (defaultWindPitch - 0.3f)) && (LongSoundEffectSource.pitch <= (defaultWindPitch + 0.3f)))
    //        {
    //            Debug.Log("Stabalize 3: " + LongSoundEffectSource.pitch);
    //            LongSoundEffectSource.pitch += windPitchIncrementUp;
    //            yield return new WaitForSeconds(0.1f);
    //            StartCoroutine("StabalizeAirPitch");
    //        }

    //        yield return new WaitForSeconds(0.01f);
    //        StartCoroutine("StabalizeAirPitch");
    //    }
    //    else if(increasePitch && !decreasePitch)
    //    {
    //        yield return new WaitForSeconds(0.01f);
    //        StartCoroutine("IncreaseAirPitch");
    //    }
    //    else if(!increasePitch && decreasePitch)
    //    {
    //        yield return new WaitForSeconds(0.01f);
    //        StartCoroutine("DecreaseAirPitch");
    //    }
    //}

    //private IEnumerator DecreaseAirPitch()
    //{
    //    if (decreasePitch)
    //    {
    //        if (LongSoundEffectSource.pitch < windPitchMin)
    //        {
    //            LongSoundEffectSource.pitch = windPitchMin;
    //        }
    //        else
    //        {
    //            LongSoundEffectSource.pitch -= windPitchIncrementUp;

    //        }

    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine("DecreaseAirPitch");
    //    }
    //    else if (!decreasePitch && increasePitch)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine("IncreaseAirPitch");
    //    }
    //    else if ((decreasePitch && increasePitch) || (!decreasePitch && !increasePitch))
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine("StabalizeAirPitch");
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(0.01f);
    //        StartCoroutine("DecreaseAirPitch");
    //    }
    //}

    /// <summary>
    /// Applies a force at the plane's center of mass on frames where currentForceappliedTimer is less than 1.
    /// As a result, the plane will slowly tilt towards the ground. 
    /// </summary>
    private void ForceAtCenterOfMass()
    {
        if (currentForceAppliedTimer <= 0)
        {
            forceAppliedThisFrame = true;
            Rigidbody.AddForceAtPosition(forceAtPos, centerOfMassReference.transform.position);
            currentForceAppliedTimer = forceAppliedTimer;
        }
        else if (currentForceAppliedTimer > 0)
        {
            currentForceAppliedTimer--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            increaseWindPitchRate = hitGroundWindPitchRate;
            inSlideMode = true;
            //decreasePitch = false;
            //increasePitch = false;
            anim.SetBool("Sliding", true);
        }

        if (collision.gameObject.tag == "Finish")
        {
            finished = true;
            LongSoundEffectSource.Stop();
            StartCoroutine("WinHandler");
            
        }
    }

    private IEnumerator WinHandler()
    {
        SoundEffectSource.clip = confettiSound;
        SoundEffectSource.Play();
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground" && Rigidbody.velocity.z < 0.1f)
        {
            hitGround = true;
            inSlideMode = false;
            increaseWindPitchRate = startWindPitchRate;
            LongSoundEffectSource.Stop();
            anim.SetBool("Sliding", false);
            playOnce = false;
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
            newTee.y = throwHeight;
            planeCam.transform.localPosition = camStartPos;
            strokeText.text = "Stroke: " + stroke;
            isThrown = false;
            aiming = true;
            anim.SetBool("isThrown", isThrown);
            gameObject.transform.position = newTee;

            if (hitGround)
            {
                RotateTowardsFinish();
            }

            hitGround = false;

            Rigidbody.isKinematic = true;
        }
    }

    /// <summary>
    /// Sets the charge bar to active when the plane is being thrown.
    /// </summary>
    private void ChargeBar()
    {
        throwing = true;
        aiming = false;
        chargeBarController.enabled = true;
        chargeBarController.chargeBar.gameObject.SetActive(true);
    }

    void AimLogic()
    {
        if(aiming)
        {
            float xRotationValue = Input.GetAxis("Horizontal");
            float yRotationValue = Input.GetAxis("Vertical");

            Vector3 rotationChanges = new Vector3();

            if (xRotationValue != 0)
            {
                rotationChanges += new Vector3(0 , (xRotationValue * Time.deltaTime * xAimResponsitivity), 0);
            }
            if (yRotationValue != 0)
            {
                rotationChanges += new Vector3((yRotationValue * Time.deltaTime * yAimResponsitivity), 0, 0);
            }

            Quaternion newRotation = Quaternion.Euler(Rigidbody.rotation.eulerAngles + rotationChanges);
            Rigidbody.MoveRotation(newRotation);
        }
        
    }

    /// <summary>
    /// Rotates the plane towards the Game Object tagged as "Finish".
    /// </summary>
    private void RotateTowardsFinish()
    {
        GameObject finishObj = GameObject.FindWithTag("Finish");

        Vector3 finishDirection = finishObj.transform.position - gameObject.transform.position;

        Quaternion rotation = Quaternion.LookRotation(new Vector3(finishDirection.x, 0, finishDirection.z));

        transform.rotation = rotation;
    }
}
