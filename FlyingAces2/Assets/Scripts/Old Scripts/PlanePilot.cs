using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlanePilot : MonoBehaviour
{
    public float yBoundary = -10f;

    public Vector3 thrownCamPos;
    private Vector3 camStartPos;

    //public Camera aimingCam;
    public Camera planeCam;
    public float speed = 90.0f;
    public bool inWindZone = false;
    public GameObject windZone;
    public static bool thrown = false;
    Rigidbody rb;
    public float windZoneForce = 500f;
    public float speedBoostForce = 500f;

    float liftCoefficent;
    float forwardLiftCoefficent;
    Vector3 launchSpeed = new Vector3(0, 300, 2000);
    Vector3 liftForce;
    Vector3 forwardLiftForce;

    public int stroke = 1;
    public TextMeshProUGUI strokeText;
    //public Text instructionsText;
    //public Text speedText;
    bool finished = false;

    public GameObject player;
    Vector3 strokePosition = new Vector3(0f, 0f, 0f);
    public string sceneName;
    public Scene currentScene;
    public string nextSceneName;

    private Quaternion startRot;
    void Start()
    {
        startRot = gameObject.transform.rotation;
        strokeText.text = "Stroke: " + stroke;
        //aimingCam.enabled = true;
        //planeCam.enabled = false;
        //Debug.Log("plane pilot script added to: " + gameObject.name);
        camStartPos = planeCam.transform.localPosition;
        rb = GetComponent<Rigidbody>();
        thrown = false;
        currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        //print(sceneName);
    }

    // Update is called once per frame
    private void Update()
    {
        if(gameObject.transform.position.y < yBoundary)
        {
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!thrown)
            {
                thrown = true;

                planeCam.transform.localPosition = thrownCamPos;
                //aimingCam.enabled = false;
                //planeCam.enabled = true;

                //GetComponent<Aiming>().enabled = false;

                GetComponent<AimScript>().enabled = false;
                //rb.isKinematic = false;
                rb.AddRelativeForce(launchSpeed);
                stroke += 1;

            }
        }
    }
    void FixedUpdate()
    {
        //if (Input.GetKey(KeyCode.R))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}

        //if (Input.GetKey(KeyCode.M))
        //{
        //    SceneManager.LoadScene("MainMenu");
        //}

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    if (instructionsText.gameObject.activeSelf == true)
        //    {
        //        instructionsText.gameObject.SetActive(false);
        //    }
        //    else if (instructionsText.gameObject.activeSelf == false)
        //    {
        //        instructionsText.gameObject.SetActive(true);
        //    }

        //}

        //speedText.text = "Speed: " + speed;

        //CAMERA BEHAVIOR//
        //Vector3 moveCamTo = transform.position - transform.forward * 5.0f + Vector3.up * 0.0f;
        //float bias = 0.96f;
        //planeCam.transform.position = planeCam.transform.position * bias + moveCamTo * (1.0f - bias);
        //planeCam.transform.LookAt(transform.position + transform.forward * 30.0f);
        ///////////////////

        //if (inWindZone)
        //{
        //    rb.AddForce(windZone.GetComponent<WindArea>().direction * windZone.GetComponent<WindArea>().strength);
        //}
        //if (transform.rotation.x > 0)
        //{
        //    liftCoefficent = -1 * transform.rotation.x / 3.75f;
        //    forwardLiftCoefficent = -1 * transform.rotation.x / 5f;
        //}
        //else
        //{
        //    liftCoefficent = -1 * transform.rotation.x / 3.75f;
        //    forwardLiftCoefficent = -1 * transform.rotation.x / 3.75f;
        //}
        
        //liftForce = new Vector3(0, liftCoefficent * (Mathf.Pow(rb.velocity.z, 2)), 0);
        //forwardLiftForce = new Vector3(0, 0, -forwardLiftCoefficent * (Mathf.Pow(rb.velocity.z, 2) / 8));
        

        //if (thrown)
        //{
        //    //rb.useGravity = thrown;
        //    //rb.velocity -= Vector3.up * Time.deltaTime;
        //    //Vector3 vertVel = rb.velocity - Vector3.ProjectOnPlane(transform.up, rb.velocity);
        //    //var fall = vertVel.magnitude;
        //    //rb.velocity -= vertVel * Time.deltaTime;
        //    //rb.velocity += vertVel.magnitude * transform.forward * Time.deltaTime;
        //    ////rb.AddRelativeForce(liftForce);
        //    //rb.AddForce(forwardLiftForce);
        //    //transform.position += transform.forward * Time.deltaTime * speed;
        //    //speed -= transform.forward.y * Time.deltaTime * 50.0f;

        //    //if (speed < 45.0f)
        //    //{
        //    //    speed = 45.0f;
        //    //}
        //    //rb.AddRelativeForce(new Vector3(Input.GetAxis("Horizontal") * speed, 0, 0));
            
        //    transform.Rotate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), -Input.GetAxis("Horizontal"));
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WindArea")
        {
            Debug.Log("wooooooosh");
            //inWindZone = true;
            rb.AddForce(0, windZoneForce, 0);
            //rb.velocity.y * 5f
        }

        if (other.tag == "Speed Boost")
        {
            Debug.Log("ZOOOOOOOOOOM");
            rb.AddForce(0, 0, speedBoostForce);
            //rb.velocity.x * 1.3f
        }

        if (other.tag == "Finish")
        {
            finished = true;
            //strokeText.text = "You finished with a stroke of " + stroke + "! Good Job! Press R to Restart or M to go back to the Main Menu!";
        }

        if (other.tag == "outofbounds")
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WindArea")
        {
            inWindZone = false;
        }
    }

    Vector3 newTee = new Vector3();
    bool hitGround1, hitGround2, hitGround3;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            hitGround1 = true;
            SetUpNewThrow(collision);

        }

        if (collision.gameObject.tag == "Finish")
        {
            finished = true;
            SceneManager.LoadScene(nextSceneName);
            //strokeText.text = "You finished with a stroke of " + stroke + "! Good Job! Press R to Restart or M to go back to the Main Menu!";
        }
        //if (collision.gameObject.tag == "ground2")
        //{
        //    hitGround2 = true;
        //    SetUpNewThrow(collision);

        //}
        //if (collision.gameObject.tag == "ground3")
        //{
        //    hitGround3 = true;
        //    SetUpNewThrow(collision);
        //}
    }

    void SetUpNewThrow(Collision collision)
    {
        if (!finished)
        {
            ContactPoint contact = collision.GetContact(0);
            newTee = contact.point;
            newTee.y += 2f;
            planeCam.transform.localPosition = camStartPos;
            strokeText.text = "Stroke: " + stroke;
            //speed = 90;
            thrown = false;
            //aimingCam.enabled = true;
            //planeCam.enabled = false;
            //GetComponent<Aiming>().enabled = true;
            GetComponent<AimScript>().enabled = false;
            gameObject.transform.position = newTee;
            player.transform.position = newTee;
            //gameObject.transform.parent = player.transform;

            if (hitGround1)
            {
                //gameObject.transform.position = player.transform.position + new Vector3(-1.5f, -3.0f, 10.0f);
                //gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //player.transform.rotation = new Quaternion(0.5f, player.transform.rotation.y, 0.0f, player.transform.rotation.w);
                gameObject.transform.rotation = startRot;
            }
            //else if (hitGround2)
            //{
            //    gameObject.transform.position = player.transform.position + new Vector3(-1f, -3.0f, -10.0f);
            //    gameObject.transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
            //    player.transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
            //}
            //else if (hitGround3)
            //{
            //    gameObject.transform.position = player.transform.position + new Vector3(1.5f, -3.0f, 10.0f);
            //    gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            //    player.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            //}

            hitGround1 = hitGround2 = hitGround3 = false;

            //rb.isKinematic = true;
            //Debug.Log(newTee);
        }

    }
    
    private IEnumerator FadeCameraTowardsPosition(Vector3 currentPos, Vector3 newPos)
    {
        if(currentPos.x < newPos.x)
        {

        }

        if(currentPos.y < newPos.y)
        {

        }

        if(currentPos.z < newPos.z)
        {

        }

        yield return new WaitForSeconds(0.01f);
    }
}
