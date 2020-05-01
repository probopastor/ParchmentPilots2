using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EndingZoom : MonoBehaviour
{
    public CinemachineVirtualCamera landCam;
    private TestFlight testFlight;

    bool endingCoroutineRunning;

    public float xFollow = 1f;
    public float yFollow = 1f;
    public float zFollow = 1f;

    private bool invertX;
    private bool invertY;
    private bool invertZ;


    public float xFollowIncreaseMax = -70.2f;
    public float yFollowIncreaseMax = 53.5f;
    public float zFollowIncreaseMax = 5.3f;

    public float followIncreaseSmoothness = 0.01f;

    public GameObject flag;

    public bool doOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        testFlight = FindObjectOfType<TestFlight>();

        if (xFollowIncreaseMax < 0)
        {
            invertX = true;
        }

        if(yFollowIncreaseMax < 0)
        {
            invertY = true;
        }

        if(zFollowIncreaseMax < 0)
        {
            invertZ = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //When the game is complete, run the Ending Coroutine
        if(testFlight.GetFinished() && !endingCoroutineRunning)
        {
            landCam.Follow = flag.transform;
            landCam.LookAt = flag.transform;

            endingCoroutineRunning = true;
            doOnce = false;
            StartCoroutine(EndingCoroutine());
        }
    }

    /// <summary>
    /// Increases the transposer's follow offset to the chosen max value over a period of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndingCoroutine()
    {
        if(!doOnce)
        {
            doOnce = true;
            CinemachineCollider cameraCollider = landCam.GetComponent<CinemachineCollider>();
            cameraCollider.enabled = false;
        }

        var transposer = landCam.GetCinemachineComponent<CinemachineTransposer>();

        int correctnessCount = 0;

        if(invertX)
        {
            if (transposer.m_FollowOffset.x <= xFollowIncreaseMax)
            {
                correctnessCount++;
            }
            else
            {
                transposer.m_FollowOffset.x -= xFollow;
            }
        }
        else if(!invertX)
        {
            if (transposer.m_FollowOffset.x >= xFollowIncreaseMax)
            {
                correctnessCount++;
            }
            else
            {
                transposer.m_FollowOffset.x += xFollow;
            }
        }

        if(invertY)
        {
            if (transposer.m_FollowOffset.y <= yFollowIncreaseMax)
            {
                correctnessCount++;
            }
            else
            {
                transposer.m_FollowOffset.y -= yFollow;
            }
        }
        else if(!invertY)
        {
            if (transposer.m_FollowOffset.y >= yFollowIncreaseMax)
            {
                correctnessCount++;
            }
            else
            {
                transposer.m_FollowOffset.y += yFollow;
            }
        }

        if(invertZ)
        {
            if (transposer.m_FollowOffset.z <= zFollowIncreaseMax)
            {
                correctnessCount++;
            }
            else
            {
                transposer.m_FollowOffset.z -= zFollow;
            }
        }
        else if(!invertZ)
        {
            if (transposer.m_FollowOffset.z >= zFollowIncreaseMax)
            {
                correctnessCount++;
            }
            else
            {
                transposer.m_FollowOffset.z += zFollow;
            }
        }

        if (correctnessCount < 2)
        {
            yield return new WaitForSeconds(followIncreaseSmoothness);
            StartCoroutine(EndingCoroutine());
        }

        yield return new WaitForEndOfFrame();
    }
}
