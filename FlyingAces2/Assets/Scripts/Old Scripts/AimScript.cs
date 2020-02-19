using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AimScript : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    public Camera cam;

    public float mouseSensitivity = 100f;

    private float rotationX = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        //cam = GetComponentInChildren<Camera>();
        /*
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            //Cursor.visible = false;
        }*/

    }

    // Update is called once per frame
    void Update()
    {
        AimLogic();
    }

    void AimLogic()
    {
        /*
        float arrowX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; 
        float arrowY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; 

        rotationX -= arrowY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        cam.transform.Rotate(Vector3.up * arrowX);
        */


        
        float leftrightvalue = Input.GetAxisRaw("Mouse X");
        float updownvalue = Input.GetAxisRaw("Mouse Y");

        Vector3 rotationX = new Vector3(-updownvalue, 0, 0);
        Vector3 rotationY = new Vector3(0, leftrightvalue, 0);

        //rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationY));

        rb.transform.Rotate(rotationX / 3);
        cam.transform.Rotate(rotationX / 3);
        
    }
}
