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
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Cinemachine;

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

    [Tooltip("The max velocity of the plane." )]
    public float maxVelocity = 25f;

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

    private PlaneThrow_Handler planeThrowHandler;

    private ScoreManager scoreManager;

    private UIHandler uiHandler;

    private PlaneParticleHandler planeParticleHandler;

    private CollectableController collectableController;
    #endregion

    void OnEnable()
    {
        pauseManager = GameObject.FindObjectOfType<PauseManager>();
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        planeThrowHandler = GameObject.FindObjectOfType<PlaneThrow_Handler>();

        planeParticleHandler = GameObject.FindObjectOfType<PlaneParticleHandler>();

        uiHandler = GameObject.FindObjectOfType<UIHandler>();

        collectableController = FindObjectOfType<CollectableController>();

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

        //Sets the starting plane position on game start
        if(planeThrowHandler != null)
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, planeThrowHandler.GetStartThrowHeight(), gameObject.transform.position.z);

        anim = GetComponent<Animator>();
        yForce = gravity;
        isThrown = false;
        camStartPos = planeCam.transform.localPosition;

        canEnableChargeBar = true;

        currentScene = SceneManager.GetActiveScene();
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

        if (Input.GetKeyDown(KeyCode.Return) && !isThrown)
        {
            if (!planeThrowHandler.GetAimStatus() && planeThrowHandler.GetThrowStatus() && !pauseManager.isPaused)
            {
                planeThrowHandler.SetThrowStatus(false);

                isThrown = true;
                anim.SetBool("isThrown", isThrown);

                planeThrowHandler.SetChargeBarActivity(false);

                Rigidbody.isKinematic = false;
                Rigidbody.useGravity = true;

                planeThrowHandler.SetAimStatus(false);

                scoreAddedThisThrow = false;

                Rigidbody.AddRelativeForce(Vector3.forward * thrustForce * planeThrowHandler.GetChargeBarValue());

                moveForward = true;
            }
        }
        else if (isThrown)
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
                    moveForward = false;
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


        if (Input.GetKeyDown(KeyCode.Return) && planeThrowHandler.GetAimStatus() && !planeThrowHandler.GetThrowStatus() && canEnableChargeBar && !pauseManager.isPaused)
        {
            planeThrowHandler.ReactivateChargeBar();
        }

        if ((Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace)) && planeThrowHandler.GetThrowStatus())
        {
            planeThrowHandler.SetThrowStatus(false);
            planeThrowHandler.SetAimStatus(true);
            planeThrowHandler.SetChargeBarActivity(false);
        }

        OutOfBoundsCheck();
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
    }

    private IEnumerator CheckForPlaneDeceleration()
    {
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
                SaveData.SaveHighScore(currentScene.name, scoreManager.GetStroke());
                SaveData.UnlockNextLevel(currentScene.name);
                collectableController.SaveBobbleheadCollection(currentScene.name);
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
}
