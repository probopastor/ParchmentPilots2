using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SelectPlane : MonoBehaviour
{
    public CinemachineFreeLook freeLook;
    public CinemachineVirtualCamera flightCam;
    public CinemachineVirtualCamera landCam;

    public GameObject[] planes;

    public Transform currentPlaneTransform;

    // Start is called before the first frame update
    void Start()
    {
        EnablePlanes(0);
        //vcb = CinemachineStateDrivenCamera.ChildCameras;
    }

    // Update is called once per frame
    void Update()
    {
        
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

                freeLook.Follow = planes[i].transform.GetChild(0).transform;
                freeLook.LookAt = planes[i].transform.GetChild(0).transform;

                flightCam.Follow = planes[i].transform;
                flightCam.LookAt = planes[i].transform;

                landCam.Follow = planes[i].transform;
                landCam.LookAt = planes[i].transform;

                Debug.Log("Enabled plane: " + planes[i]);
            }
            else
            {
                planes[i].SetActive(false);
                Debug.Log("Disabled plane: " + planes[i]);
            }
        }
    }
}
