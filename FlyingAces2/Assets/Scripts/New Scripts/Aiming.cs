using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Aiming : MonoBehaviour
{
    Rigidbody rb;

    [Tooltip("How quickly the camera reacts to mouse movement")]
    public float mouseSensitivity = 100f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        AimLogic();
    }

    void AimLogic()
    {
        // Input value for vertical mouse movement
        float leftrightvalue = Input.GetAxisRaw("Mouse X");
        // Input value for horizontal mouse movement
        float updownvalue = Input.GetAxisRaw("Mouse Y");

        // The Vector3 for rotating the player vertically
        Vector3 rotationX = new Vector3(-updownvalue, 0, 0);
        // The Vector3 for rotating the player horizontally
        Vector3 rotationY = new Vector3(0, leftrightvalue, 0);

        rb.transform.Rotate(rotationX / 3);
        //rb.transform.Rotate(rotationY / 3);
    }
}
