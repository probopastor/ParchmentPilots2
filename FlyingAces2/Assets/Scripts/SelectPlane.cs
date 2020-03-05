using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPlane : MonoBehaviour
{
    public GameObject[] planes;

    // Start is called before the first frame update
    void Start()
    {
        EnablePlanes(0);
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
                Debug.Log("Enabled plane: " + planes[i]);
            }
            else
            {
                planes[i].SetActive(false);
                Debug.Log("Disabled plane: " + planes[i]);
            }
        }
    }

    //public void SelectStandardPlane()
    //{
    //    EnablePlanes(0);
    //}

    //public void SelectSlowPlane()
    //{
    //    EnablePlanes(1);
    //}
    //public void SelectFastPlane()
    //{
    //    EnablePlanes(2);
    //}
}
