/*
* Author: William Nomikos, Grant Frey
* SelectPlane.cs
* [Obsolete] Allows the player to switch which plane they are using.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SelectPlane : MonoBehaviour
{
    public CinemachineVirtualCamera flightCam;
    public CinemachineVirtualCamera landCam;

    public GameObject[] planes;

    public Transform currentPlaneTransform;

    // Start is called before the first frame update
    void Start()
    {
        EnablePlanes(0);
    }

    public void EnablePlanes(int planeIndex)
    {
        for (int i = 0; i < planes.Length; i++)
        {
            if (i == planeIndex)
            {
                planes[i].SetActive(true);

                planes[i].transform.position = currentPlaneTransform.position;
                planes[i].transform.rotation = currentPlaneTransform.rotation;

                flightCam.Follow = planes[i].transform;
                flightCam.LookAt = planes[i].transform;

                landCam.Follow = planes[i].transform;
                landCam.LookAt = planes[i].transform;
                
                //planes[i].GetComponent<TestFlight>().DisablePlaneSelect();
                
            }
            else
            {
                planes[i].SetActive(false);
            }
        }
    }
}
