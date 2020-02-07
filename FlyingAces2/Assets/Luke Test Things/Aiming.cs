using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Aiming : MonoBehaviour
{
    float aimSpeed = 35;
    Rigidbody rb;
    [SerializeField]
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //cam = GetComponentInChildren<Camera>();
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        AimLogic();
    }

    void AimLogic()
    {

        //float updownvalue;
        //float leftrightvalue;
        //    leftrightvalue = Input.GetAxisRaw("Mouse X");
        //    updownvalue = Input.GetAxisRaw("Mouse Y");

        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.Rotate(-1 * aimSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.Rotate(1 * aimSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.Rotate(0, -1 * aimSpeed * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.Rotate(0, 1 * aimSpeed * Time.deltaTime, 0);
        }


        //Vector3 rotationX = new Vector3(-updownvalue, 0, 0);
        //Vector3 rotationY = new Vector3(0, leftrightvalue, 0);



        //rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationY));
        //cam.transform.Rotate(rotationX / 3);
    }
}
