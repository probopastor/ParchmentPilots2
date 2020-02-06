using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Aiming : MonoBehaviour
{
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
        float leftrightvalue = Input.GetAxisRaw("Mouse X");
        float updownvalue = Input.GetAxisRaw("Mouse Y");
        //Vector3 rotationX = new Vector3(-updownvalue, 0, 0);
        //Vector3 rotationY = new Vector3(0, leftrightvalue, 0);

        gameObject.transform.Rotate(-updownvalue, leftrightvalue, 0);

        //rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationY));
        //cam.transform.Rotate(rotationX / 3);
    }
}
