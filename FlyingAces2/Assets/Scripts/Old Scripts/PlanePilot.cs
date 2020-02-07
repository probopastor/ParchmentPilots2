using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlanePilot : MonoBehaviour
{
    public Camera aimingCam;
    public Camera planeCam;
    public float speed = 90.0f;
    public bool inWindZone = false;
    public GameObject windZone;
    public static bool thrown = false;
    Rigidbody rb;

    float liftCoefficent;
    Vector3 launchSpeed = new Vector3(0, 300, 2000);
    Vector3 liftForce;

    public int stroke = 1;
    public TextMeshProUGUI strokeText;
    //public Text instructionsText;
    //public Text speedText;
    bool finished = false;

    public GameObject player;
    Vector3 strokePosition = new Vector3(0f, 0f, 0f);
    public string sceneName;
    public Scene currentScene;
    void Start()
    {
        strokeText.text = "Stroke: " + stroke;
        aimingCam.enabled = true;
        planeCam.enabled = false;
        //Debug.Log("plane pilot script added to: " + gameObject.name);
        rb = GetComponent<Rigidbody>();
        thrown = false;
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        //print(sceneName);
    }

    // Update is called once per frame
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
        Vector3 moveCamTo = transform.position - transform.forward * 5.0f + Vector3.up * 0.0f;
        float bias = 0.96f;
        planeCam.transform.position = planeCam.transform.position * bias + moveCamTo * (1.0f - bias);
        planeCam.transform.LookAt(transform.position + transform.forward * 30.0f);
        ///////////////////

        //if (inWindZone)
        //{
        //    rb.AddForce(windZone.GetComponent<WindArea>().direction * windZone.GetComponent<WindArea>().strength);
        //}
        if(transform.rotation.x > 0)
        {
            liftCoefficent = -1 * transform.rotation.x / 3.75f;
        }
        else
        {
            liftCoefficent = -1 * transform.rotation.x / 3.75f;
        }
        
        liftForce = new Vector3(0, liftCoefficent * (Mathf.Pow(rb.velocity.z, 2)), -liftCoefficent * (Mathf.Pow(rb.velocity.z, 2) / 10));

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!thrown)
            {
                aimingCam.enabled = false;
                planeCam.enabled = true;
                GetComponent<AimScript>().enabled = false;
                rb.isKinematic = false;
                rb.AddRelativeForce(launchSpeed);
            }
            thrown = true;
        }

        if (thrown)
        {
            rb.useGravity = thrown;
            rb.AddRelativeForce(liftForce);
            //transform.position += transform.forward * Time.deltaTime * speed;
            //speed -= transform.forward.y * Time.deltaTime * 50.0f;

            //if (speed < 45.0f)
            //{
            //    speed = 45.0f;
            //}
            rb.AddRelativeForce(new Vector3(Input.GetAxis("Horizontal") * speed, 0, 0));
            transform.Rotate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WindArea")
        {
            windZone = other.gameObject;
            inWindZone = true;
        }

        if (other.tag == "ring")
        {
            speed += 10;
        }

        if (other.tag == "finish")
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
            SceneManager.LoadScene("GameScene");
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
            newTee += new Vector3(0.0f, 30.0f, 0.0f);
            stroke += 1;
            strokeText.text = "Stroke: " + stroke;
            speed = 90;
            //strokeText.text = "Stroke: " + stroke;
            //speed = 90;
            thrown = false;
            aimingCam.enabled = true;
            planeCam.enabled = false;
            GetComponent<AimScript>().enabled = true;
            gameObject.transform.position = newTee;
            player.transform.position = newTee;
            //gameObject.transform.parent = player.transform;

            if (hitGround1)
            {
                gameObject.transform.position = player.transform.position + new Vector3(-1.5f, -3.0f, 10.0f);
                gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                player.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
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

            rb.isKinematic = true;
            //Debug.Log(newTee);
        }

    }

}
