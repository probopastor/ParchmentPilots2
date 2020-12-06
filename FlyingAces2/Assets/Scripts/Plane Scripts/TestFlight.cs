/*
* Author: William Nomikos, Grant Frey
* TestFlight.cs
* Handles plane throwing, plane re-throwing, moving the plane, plane gravity, 
* audio source pitch increasing with velocity, particles changing with
* velocity, score text, rotating the plane towards the flag on rethrow, 
* the tutorial, and winning the level.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Cinemachine;
using AwesomeToon;

public class TestFlight : MonoBehaviour
{
    #region variables

    [Tooltip("The camera object in the scene")]
    public Camera planeCam;

    [Tooltip("The force that the wind zones put on the plane")]
    public float windZoneForce;

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

    [Tooltip("The planes center of mass at which torq should be applied")]
    public GameObject centerOfMassReference;

    [Tooltip("The strength of the force applied to the nose of the plane")]
    public Vector3 forceAtPos = new Vector3(0, 1, 0);

    [Tooltip("The max velocity of the plane.")]
    public float maxVelocity = 25f;

    #region Stall Variables
    [Tooltip("The X stall velocity. ")]
    public float xStallVel = 10f;

    [Tooltip("The Y stall velocity. ")]
    public float yStallVel = 1f;

    [Tooltip("The Z stall velocity. ")]
    public float zStallVel = 10f;
    #endregion 

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

    [Tooltip("The victory particles")]
    public ParticleSystem winSystem;

    private bool scoreAddedThisThrow;

    private bool canEnableChargeBar;

    private bool moveForward;

    private float thisAngle = 0;

    bool checkForPlaneDeceleration;
    bool firstDecelerationCheck;

    private Transform closestObject;

    public Animator transitionAnimator;

    private MeshRenderer meshRenderer;

    private AwesomeToonHelper toonHelper;

    private PlaneThrow_Handler planeThrowHandler;

    private ScoreManager scoreManager;

    private UIHandler uiHandler;

    private PlaneParticleHandler planeParticleHandler;

    private CollectableController collectableController;

    private AchievementsController achievementsController;

    private bool planeStalled;

    public Material specialMaterial;

    public InputMaster controls;

    private Vector2 input;

    private bool lowPowerThrows = true;
    private bool highPowerThrows = true;

    private GameObject[] fans;
    private int fansHitThisLevel = 0;

    private float idleTimer = 0;
    private float flightTimer = 0;
    #endregion

    void Awake()
    {
        controls = new InputMaster();
        controls.Player.Throw.performed += cxt => ThrowCheck();
        controls.Player.Throw.performed += cxt => CheckChargeBar();
        controls.Player.Cancel.performed += cxt => CancelChargeBar();
        controls.Player.Aim.performed += context => HandleAimInput(context);
        controls.Player.Aim.canceled += context => ResetAimInput();
        controls.Player.DeviceCheck.performed += context => ResetAFKTimer();
    }

    private void OnEnable()
    {
        controls.Enable();
        pauseManager = GameObject.FindObjectOfType<PauseManager>();
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void HandleAimInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    void ResetAimInput()
    {
        input = Vector2.zero;
    }

    void ResetAFKTimer()
    {
        idleTimer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        planeThrowHandler = GameObject.FindObjectOfType<PlaneThrow_Handler>();

        planeParticleHandler = GameObject.FindObjectOfType<PlaneParticleHandler>();

        uiHandler = GameObject.FindObjectOfType<UIHandler>();

        collectableController = FindObjectOfType<CollectableController>();

        achievementsController = FindObjectOfType<AchievementsController>();

        fans = GameObject.FindGameObjectsWithTag("WindArea");

        moveForward = false;
        planeStalled = false;

        pauseManager = GameObject.FindObjectOfType<PauseManager>();

        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        Rigidbody = gameObject.GetComponent<Rigidbody>();

        eventSystem = EventSystem.current;

        meshRenderer = GetComponent<MeshRenderer>();

        toonHelper = GetComponent<AwesomeToonHelper>();

        increaseWindPitchRate = startWindPitchRate;
        soundEffectSourceVolume = SoundEffectSource.volume;

        playOnce = false;
        playCrumbleOnce = false;
        inSlideMode = false;
        playWinSoundOnce = false;

        scoreAddedThisThrow = false;

        //Sets the starting plane position on game start
        if (planeThrowHandler != null)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, planeThrowHandler.GetStartThrowHeight(), 
                gameObject.transform.position.z);
        }
            

        anim = GetComponent<Animator>();
        yForce = gravity;
        isThrown = false;
        camStartPos = planeCam.transform.localPosition;

        canEnableChargeBar = true;

        currentScene = SceneManager.GetActiveScene();

        if (PlayerPrefs.GetInt(currentScene.name + " bobblehead 1", 0) == 1 && PlayerPrefs.GetInt(currentScene.name + " bobblehead 2", 0) == 1 &&
            PlayerPrefs.GetInt(currentScene.name + " bobblehead 3", 0) == 1)
        {
            toonHelper.material = specialMaterial;
        }
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
 
        if (isThrown)
        {
            RaycastHit rayHit;
            if (!playOnce)
            {
                LongSoundEffectSource.clip = windSound;
                LongSoundEffectSource.Play();

                playOnce = true;
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, skyLayer))
            {
                AngleBetweenGround(rayHit);

                if (thisAngle > 4 && thisAngle < 5f)
                {
                    if (!planeStalled)
                    {
                        StallPlane();
                    }
                }
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
        AFKTimer();
        airborneTimer();
        OutOfBoundsCheck();
    }

    private void AFKTimer()
    {
        idleTimer += 1 * Time.deltaTime;
        if(idleTimer >= 180 && PlayerPrefs.GetInt("Achievement 13", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 13", 1);
            achievementsController.AchievementGet(13);
        }
    }

    private void airborneTimer()
    {
        if(!planeThrowHandler.GetAimStatus() && !planeThrowHandler.GetThrowStatus())
        {
            flightTimer += 1 * Time.deltaTime;
            if (flightTimer >= 30 && PlayerPrefs.GetInt("Achievement 14", 0) == 0)
            {
                PlayerPrefs.SetInt("Achievement 14", 1);
                achievementsController.AchievementGet(14);
            }
        }
        else
        {
            flightTimer = 0;
        }

    }

    private void CancelChargeBar()
    {
        if (planeThrowHandler.GetThrowStatus())
        {
            planeThrowHandler.SetThrowStatus(false);
            planeThrowHandler.SetAimStatus(true);
            planeThrowHandler.SetChargeBarActivity(false);
        }
    }

    private void CheckChargeBar()
    {
        if (planeThrowHandler.GetAimStatus() && !planeThrowHandler.GetThrowStatus() && canEnableChargeBar && !pauseManager.isPaused)
        {
            planeThrowHandler.ReactivateChargeBar();
        }
    }

    private void ThrowCheck()
    {
        if (!isThrown && !planeThrowHandler.GetAimStatus() && planeThrowHandler.GetThrowStatus() && !pauseManager.isPaused)
        {
            planeThrowHandler.SetThrowStatus(false);

            isThrown = true;
            anim.SetBool("isThrown", isThrown);

            

            Rigidbody.isKinematic = false;
            Rigidbody.useGravity = true;

            planeThrowHandler.SetAimStatus(false);

            scoreAddedThisThrow = false;

            Rigidbody.AddRelativeForce(Vector3.forward * thrustForce * planeThrowHandler.GetChargeBarValue());
            
            if(planeThrowHandler.GetChargeBarValue() > 50 && lowPowerThrows == true)
            {
                lowPowerThrows = false;
            }

            if (planeThrowHandler.GetChargeBarValue() < 50 && highPowerThrows == true)
            {
                highPowerThrows = false;
            }

            if (planeThrowHandler.GetChargeBarValue() >= 99 && PlayerPrefs.GetInt("Achievement 6", 0) == 0)
            {
                PlayerPrefs.SetInt("Achievement 6", 1);
                achievementsController.AchievementGet(6);
            }
            if (planeThrowHandler.GetChargeBarValue() <= 1 && PlayerPrefs.GetInt("Achievement 7", 0) == 0)
            {
                PlayerPrefs.SetInt("Achievement 7", 1);
                achievementsController.AchievementGet(7);
            }
            Debug.Log(planeThrowHandler.GetChargeBarValue());
            moveForward = true;
            planeThrowHandler.SetChargeBarActivity(false);
        }
    }

    private void FixedUpdate()
    {
        MovePlane();

        if (isThrown)
        {
            if (!pauseManager.isPaused)
            {
                ForceAtCenterOfMass();
            }
        }

        CheckMaxVelocity();
    }

    /// <summary>
    /// Clamps the plane's velocity at the max velocity value. 
    /// </summary>
    private void CheckMaxVelocity()
    {
        Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, maxVelocity);
        if(Rigidbody.velocity.magnitude == maxVelocity && PlayerPrefs.GetInt("Achievement 18", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 18", 1);
            achievementsController.AchievementGet(18);
        }
    }

    private IEnumerator CheckForPlaneDeceleration()
    {
        if (!firstDecelerationCheck)
        {
            firstDecelerationCheck = true;
            yield return new WaitForSeconds(2);
        }

        if ((Rigidbody.velocity.x < xStallVel && Rigidbody.velocity.x > -xStallVel)
            && (Rigidbody.velocity.z < zStallVel && Rigidbody.velocity.z > -zStallVel)
            && Rigidbody.velocity.y < yStallVel)
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
    /// Allows the player to move the plane once it is thrown.
    /// </summary>
    private void MovePlane()
    {
        if (isThrown)
        {
            float roll = input.x;
            float tilt = input.y;

            float yaw = 0;//Input.GetAxis("Yaw") / 8

            float tip = (transform.right + Vector3.up).magnitude - 1.414214f;
            yaw -= tip;

            if (OptionsController.invertedVerticalControls)
            {
                tilt = -input.y;
            }
            else
            {
                tilt = input.y;
            }

            if (OptionsController.invertedHorizontalControls)
            {
                roll = -input.x;
            }
            else
            {
                roll = input.x;
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

            if (moveForward)
            {
                Vector3 newForward = transform.forward * Rigidbody.velocity.magnitude;
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

    /// <summary>
    /// Determines the angle between the plane and the ground. 
    /// </summary>
    /// <param name="rayHit"></param>
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
        }
    }

    /// <summary>
    /// Applies a force at the plane's center of mass on frames where currentForceappliedTimer is less than 1.
    /// As a result, the plane will slowly tilt towards the ground. 
    /// </summary>
    private void ForceAtCenterOfMass()
    {
        Rigidbody.AddForceAtPosition(forceAtPos, centerOfMassReference.transform.position);
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

            planeParticleHandler.PauseParticles();

            anim.SetBool("Sliding", true);
        }

        if (collision.gameObject.tag == "Finish" && !finished)
        {
            LongSoundEffectSource.Stop();
            uiHandler.FinishScore();
            StartCoroutine("WinHandler");

            finished = true;
        }

        if(collision.gameObject.tag == "Toast" && PlayerPrefs.GetInt("Achievement 15", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 15", 1);
            achievementsController.AchievementGet(15);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            SoundEffectSource.Stop();

            increaseWindPitchRate = startWindPitchRate;
            inSlideMode = false;

            planeParticleHandler.ResumeParticles();

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
            RotateTowardsWindDirection(windAngle);
        }
    }

    /// <summary>
    /// Handles stalling the plane, which rotates the plane towards the ground and 
    /// prevents normal plane control.
    /// </summary>
    private void StallPlane()
    {
        moveForward = false;
        planeStalled = true;

        StartCoroutine(RotateTowardsGround());

    }

    private IEnumerator RotateTowardsGround()
    {
        //if (planeStalled)
        //{

        //    Vector3 x = Vector3.Cross(transform.forward, -Vector3.up);
        //    float theta = Mathf.Asin(x.magnitude);
        //    Vector3 w = x.normalized * theta / Time.fixedDeltaTime;
        //    Quaternion q = transform.rotation * Rigidbody.inertiaTensorRotation;
        //    Vector3 T = q * Vector3.Scale(Rigidbody.inertiaTensor, (Quaternion.Inverse(q) * w));

        //    Rigidbody.AddRelativeTorque(T);

        //    Rigidbody.AddRelativeTorque(Vector3.right * 100f);


        //    yield return new WaitForEndOfFrame();
        //    StartCoroutine(RotateTowardsGround());
        //}

        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Rotates the plane in the direction of fan wind when it is accelerated by
    /// a fan.
    /// </summary>
    /// <param name="angle"></param>
    private void RotateTowardsWindDirection(Vector3 angle)
    {
        //Rotate the plane in the direction of the wind
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WindArea")
        {
            moveForward = false;
            if(PlayerPrefs.GetInt("Fans hit", 0) <= 10)
            {
                PlayerPrefs.SetInt("Fans hit", PlayerPrefs.GetInt("Fans hit") + 1);
                if(PlayerPrefs.GetInt("Fans hit") >= 5 && PlayerPrefs.GetInt("Achievement 3", 0) == 0)
                {
                    PlayerPrefs.SetInt("Achievement 3", 1);
                    achievementsController.AchievementGet(3);
                }
                if (PlayerPrefs.GetInt("Fans hit") >= 10 && PlayerPrefs.GetInt("Achievement 4", 0) == 0)
                {
                    PlayerPrefs.SetInt("Achievement 4", 1);
                    achievementsController.AchievementGet(4);
                }
            }
            if(++fansHitThisLevel == fans.Length && PlayerPrefs.GetInt("Achievement 5", 0) == 0)
            {
                PlayerPrefs.SetInt("Achievement 5", 1);
                achievementsController.AchievementGet(5);
            }
        }

        if(other.tag == "Recycle bin" && PlayerPrefs.GetInt("Achievement 16", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 16", 1);
            achievementsController.AchievementGet(16);
        }

        if (other.tag == "Party" && PlayerPrefs.GetInt("Achievement 19", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 19", 1);
            achievementsController.AchievementGet(19);
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
                SaveData.SaveHighScore(currentScene.name, scoreManager.GetStroke());
                SaveData.UnlockNextLevel(currentScene.name);
                collectableController.SaveBobbleheadCollection(currentScene.name);
                AchievementCheck();
                yield return new WaitForSeconds(6f);
                MusicSource.Stop();

                winSystem.Play();
                TrumpetSource.clip = winHorn;
                TrumpetSource.Play();

                yield return new WaitForSeconds(1f);
                playWinSoundOnce = true;

                pauseManager.OpenEndOfLevelMenu();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground" && (Rigidbody.velocity.x < 0.1f && Rigidbody.velocity.y < 0.1f && Rigidbody.velocity.z < 0.1f))
        {
            if (!scoreAddedThisThrow)
            {
                scoreManager.IncreaseScore(1);
                scoreAddedThisThrow = true;
            }

            hitGround = true;
            inSlideMode = false;
            increaseWindPitchRate = startWindPitchRate;
            LongSoundEffectSource.Stop();
            anim.SetBool("Sliding", false);
            playOnce = false;
            playCrumbleOnce = false;

            planeThrowHandler.SetUpNewThrow(collision);
        }
    }

    /// <summary>
    /// If player manages to get out of bounds of the map,
    /// reload the current level.
    /// </summary>
    private void OutOfBoundsCheck()
    {
        if (gameObject.transform.position.y <= -385)
        {
            Scene thisScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(thisScene.name);
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
    }

    public void SetPlaneDecelerationStatus(bool isDecelerating)
    {
        checkForPlaneDeceleration = isDecelerating;
    }

    public void SetFirstDecelerationCheck(bool isFirstDecelerated)
    {
        firstDecelerationCheck = isFirstDecelerated;
    }

    public void SetHitGroundStatus(bool planeHitGround)
    {
        hitGround = planeHitGround;
    }

    public bool GetHitGroundStatus()
    {
        return hitGround;
    }

    public Rigidbody GetPlaneBody()
    {
        return Rigidbody;
    }

    public float GetPlaneVelocity()
    {
        return Rigidbody.velocity.magnitude;
    }

    public void SetAimRotation(Vector3 changesInRotation)
    {
        Quaternion newRotation = Quaternion.Euler(Rigidbody.rotation.eulerAngles + changesInRotation);
        Rigidbody.MoveRotation(newRotation);
    }

    public void TemporaryResetMethod(Collision collision)
    {
        if (!finished)
        {
            SetPlaneDecelerationStatus(false);
            SetFirstDecelerationCheck(false);

            planeStalled = false;
            moveForward = false;
            Rigidbody.useGravity = false;
            planeParticleHandler.PlayParticles();

            ContactPoint contact = collision.GetContact(0);
            newTee = contact.point;

            newTee.y = planeThrowHandler.GetStartThrowHeight();

            uiHandler.UpdateScoreText();
            isThrown = false;

            planeThrowHandler.SetAimStatus(true);

            planeCam.transform.localPosition = camStartPos;
            anim.SetBool("isThrown", isThrown);
            gameObject.transform.position = newTee;

            if (GetHitGroundStatus())
            {
                planeThrowHandler.SetMovePlaneToPos(true);
            }

            SetHitGroundStatus(false);

            Rigidbody.isKinematic = true;
        }
    }

    public bool GetIsThrown()
    {
        return isThrown;
    }

    public bool GetEffectPlayOnce()
    {
        return playOnce;
    }

    private void AchievementCheck()
    {
        if(scoreManager.GetStroke() == 1 && PlayerPrefs.GetInt("Achievement 1", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 1", 1);
            achievementsController.AchievementGet(1);
        }

        if(PlayerPrefs.GetInt("Level 1-1 high score", 0) == 1 && PlayerPrefs.GetInt("level 1-2 high score", 0) == 1 &&
            PlayerPrefs.GetInt("level 1-3 high score", 0) == 1 && PlayerPrefs.GetInt("level 1-4 high score", 0) == 1 && 
            PlayerPrefs.GetInt("Achievement 2", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 2", 1);
            achievementsController.AchievementGet(2);
        }

        if(PlayerPrefs.GetInt("Achievement 11", 0) == 0 && lowPowerThrows == true)
        {
            PlayerPrefs.SetInt("Achievement 11", 1);
            achievementsController.AchievementGet(11);
        }

        if (PlayerPrefs.GetInt("Achievement 12", 0) == 0 && highPowerThrows == true)
        {
            PlayerPrefs.SetInt("Achievement 12", 1);
            achievementsController.AchievementGet(12);
        }
    }
}
