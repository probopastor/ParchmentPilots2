using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TestFlight : MonoBehaviour
{
    #region variables
    [Tooltip("The camera object in the scene")]
    public Camera planeCam;

    [Tooltip("The UI panel for choosing planes")]
    public GameObject planeSelectPanel;

    [Tooltip("The height at which the plane is thrown on the first throw of the level")]
    public float startThrowHeight = 200f;

    [Tooltip("The height at which the plane is thrown at the start of a new throw")]
    public float throwHeight = 15f;

    [Tooltip("Number of strokes taken for the current level")]
    public int stroke = 1;

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

    public GameObject firstPlaneSelectButton;

    private int currentForceAppliedTimer = 0;
    private bool forceAppliedThisFrame;

    public AudioSource MusicSource;
    public AudioSource SoundEffectSource;
    public AudioSource LongSoundEffectSource;
    public AudioSource SinglePitchSoundEffectSource;

    private float soundEffectSourceVolume = 1f;

    public AudioClip winFanfare;
    public AudioClip winHorn;
    public AudioClip windSound;
    public AudioClip crumbleSound;
    public AudioClip hitSound;
    public AudioClip planeSelectSound;

    public float windPitchMax = 4f;
    public float windPitchMin = 0.1f;

    private bool inSlideMode;

    private SelectPlane selectPlaneTransform;

    //bool decreasePitch;
    //bool increasePitch;
    //public float windPitchMax = 1f;
    //public float windPitchMin = 1f;
    //public float defaultWindPitch = 0f;
    //public float windPitchIncrementDown = 0.1f;
    //public float windPitchIncrementUp = 0.1f;

    private bool playOnce;
    private bool playCrumbleOnce;
    private bool playWinSoundOnce;

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

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //decreasePitch = false;
        //increasePitch = false;

        //LongSoundEffectSource.pitch = defaultWindPitch;
        selectPlaneTransform = GameObject.FindObjectOfType<SelectPlane>();

        pauseManager = GameObject.FindObjectOfType<PauseManager>();

        Rigidbody = gameObject.GetComponent<Rigidbody>();

        eventSystem = EventSystem.current;

        increaseWindPitchRate = startWindPitchRate;
        soundEffectSourceVolume = SoundEffectSource.volume;

        playOnce = false;
        playCrumbleOnce = false;
        inSlideMode = false;
        playWinSoundOnce = false;

        planeSelect = false;
        planeSelectPanel.SetActive(false);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, startThrowHeight, gameObject.transform.position.z);
        RotateTowardsFinish();
        anim = GetComponent<Animator>();
        chargeBarController = GetComponent<ThrowingChargeBarController>();
        yForce = gravity;
        isThrown = false;
        camStartPos = planeCam.transform.localPosition;
        strokeText.text = stroke.ToString();
        parText.text = par.ToString();
        holeText.text = hole.ToString();
        chargeBarController.enabled = false;
        forceAppliedThisFrame = false;
        scoreText.enabled = false;

        currentForceAppliedTimer = forceAppliedTimer;

        //leftSystem = GetComponent<ParticleSystem>();
        //rightSystem = GetComponent<ParticleSystem>();
        leftMain = leftSystem.main;
        rightMain = rightSystem.main;

        emissionModuleLeft = leftSystem.emission;
        emissionModuleRight = rightSystem.emission;

        emissionModuleLeft.rateOverDistance = 0f;
        emissionModuleRight.rateOverDistance = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isThrown)
        {
            leftSystem.Stop();
            rightSystem.Stop();

            if(!pauseManager.isPaused)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    PlaneSelect();
                }
            }
            else if(pauseManager.isPaused)
            {
                DisablePlaneSelect();
            }
        }

        if (Input.GetKeyUp(KeyCode.Return) && !isThrown)
        {
            Debug.Log(pauseManager.isPaused);
            if (!aiming && throwing && !planeSelect && !pauseManager.isPaused)
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
        //else if(!isThrown && aiming)
        //{
        //    if(Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        planeSelectPanel.SetActive(true);
        //        Cursor.visible = true;

        //        planeSelect = true;
        //        aiming = false;
        //    }
        //}
        //else if(!isThrown && !aiming && planeSelect)
        //{
        //    if(Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        planeSelectPanel.SetActive(false);
        //        Cursor.visible = false;
        //        aiming = true;
        //        planeSelect = false;
        //    }
        //}
        else if (isThrown)
        {
            emissionModuleLeft.rateOverDistance = windEmmissionRate;
            emissionModuleRight.rateOverDistance = windEmmissionRate;

            RaycastHit rayHit;
            if(!playOnce)
            {
                LongSoundEffectSource.clip = windSound;
                LongSoundEffectSource.Play();
                //StartCoroutine("IncreaseAirPitch");
                leftSystem.Play();
                rightSystem.Play();

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
            if(inSlideMode && !playCrumbleOnce)
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
        else if(pauseManager.isPaused)
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

        OutOfBoundsCheck();
    }

    private void FixedUpdate()
    {
        AimLogic();
        MovePlane();    
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
    /// Enables or disables plane selection panel.
    /// </summary>
    /// <param name="closeSelectionPanel"></param>
    public void PlaneSelect()
    {
        SinglePitchSoundEffectSource.clip = planeSelectSound;
        SinglePitchSoundEffectSource.Play();

        if (aiming && !planeSelect)
        {
            planeSelectPanel.SetActive(true);
            eventSystem.SetSelectedGameObject(firstPlaneSelectButton);
            //Cursor.visible = true;

            planeSelect = true;
            aiming = false;
        }
        else if (!aiming && planeSelect)
        {
            planeSelectPanel.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
            //Cursor.visible = false;

            aiming = true;
            planeSelect = false;
        }
    }

    public void DisablePlaneSelect()
    {
        planeSelectPanel.SetActive(false);
        Cursor.visible = false;

        aiming = true;
        planeSelect = false;
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
                changes += transform.worldToLocalMatrix.MultiplyVector(transform.forward) * roll * Time.deltaTime * rollResponsivity;
            }
            if (yaw != 0)
            {
                changes += transform.worldToLocalMatrix.MultiplyVector(transform.up) * yaw * Time.deltaTime * yawResponsivity;
            }

            Quaternion newRotation = Quaternion.Euler(Rigidbody.rotation.eulerAngles + changes);

            Rigidbody.MoveRotation(newRotation);

#pragma warning disable CS0618 // Type or member is obsolete
            Vector3 vertVel = Rigidbody.velocity - Vector3.Exclude(transform.up, Rigidbody.velocity);
#pragma warning restore CS0618 // Type or member is obsolete
            fall = vertVel.magnitude;
            Rigidbody.velocity -= vertVel * Time.deltaTime;
            Rigidbody.velocity += vertVel.magnitude * transform.worldToLocalMatrix.MultiplyVector(transform.forward) * Time.deltaTime / 10;

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

            //decreasePitch = false;
            //increasePitch = false;
            anim.SetBool("Sliding", true);
        }

        if (collision.gameObject.tag == "Finish" && !finished)
        {
            
            stroke--;
            strokeText.text = stroke.ToString();
            LongSoundEffectSource.Stop();
            StartCoroutine("WinHandler");

            scoreText.enabled = true;
            if (stroke == 1)
            {
                scoreText.text = "Hole in one! Smooth flying!";
            }
            else if (stroke == par - 1)
            {
                scoreText.text = "You got a birdie! Nice!";
            }
            else if (stroke == par - 2)
            {
                scoreText.text = "You got an eagle! Good job!";
            }
            else if (stroke == par)
            {
                scoreText.text = "You got a par!";
            }
            else if (stroke == par - 3)
            {
                scoreText.text = "You got an albatross! Amazing!";
            }
            else if(stroke == par + 1)
            {
                scoreText.text = "You got a bogey. So close...";
            }
            else if (stroke == par + 2)
            {
                scoreText.text = "Double bogey. Better luck next time.";
            }
            else if (stroke == par + 3)
            {
                scoreText.text = "Triple bogey. Next hole will be better.";
            }
            else if (stroke > par + 4)
            {
                scoreText.text = "Let's not talk about that hole...";
            }
            //hole++;
            //SceneManager.LoadScene(nextSceneName);
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

            //decreasePitch = false;
            //increasePitch = false;
            anim.SetBool("Sliding", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WindArea")
        {
            Rigidbody.AddForce(0, 0, windZoneForce);
        }
    }

    private IEnumerator WinHandler()
    {
        if (!finished)
        {
            if(!playWinSoundOnce)
            {
                MusicSource.clip = winFanfare;
                MusicSource.Play();
                yield return new WaitForSeconds(5f);
                MusicSource.Stop();

                winSystem.Play();
                SinglePitchSoundEffectSource.clip = winHorn;
                SinglePitchSoundEffectSource.Play();

                yield return new WaitForSeconds(2f);

                SceneManager.LoadScene(nextSceneName);
                playWinSoundOnce = true;
            }
          
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground" && (Rigidbody.velocity.x < 0.1f && Rigidbody.velocity.y < 0.1f && Rigidbody.velocity.z < 0.1f))
        {
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
            Rigidbody.useGravity = false;

            leftSystem.Play();
            rightSystem.Play();

            ContactPoint contact = collision.GetContact(0);
            newTee = contact.point;
            newTee.y = throwHeight;
            planeCam.transform.localPosition = camStartPos;
            strokeText.text = stroke.ToString();
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

            selectPlaneTransform.currentPlaneTransform = gameObject.transform;
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

    /// <summary>
    /// If player manages to get out of bounds of the map,
    /// reload the current level.
    /// </summary>
    private void OutOfBoundsCheck()
    {
        if(gameObject.transform.position.y <= 10.1f)
        {
            Scene thisScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(thisScene.name);
        }
    }
}
