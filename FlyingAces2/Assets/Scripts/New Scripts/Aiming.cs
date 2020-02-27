using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Aiming : MonoBehaviour
{
    Rigidbody rb;
    TestFlight tf;

    //[Tooltip("How quickly the camera reacts to mouse movement")]
    //public float mouseSensitivity = 100f;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

        tf = FindObjectOfType<TestFlight>();
    }

    // Update is called once per frame
    void Update()
    {
        //AimLogic();
    }

    void AimLogic()
    {
        // Input value for vertically rotating the plane
        float leftrightvalue = Input.GetAxisRaw("Horizontal");
        // Input value for horizontally rotating the plane
        float updownvalue = Input.GetAxisRaw("Vertical");

        // The Vector3 for rotating the player vertically
        Vector3 rotationX = new Vector3(-updownvalue, 0, 0);
        // The Vector3 for rotating the player horizontally
        Vector3 rotationY = new Vector3(0, leftrightvalue, 0);

        rb.transform.Rotate(rotationX / 3, Space.Self);
        rb.transform.Rotate(rotationY / 3, Space.Self);

        if(rb.transform.rotation.z != 0)
        {
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
        }
    }
}
