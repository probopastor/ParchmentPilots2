using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewScaler : MonoBehaviour
{
    public float[] fieldOfView;

    private Camera thisCamera;

    // Start is called before the first frame update
    void Start()
    {
        thisCamera = gameObject.GetComponent<Camera>();
        ChangeFieldOfView();
    }

    public void ChangeFieldOfView()
    {
        Resolution thisRes = Screen.currentResolution;

        if (thisRes.width == 960 && thisRes.height == 720)
        {
            thisCamera.fieldOfView = fieldOfView[0];
        }
        else if (thisRes.width == 1280 && thisRes.height == 1024)
        {
            thisCamera.fieldOfView = fieldOfView[1];
        }
        else if (thisRes.width == 1920 && thisRes.height == 1080)
        {
            thisCamera.fieldOfView = fieldOfView[2];
        }
        else if (thisRes.width == 1680 && thisRes.height == 1050)
        {
            thisCamera.fieldOfView = fieldOfView[3];
        }
    }
}
