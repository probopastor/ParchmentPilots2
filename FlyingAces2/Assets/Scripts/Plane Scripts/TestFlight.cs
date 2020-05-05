﻿/*
* Author: William Nomikos, Grant Frey
* TestFlight.cs
* Handles plane throwing, plane re-throwing, moving the plane, plane gravity, 
* audio source pitch increasing with velocity, particles changing with
* velocity, score text, rotating the plane towards the flag on rethrow, 
* the tutorial, and winning the level.
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Cinemachine;

public class TestFlight : MonoBehaviour
{
    #region variables
    [Tooltip("The camera object in the scene")]
    public Camera planeCam;

    [Tooltip("The height at which the plane is thrown on the first throw of the level")]
    public float startThrowHeight = 200f;

    [Tooltip("The height at which the plane is thrown at the start of a new throw")]
    public float throwHeight = 15f;

    [Tooltip("Number of strokes to score a par")]
    public int par = 0;

    [Tooltip("The hole the player is currently on")]
    public int hole = 0;

    [Tooltip("The force that the wind zones put on the plane")]
    public float windZoneForce;

    [Tooltip("The text object that displays the stroke the player is on")]
    public TextMeshProUGUI strokeText;

    [Tooltip("The text object that displays the par for the course")]
    public TextMeshProUGUI parText;

    [Tooltip("The text object that displays the hole the player is on")]
    public TextMeshProUGUI holeText;

    [Tooltip("The text that displays what score the player got on the hole")]
    public TextMeshProUGUI scoreText;

    [Tooltip("The notecard the score text is displayed on")]
    public GameObject scoreCard;

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

    public AudioSource MusicSource;
    public AudioSource SoundEffectSource;
    public AudioSource LongSoundEffectSource;
    public AudioSource SinglePitchSoundEffectSource;
    public AudioSource TrumpetSource;

    private float soundEffectSourceVolume = 1f;

    public AudioClip winFanfare;
    public AudioClip winHorn;
    public AudioClip windSound;
    public AudioClip crumbleSound;
    public AudioClip hitSound;

    public float windPitchMax = 4f;
    public float windPitchMin = 0.1f;

    private bool inSlideMode;

    private bool playOnce;
    private bool playCrumbleOnce;
    private bool playWinSoundOnce;

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

    private Vector3 launchSpeed = new Vector3(0, 0, 1000);
    private Vector3 strokePosition = new Vector3(0f, 0f, 0f);
    private Vector3 newTee = new Vector3();
    private Vector3 camStartPos;
    private Vector3 windAngle;

    public float increaseWindPitchRate = 10.0f;
    public float increaseCrumblePitchRate = 10.0f;
    public float startWindPitchRate = 20f;
    public float hitGroundWindPitchRate = 100f;

    private PauseManager pauseManager;
    private EventSystem eventSystem;

    public float windEmmissionRate = 10f;

    [Tooltip("The left wing's air particles.")]
    public ParticleSystem leftSystem;

    [Tooltip("The right wing's air particles.")]
    public ParticleSystem rightSystem;

    [Tooltip("The victory particles")]
    public ParticleSystem winSystem;

    private ParticleSystem.MainModule leftMain;
    private ParticleSystem.MainModule rightMain;

    ParticleSystem.EmissionModule emissionModuleLeft;
    ParticleSystem.EmissionModule emissionModuleRight;

    [Tooltip("The rate at which the wing particle's color and albedo change with velocity")]
    public float increaseParticleColorSwitchRate = 10.0f;

    [Tooltip("Default wing aircurrent particle color")]
    public Color particleColor;

    [Tooltip("Wing aircurrent particle color at the min velocity")]
    public Color particleColorMinSpeed;

    [Tooltip("Wing aircurrent particle color at the max velocity")]
    public Color particleColorMaxSpeed;

    [Tooltip("The rate at which the wing particle's lifespan change with velocity")]
    public float increaseParticleLifeTimeRate = 10.0f;

    [Tooltip("Wing aircurrent particle lifetime at the min velocity")]
    public float minLifetime = 0.1f;

    [Tooltip("Wing aircurrent particle lifetime at the max velocity")]
    public float maxLifetime = 0.5f;

    [Tooltip("The rate at which the wing particle's size change with velocity")]
    public float increaseParticleSizeRate = 10.0f;

    [Tooltip("Wing aircurrent particle size at the min velocity")]
    public float minSize = 0f;

    [Tooltip("Wing aircurrent particle size at the max velocity")]
    public float maxSize = 1f;

    public bool isLevel1;

    private bool scoreAddedThisThrow;

    private ScoreManager scoreManager;

    private bool canEnableChargeBar;

    private bool moveForward;

    private float thisAngle = 0;

    bool checkForPlaneDeceleration;
    bool firstDecelerationCheck;

    public float RadiusToCheckOnRethrow = 1f;
    private Transform closestObject;

    public LayerMask radiusCheckLayers;

    public float xBuffer = 1f;
    public float zBuffer = 1f;

    private bool movePlaneToPos;

    public Animator transitionAnimator;

    #endregion

    void OnEnable()
    {
        pauseManager = GameObject.FindObjectOfType<PauseManager>();
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        movePlaneToPos = false;
        moveForward = false;
        pauseManager = GameObject.FindObjectOfType<PauseManager>();

        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        Rigidbody = gameObject.GetComponent<Rigidbody>();

        eventSystem = EventSystem.current;

        increaseWindPitchRate = startWindPitchRate;
        soundEffectSourceVolume = SoundEffectSource.volume;

        playOnce = false;
        playCrumbleOnce = false;
        inSlideMode = false;
        playWinSoundOnce = false;

        scoreAddedThisThrow = false;

        gameObject.transform.position = new Vector3(gameObject.transform.position.x, startThrowHeight, gameObject.transform.position.z);
        RotateTowardsFinish();
        anim = GetComponent<Animator>();
        chargeBarController = GetComponent<ThrowingChargeBarController>();
        yForce = gravity;
        isThrown = false;
        camStartPos = planeCam.transform.localPosition;
        strokeText.text = scoreManager.stroke.ToString();
        parText.text = par.ToString();
        holeText.text = hole.ToString();
        chargeBarController.enabled = false;
        forceAppliedThisFrame = false;
        scoreCard.SetActive(false);

        currentForceAppliedTimer = forceAppliedTimer;

        leftMain = leftSystem.main;
        rightMain = rightSystem.main;

        emissionModuleLeft = leftSystem.emission;
        emissionModuleRight = rightSystem.emission;

        emissionModuleLeft.rateOverDistance = 0f;
        emissionModuleRight.rateOverDistance = 0f;

        canEnableChargeBar = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkForPlaneDeceleration && isThrown)
        {
            checkForPlaneDeceleration = true;
            StartCoroutine(CheckForPlaneDeceleration());
        }

        if (pauseManager.isPaused)
        {
            canEnableChargeBar = false;
            StartCoroutine(DelayAfterGameUnpause());
        }


        if (isLevel1)
        {
            TutorialHandler();
        }
        else if (!isLevel1)
        {
            pauseManager.tutorialChargingObject.SetActive(false);
            pauseManager.tutorialAimObject.SetActive(false);
            pauseManager.tutorialThrowingObject.SetActive(false);
            pauseManager.tutorialFlyingObject.SetActive(false);
        }

        if (!isThrown)
        {
            leftSystem.Stop();
            rightSystem.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Return) && !isThrown)
        {
            if (!aiming && throwing && !pauseManager.isPaused)
            {
                throwing = false;
                isThrown = true;
                anim.SetBool("isThrown", isThrown);
                chargeBarController.chargeBar.gameObject.SetActive(false);
                chargeBarController.enabled = false;
                Rigidbody.isKinematic = false;
                Rigidbody.useGravity = true;

                aiming = false;

                scoreAddedThisThrow = false;
                Rigidbody.AddRelativeForce(Vector3.forward * thrustForce * chargeBarController.chargeBar.value);
                moveForward = true;
            }
        }
        else if (isThrown)
        {
            emissionModuleLeft.rateOverDistance = windEmmissionRate;
            emissionModuleRight.rateOverDistance = windEmmissionRate;

            RaycastHit rayHit;
            if (!playOnce)
            {
                LongSoundEffectSource.clip = windSound;
                LongSoundEffectSource.Play();
                leftSystem.Play();
                rightSystem.Play();

                playOnce = true;
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, skyLayer))
            {
                AngleBetweenGround(rayHit);

                if (thisAngle > 4 && thisAngle < 5f)
                {
                    moveForward = false;
                }

                Debug.Log(thisAngle);
            }

            //The smaller the plane's angle towards the ground, the faster the plane will accelerate downwards. First raycast points from nose of plane.
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, affectedRayCastLayer))
            {
                AngleAcceleration(rayHit);
            }
        }

        if ((isThrown || inSlideMode) && !pauseManager.isPaused)
        {
            if (inSlideMode && !playCrumbleOnce)
            {
                increaseWindPitchRate = 100f;
                SoundEffectSource.clip = crumbleSound;
                SoundEffectSource.Play();
                SoundEffectSource.loop = true;
                playCrumbleOnce = true;
            }

            var velSound = Rigidbody.velocity.magnitude / increaseWindPitchRate;
            LongSoundEffectSource.pitch = Mathf.Clamp(velSound, windPitchMin, windPitchMax);

            var crumbleSoundVelocity = Rigidbody.velocity.magnitude / increaseCrumblePitchRate;
            SoundEffectSource.volume = Mathf.Clamp(crumbleSoundVelocity, windPitchMin, windPitchMax);

            LongSoundEffectSource.pitch = Mathf.Clamp(velSound, windPitchMin, windPitchMax);

            WindParticleHandler();
        }
        else if (pauseManager.isPaused)
        {
            LongSoundEffectSource.Stop();
            SoundEffectSource.loop = false;

            SoundEffectSource.volume = soundEffectSourceVolume;

            playOnce = false;
            playCrumbleOnce = false;
        }
        else if (!isThrown && !inSlideMode)
        {
            SoundEffectSource.volume = soundEffectSourceVolume;
            SoundEffectSource.loop = false;
            SoundEffectSource.Stop();
        }


        if (Input.GetKeyDown(KeyCode.Return) && aiming && !throwing && canEnableChargeBar && !pauseManager.isPaused)
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

        OutOfBoundsCheck();

        if (movePlaneToPos)
        {
            movePlaneToPos = false;

            float xMove = 0;
            float zMove = 0;

            float xCheckHit = 0;
            float zCheckHit = 0;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.right, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                xMove = -xBuffer;
                xCheckHit++;
            }

            if (Physics.Raycast(transform.position, Vector3.left, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                xMove = xBuffer;
                xCheckHit++;
            }

            if (Physics.Raycast(transform.position, Vector3.forward, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                zMove = -zBuffer;
                zCheckHit++;
            }

            if (Physics.Raycast(transform.position, -Vector3.forward, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                zMove = zBuffer;
                zCheckHit++;
            }

            if(xCheckHit > 1)
            {
                xMove = 0;
            }

            if(zCheckHit > 1)
            {
                zMove = 0;
            }

            Debug.Log("xMove: " + xMove + " zMove: " + zMove);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + xMove, throwHeight, gameObject.transform.position.z + zMove);
            RotateTowardsFinish();
        }
    }

    private void FixedUpdate()
    {
        AimLogic();
        MovePlane();

        if (isThrown)
        {
            if (!pauseManager.isPaused)
            {
                ForceAtCenterOfMass();
            }
        }
    }

    private IEnumerator CheckForPlaneDeceleration()
    {
        //Debug.Log("x Vel: " + Rigidbody.velocity.x + " | " + "z Vel: " + Rigidbody.velocity.z);

        if (!firstDecelerationCheck)
        {
            firstDecelerationCheck = true;
            yield return new WaitForSeconds(2);
        }

        if ((Rigidbody.velocity.x < 10f && Rigidbody.velocity.x > -10f) && (Rigidbody.velocity.z < 10f && Rigidbody.velocity.z > -10f) && Rigidbody.velocity.y < 1f)
        {
            moveForward = false;
        }

        checkForPlaneDeceleration = false;
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Sets canEnableChargeBar to true after fixed update so that when a player exits the pause
    /// menu, the charge bar does not immediatly enable itself.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayAfterGameUnpause()
    {
        yield return new WaitForFixedUpdate();
        canEnableChargeBar = true;
    }

    /// <summary>
    /// Alters wind particle appearance in color, size, and lifetime based on plane velocity.
    /// </summary>
    private void WindParticleHandler()
    {
        var particleColorChange = Rigidbody.velocity.magnitude / increaseParticleColorSwitchRate;
        leftMain.startColor = new Color((Mathf.Clamp(particleColorChange, particleColorMinSpeed.r, particleColorMaxSpeed.r)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.g, particleColorMaxSpeed.g)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.b, particleColorMaxSpeed.b)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.a, particleColorMaxSpeed.a)));
        rightMain.startColor = new Color((Mathf.Clamp(particleColorChange, particleColorMinSpeed.r, particleColorMaxSpeed.r)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.g, particleColorMaxSpeed.g)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.b, particleColorMaxSpeed.b)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.a, particleColorMaxSpeed.a)));

        var particleLifetimeChange = Rigidbody.velocity.magnitude / increaseParticleLifeTimeRate;
        leftMain.startLifetime = Mathf.Clamp(particleLifetimeChange, minLifetime, maxLifetime);
        rightMain.startLifetime = Mathf.Clamp(particleLifetimeChange, minLifetime, maxLifetime);

        var particleSizeChange = Rigidbody.velocity.magnitude / increaseParticleSizeRate;
        leftMain.startSize = Mathf.Clamp(particleSizeChange, minSize, maxSize);
        rightMain.startSize = Mathf.Clamp(particleSizeChange, minSize, maxSize);
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

            if (OptionsController.invertedVerticalControls)
            {
                tilt = -Input.GetAxis("Vertical");
            }
            else
            {
                tilt = Input.GetAxis("Vertical");
            }

            if (OptionsController.invertedVerticalControls)
            {
                roll = -Input.GetAxis("Horizontal");
            }
            else
            {
                roll = Input.GetAxis("Horizontal");
            }

            if (transform.rotation.eulerAngles.x > 85f && transform.rotation.eulerAngles.x < 95f && tilt > 0)
            {
                tilt = 0;
            }
            if (transform.rotation.eulerAngles.x < 275f && transform.rotation.eulerAngles.x > 260f && tilt < 0)
            {
                tilt = 0;
            }
            Vector3 changes = new Vector3();
            Vector3 rollChanges = new Vector3();

            if ((transform.worldToLocalMatrix.MultiplyVector(transform.forward) + Rigidbody.velocity.normalized).magnitude < 1.4f)
            {
                tilt += 0.3f;
            }

            if (tilt != 0)
            {
                changes += transform.worldToLocalMatrix.MultiplyVector(transform.right) * tilt * Time.deltaTime * tiltResponsivity;
            }
            if (roll != 0)
            {
                //changes += transform.worldToLocalMatrix.MultiplyVector(transform.forward) * roll * Time.deltaTime * rollResponsivity;
                rollChanges += transform.worldToLocalMatrix.MultiplyVector(transform.forward) * roll * Time.deltaTime * rollResponsivity;
            }
            if (yaw != 0)
            {
                changes += transform.worldToLocalMatrix.MultiplyVector(transform.up) * yaw * Time.deltaTime * yawResponsivity;
            }

            Quaternion newRotation = Quaternion.Euler(Rigidbody.rotation.eulerAngles + changes);

            Rigidbody.MoveRotation(newRotation);
            Rigidbody.AddRelativeTorque(rollChanges);

#pragma warning disable CS0618 // Type or member is obsolete
            Vector3 vertVel = Rigidbody.velocity - Vector3.Exclude(transform.up, Rigidbody.velocity);
#pragma warning restore CS0618 // Type or member is obsolete
            fall = vertVel.magnitude;
            Rigidbody.velocity -= vertVel * Time.deltaTime;
            Rigidbody.velocity += vertVel.magnitude * transform.worldToLocalMatrix.MultiplyVector(transform.forward) * Time.deltaTime / 10;

            var forwardVel = Rigidbody.velocity;
            forwardVel.y = 0;

            //if (Input.GetKey(KeyCode.A))
            //{
            //    Rigidbody.AddRelativeTorque(Vector3.down);
            //}
            //else if (Input.GetKey(KeyCode.D))
            //{
            //    Rigidbody.AddRelativeTorque(Vector3.up);
            //}

            if (moveForward)
            {
                //if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                //{
                //}

                Vector3 newForward = transform.forward * Rigidbody.velocity.magnitude;
                //newForward.y = Rigidbody.velocity.y;
                //if(newForward.y < -200f)
                //{
                //    newForward.y = -200f;
                //}

                Rigidbody.velocity = newForward;
            }
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

    private void AngleBetweenGround(RaycastHit rayHit)
    {
        RaycastHit downHit;

        //Raycast points down from the plane to get the angle towards the ground.
        Physics.Raycast(transform.position, Vector3.up, out downHit, Mathf.Infinity, skyLayer);

        if (downHit.collider != null)
        {
            Vector3 vector1 = rayHit.point - transform.position;
            Vector3 vector2 = downHit.point - transform.position;
            float angle = Mathf.Acos(Vector3.Dot(vector1.normalized, vector2.normalized));
            thisAngle = angle * Mathf.Rad2Deg;

            //if ((angle * Mathf.Rad2Deg) >= 1)
            //{
            //    thisAngle = angle;
            //}
        }
    }

    /// <summary>
    /// Applies a force at the plane's center of mass on frames where currentForceappliedTimer is less than 1.
    /// As a result, the plane will slowly tilt towards the ground. 
    /// </summary>
    private void ForceAtCenterOfMass()
    {
        forceAppliedThisFrame = true;
        Rigidbody.AddForceAtPosition(forceAtPos, centerOfMassReference.transform.position);
        currentForceAppliedTimer = forceAppliedTimer;

        if (currentForceAppliedTimer <= 0)
        {
            //forceAppliedThisFrame = true;
            //Rigidbody.AddForceAtPosition(forceAtPos, centerOfMassReference.transform.position);
            //currentForceAppliedTimer = forceAppliedTimer;
        }
        else if (currentForceAppliedTimer > 0)
        {
            currentForceAppliedTimer--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        moveForward = false;

        SinglePitchSoundEffectSource.clip = hitSound;
        SinglePitchSoundEffectSource.Play();

        if (collision.gameObject.tag == "ground")
        {
            increaseWindPitchRate = hitGroundWindPitchRate;
            inSlideMode = true;

            emissionModuleLeft.rateOverDistance = 0f;
            emissionModuleRight.rateOverDistance = 0f;

            leftSystem.Pause();
            rightSystem.Pause();

            anim.SetBool("Sliding", true);
        }

        if (collision.gameObject.tag == "Finish" && !finished)
        {
            int scoreStroke = scoreManager.stroke;

            strokeText.text = scoreManager.stroke.ToString();
            LongSoundEffectSource.Stop();
            StartCoroutine("WinHandler");

            scoreCard.SetActive(true);

            if (scoreStroke == 1)
            {
                scoreText.text = "Hole in one! Smooth flying!";
            }
            else if (scoreStroke == par - 1)
            {
                scoreText.text = "You got a birdie! Nice!";
            }
            else if (scoreStroke == par - 2)
            {
                scoreText.text = "You got an eagle! Good job!";
            }
            else if (scoreStroke == par)
            {
                scoreText.text = "You got a par!";
            }
            else if (scoreStroke == par - 3)
            {
                scoreText.text = "You got an albatross! Amazing!";
            }
            else if (scoreStroke == par + 1)
            {
                scoreText.text = "You got a bogey. So close...";
            }
            else if (scoreStroke == par + 2)
            {
                scoreText.text = "Double bogey. Better luck next time.";
            }
            else if (scoreStroke == par + 3)
            {
                scoreText.text = "Triple bogey. Next hole will be better.";
            }
            else if (scoreStroke > par + 4)
            {
                scoreText.text = "Let's not talk about that hole...";
            }

            finished = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            SoundEffectSource.Stop();

            increaseWindPitchRate = startWindPitchRate;
            inSlideMode = false;

            emissionModuleLeft.rateOverDistance = windEmmissionRate;
            emissionModuleRight.rateOverDistance = windEmmissionRate;

            leftSystem.Play();
            rightSystem.Play();

            anim.SetBool("Sliding", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "WindArea")
        {
            windAngle = other.gameObject.transform.parent.transform.position - transform.position;
            windAngle = windAngle.normalized;
            Rigidbody.AddForce(-windAngle * windZoneForce);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WindArea")
        {
            moveForward = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WindArea")
        {
            moveForward = true;
        }
    }

    private IEnumerator WinHandler()
    {
        if (!finished)
        {
            if (!playWinSoundOnce)
            {
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                Rigidbody.constraints = RigidbodyConstraints.FreezePosition;

                MusicSource.clip = winFanfare;
                MusicSource.Play();
                yield return new WaitForSeconds(6f);
                MusicSource.Stop();

                winSystem.Play();
                TrumpetSource.clip = winHorn;
                TrumpetSource.Play();

                yield return new WaitForSeconds(1f);
                playWinSoundOnce = true;
                StartCoroutine(LevelLoad(nextSceneName));
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground" && (Rigidbody.velocity.x < 0.1f && Rigidbody.velocity.y < 0.1f && Rigidbody.velocity.z < 0.1f))
        {
            if (!scoreAddedThisThrow)
            {
                scoreManager.stroke += 1;
                scoreAddedThisThrow = true;
            }

            hitGround = true;
            inSlideMode = false;
            increaseWindPitchRate = startWindPitchRate;
            LongSoundEffectSource.Stop();
            anim.SetBool("Sliding", false);
            playOnce = false;
            playCrumbleOnce = false;
            SetUpNewThrow(collision);
        }
    }

    void SetUpNewThrow(Collision collision)
    {
        if (!finished)
        {
            checkForPlaneDeceleration = false;
            firstDecelerationCheck = false;
            moveForward = false;
            Rigidbody.useGravity = false;
            leftSystem.Play();
            rightSystem.Play();

            ContactPoint contact = collision.GetContact(0);
            newTee = contact.point;
            newTee.y = throwHeight;
            strokeText.text = scoreManager.stroke.ToString();
            isThrown = false;
            aiming = true;
            planeCam.transform.localPosition = camStartPos;
            anim.SetBool("isThrown", isThrown);
            gameObject.transform.position = newTee;

            if (hitGround)
            {
                //RotateTowardsFinish();
                movePlaneToPos = true;
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
        //TutorialHandler();

        throwing = true;
        aiming = false;
        chargeBarController.enabled = true;
        chargeBarController.chargeBar.gameObject.SetActive(true);
    }

    void AimLogic()
    {
        if (aiming)
        {
            float xRotationValue;
            float yRotationValue;
            if (OptionsController.invertedVerticalControls)
            {
                yRotationValue = -Input.GetAxis("Vertical");
            }
            else
            {
                yRotationValue = Input.GetAxis("Vertical");
            }

            if (OptionsController.invertedVerticalControls)
            {
                xRotationValue = -Input.GetAxis("Horizontal");
            }
            else
            {
                xRotationValue = Input.GetAxis("Horizontal");
            }

                if (transform.rotation.eulerAngles.x > 85f && transform.rotation.eulerAngles.x < 95f && yRotationValue > 0)
            {
                yRotationValue = 0;
            }
            if (transform.rotation.eulerAngles.x < 275f && transform.rotation.eulerAngles.x > 260f && yRotationValue < 0)
            {
                yRotationValue = 0;
            }
            Vector3 rotationChanges = new Vector3();

            if (xRotationValue != 0)
            {
                rotationChanges += new Vector3(0, (xRotationValue * Time.deltaTime * xAimResponsitivity), 0);
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

    /// <summary>
    /// If player manages to get out of bounds of the map,
    /// reload the current level.
    /// </summary>
    private void OutOfBoundsCheck()
    {
        if (gameObject.transform.position.y <= -2)
        {
            Scene thisScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(thisScene.name);
        }
    }

    private void TutorialHandler()
    {
        if (scoreManager.stroke == 1)
        {
            pauseManager.tutorialChargingObject.SetActive(false);
            pauseManager.tutorialAimObject.SetActive(true);
            pauseManager.tutorialThrowingObject.SetActive(true);

            if (throwing)
            {
                pauseManager.tutorialChargingObject.SetActive(true);

                pauseManager.tutorialAimObject.SetActive(false);
                pauseManager.tutorialThrowingObject.SetActive(false);
            }

            if (isThrown)
            {
                pauseManager.tutorialFlyingObject.SetActive(true);

                pauseManager.tutorialChargingObject.SetActive(false);
                pauseManager.tutorialAimObject.SetActive(false);
                pauseManager.tutorialThrowingObject.SetActive(false);
            }
            else if (!isThrown)
            {
                pauseManager.tutorialFlyingObject.SetActive(false);
            }
        }
        else if (scoreManager.stroke > 1)
        {
            pauseManager.tutorialChargingObject.SetActive(false);
            pauseManager.tutorialAimObject.SetActive(false);
            pauseManager.tutorialThrowingObject.SetActive(false);
            pauseManager.tutorialFlyingObject.SetActive(false);
        }

        if (pauseManager.isPaused)
        {
            pauseManager.tutorialChargingObject.SetActive(false);
            pauseManager.tutorialAimObject.SetActive(false);
            pauseManager.tutorialThrowingObject.SetActive(false);
            pauseManager.tutorialFlyingObject.SetActive(false);
        }
    }

    /// <summary>
    /// Returns true if level is finished, false if not.
    /// </summary>
    /// <returns></returns>
    public bool GetFinished()
    {
        return finished;
    }


    /// <summary>
    /// Loads the level asynchrously and plays the proper animation while doing so 
    /// </summary>
    IEnumerator LevelLoad(string level)
    {
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

        //yield return new WaitForSeconds(1);
        //SceneManager.LoadScene(level);
    }
}