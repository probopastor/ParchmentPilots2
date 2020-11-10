using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlaneThrow_Handler : MonoBehaviour
{
    [Tooltip("The rotational speed multiplier on the X axis when aim is rotating while aiming")]
    public float xAimResponsitivity = 40f;

    [Tooltip("The rotational speed multiplier on the Z axis when aim is rotating while aiming")]
    public float yAimResponsitivity = 40f;

    [Tooltip("The height at which the plane is thrown on the first throw of the level")]
    public float startThrowHeight = 200f;

    [Tooltip("The height at which the plane is thrown at the start of a new throw")]
    public float throwHeight = 15f;

    [Tooltip("The first checkpoint object for the player to rotate towards")]
    public GameObject firstCheckpoint;

    public float xBuffer = 1f;
    public float zBuffer = 1f;

    private bool aiming = true;
    public bool throwing = false;

    public float RadiusToCheckOnRethrow = 1f;
    public LayerMask radiusCheckLayers;

    private TestFlight testFlight;
    private bool movePlaneToPos;
    private ThrowingChargeBarController chargeBarController;

    private GameObject lookAtObject;

    public InputMaster controls;

    Vector2 input;

    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Aim.performed += context => HandleAimInput(context);
        controls.Player.Aim.canceled += context => ResetAimInput();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void HandleAimInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    void ResetAimInput()
    {
        input = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        testFlight = GameObject.FindObjectOfType<TestFlight>();
        chargeBarController = GameObject.FindObjectOfType<ThrowingChargeBarController>();
        chargeBarController.enabled = false;
        SetRotateTowardObject(firstCheckpoint);
        RotateTowardsCheckpoiint();
    }

    void FixedUpdate()
    {
        AimLogic();      
    }

    // Update is called once per frame
    void Update()
    {
        if (movePlaneToPos)
        {
            movePlaneToPos = false;

            float xMove = 0;
            float zMove = 0;

            float xCheckHit = 0;
            float zCheckHit = 0;

            RaycastHit hit;
            if (Physics.Raycast(testFlight.transform.position, Vector3.right, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                xMove = -xBuffer;
                xCheckHit++;
            }

            if (Physics.Raycast(testFlight.transform.position, Vector3.left, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                xMove = xBuffer;
                xCheckHit++;
            }

            if (Physics.Raycast(testFlight.transform.position, Vector3.forward, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                zMove = -zBuffer;
                zCheckHit++;
            }

            if (Physics.Raycast(testFlight.transform.position, -Vector3.forward, out hit, RadiusToCheckOnRethrow, radiusCheckLayers))
            {
                zMove = zBuffer;
                zCheckHit++;
            }

            if (xCheckHit > 1)
            {
                xMove = 0;
            }

            if (zCheckHit > 1)
            {
                zMove = 0;
            }

            testFlight.gameObject.transform.position = new Vector3(testFlight.gameObject.transform.position.x + xMove, throwHeight, testFlight.gameObject.transform.position.z + zMove);
            RotateTowardsCheckpoiint();
        }
    }

    private void AimLogic()
    {
        if (aiming)
        {
            float xRotationValue;
            float yRotationValue;

            if (OptionsController.invertedVerticalControls)
            {
                yRotationValue = -input.y;
            }
            else
            {
                yRotationValue = input.y;
            }

            if (OptionsController.invertedHorizontalControls)
            {
                xRotationValue = -input.x;
            }
            else
            {
                xRotationValue = input.x;
            }

            if (testFlight.transform.rotation.eulerAngles.x > 85f && testFlight.transform.rotation.eulerAngles.x < 95f && yRotationValue > 0)
            {
                yRotationValue = 0;
            }
            if (testFlight.transform.rotation.eulerAngles.x < 275f && testFlight.transform.rotation.eulerAngles.x > 260f && yRotationValue < 0)
            {
                yRotationValue = 0;
            }
            Vector3 rotationChanges = new Vector3();

            if (xRotationValue != 0)
            {
                rotationChanges += new Vector3(0, (xRotationValue * Time.deltaTime * xAimResponsitivity), 0);
            }
            if (yRotationValue != 0)
            {
                rotationChanges += new Vector3((yRotationValue * Time.deltaTime * yAimResponsitivity), 0, 0);
            }

            testFlight.SetAimRotation(rotationChanges);
        }
        
    }

    public void SetUpNewThrow(Collision collision)
    {
        //Temporary until UI pulled from TestFlight.cs
        testFlight.TemporaryResetMethod(collision);
    }

    /// <summary>
    /// Rotates the plane towards the Game Object tagged as "Finish".
    /// </summary>
    private void RotateTowardsCheckpoiint()
    {
        GameObject finishObj = lookAtObject;

        Vector3 finishDirection = finishObj.transform.position - testFlight.gameObject.transform.position;

        Quaternion rotation = Quaternion.LookRotation(new Vector3(finishDirection.x, 0, finishDirection.z));

        testFlight.transform.rotation = rotation;
    }

    /// <summary>
    /// Sets the charge bar to active when the plane is being thrown.
    /// </summary>
    private void ChargeBar()
    {
        //TutorialHandler();

        throwing = true;
        aiming = false;
        chargeBarController.enabled = true;
        chargeBarController.chargeBar.gameObject.SetActive(true);
    }

    public float GetStartThrowHeight()
    {
        return startThrowHeight;
    }

    public void SetAimStatus(bool aimStatus)
    {
        aiming = aimStatus;
    }

    public bool GetAimStatus()
    {
        return aiming;
    }

    public void SetThrowStatus(bool throwStatus)
    {
        throwing = throwStatus;
    }

    public bool GetThrowStatus()
    {
        return throwing;
    }

    public float GetChargeBarValue()
    {
        return chargeBarController.chargeBar.value;
    }

    public void SetChargeBarActivity(bool isChargeBarEnabled)
    {
        chargeBarController.chargeBar.gameObject.SetActive(isChargeBarEnabled);
        chargeBarController.enabled = isChargeBarEnabled;
    }

    public void ReactivateChargeBar()
    {
        ChargeBar();
    }

    public void SetRotateTowardObject(GameObject checkpoint)
    {
        lookAtObject = checkpoint;
    }

    //TEMPORARY UNTIL UI FIXED -- CALLED FROM TESTFLIGHT.CS
    public void SetMovePlaneToPos(bool willMovePlaneToPos)
    {
        movePlaneToPos = willMovePlaneToPos;
    }
    public bool GetMovePlaneToPos()
    {
        return movePlaneToPos;
    }
}
