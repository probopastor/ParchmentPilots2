using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TutorialHandler()
    {
        if (scoreManager.GetStroke() == 1)
        {
            pauseManager.tutorialChargingObject.SetActive(false);
            pauseManager.tutorialAimObject.SetActive(true);
            pauseManager.tutorialThrowingObject.SetActive(true);

            if (planeThrowHandler.GetThrowStatus())
            {
                pauseManager.tutorialChargingObject.SetActive(true);

                pauseManager.tutorialAimObject.SetActive(false);
                pauseManager.tutorialThrowingObject.SetActive(false);
            }

            if (isThrown)
            {
                pauseManager.tutorialFlyingObject.SetActive(true);

                pauseManager.tutorialChargingObject.SetActive(false);
                pauseManager.tutorialAimObject.SetActive(false);
                pauseManager.tutorialThrowingObject.SetActive(false);
            }
            else if (!isThrown)
            {
                pauseManager.tutorialFlyingObject.SetActive(false);
            }
        }
        else if (scoreManager.stroke > 1)
        {
            pauseManager.tutorialChargingObject.SetActive(false);
            pauseManager.tutorialAimObject.SetActive(false);
            pauseManager.tutorialThrowingObject.SetActive(false);
            pauseManager.tutorialFlyingObject.SetActive(false);
        }

        if (pauseManager.isPaused)
        {
            pauseManager.tutorialChargingObject.SetActive(false);
            pauseManager.tutorialAimObject.SetActive(false);
            pauseManager.tutorialThrowingObject.SetActive(false);
            pauseManager.tutorialFlyingObject.SetActive(false);
        }
    }
}
