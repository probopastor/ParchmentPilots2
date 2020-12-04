using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialHandler : MonoBehaviour
{
    private ScoreManager scoreManager;
    private PlaneThrow_Handler planeThrowHandler;
    private PauseManager pauseManager;
    public bool isLevel1;

    public GameObject tutorialAimObject;
    public GameObject tutorialThrowingObject;
    public GameObject tutorialChargingObject;
    public GameObject tutorialFlyingObject;

    public TextMeshProUGUI tutorialAimText;
    public TextMeshProUGUI tutorialThrowingText;
    public TextMeshProUGUI tutorialChargingText;
    public TextMeshProUGUI tutorialFlyingText;

    public InputMaster controls;

    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.DeviceCheck.performed += context => ChangeTutorialText();
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void ChangeTutorialText()
    {
        if (Keyboard.current.anyKey.isPressed)
        {
            tutorialAimText.text = "Use arrow keys/WASD to aim.";
            tutorialThrowingText.text = "Press " + "\"" + "Enter" + "\"" + "to charge your throw.";
            tutorialChargingText.text = "Press " + "\"" + "Enter" + "\"" + " to throw. " + "\"" + "Esc" + "\"" + " to aim.";
            tutorialFlyingText.text = "Use arrow keys/WASD to control your flight.";
        }
        else if (!Keyboard.current.anyKey.isPressed)
        {
            tutorialAimText.text = "Use Left Stick to aim.";
            tutorialThrowingText.text = "Press " + "\"" + "A" + "\"" + "to charge your throw.";
            tutorialChargingText.text = "Press " + "\"" + "A" + "\"" + " to throw. " + "\"" + "B" + "\"" + " to aim.";
            tutorialFlyingText.text = "Use Left Stick to control your flight.";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        planeThrowHandler = GameObject.FindObjectOfType<PlaneThrow_Handler>();
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
        pauseManager = GameObject.FindObjectOfType<PauseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLevel1)
        {
            TutorialMessages();
        }
        else if (!isLevel1)
        {
            tutorialChargingObject.SetActive(false);
            tutorialAimObject.SetActive(false);
            tutorialThrowingObject.SetActive(false);
            tutorialFlyingObject.SetActive(false);
        }
    }

    private void TutorialMessages()
    {
        if (scoreManager.GetStroke() == 1)
        {
            tutorialChargingObject.SetActive(false);
            tutorialAimObject.SetActive(true);
            tutorialThrowingObject.SetActive(true);

            if (planeThrowHandler.GetThrowStatus())
            {
                tutorialChargingObject.SetActive(true);

                tutorialAimObject.SetActive(false);
                tutorialThrowingObject.SetActive(false);
            }

            if (planeThrowHandler.GetThrowStatus())
            {
                tutorialFlyingObject.SetActive(true);

                tutorialChargingObject.SetActive(false);
                tutorialAimObject.SetActive(false);
                tutorialThrowingObject.SetActive(false);
            }
            else if (!planeThrowHandler.GetThrowStatus())
            {
                tutorialFlyingObject.SetActive(false);
            }
        }
        else if (scoreManager.GetStroke() > 1)
        {
            tutorialChargingObject.SetActive(false);
            tutorialAimObject.SetActive(false);
            tutorialThrowingObject.SetActive(false);
            tutorialFlyingObject.SetActive(false);
        }

        if (pauseManager.isPaused)
        {
            tutorialChargingObject.SetActive(false);
            tutorialAimObject.SetActive(false);
            tutorialThrowingObject.SetActive(false);
            tutorialFlyingObject.SetActive(false);
        }
    }
}
