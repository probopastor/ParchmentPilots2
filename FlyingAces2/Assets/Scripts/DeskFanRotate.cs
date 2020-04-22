using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskFanRotate : MonoBehaviour
{

    public int rotateSpeed;
 
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
    }
}
