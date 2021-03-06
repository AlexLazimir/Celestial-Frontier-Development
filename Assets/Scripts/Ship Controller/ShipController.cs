﻿using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour {

    // Press F4 to enable the cursor
    // Press F3 to enable debugging - WIP -

    //----------------------------------------------------------------
    //! Class variables
    public Camera MainCamera;

    public GameObject MainPlayer;

    FirstPersonController fpsController;

    public Transform SeatPosition;

    public float MouseSensitivity = 100f;

    public float ShipAccelerationAmmount = 100f;

    private float MouseX = 0.0f, MouseY = 0.0f;

	//CharacterController charContr;

    private Rigidbody ShipRigidBody;

    private Collider ShipExteriorCollider;

    //----------------------------------------------------------------
    //! Rotation Controller public variables
    [System.Serializable]
    public class RotationVariables
    {
        public bool IsInverted = false;

        public float ShipRotationSensitivity = 50f;
        public float ShipRotationDamping = 5f;
    }

    public RotationVariables RotationProperties;

    //----------------------------------------------------------------
    //! Movement Controller public variables
    [System.Serializable]
    public class ThrottleVariables
    {
        public float AccelerationAmmount = 0.5f;
        public float EmergencyStopAmmount = 1.5f;
    }

    public ThrottleVariables ThrottleProperties;

    //----------------------------------------------------------------
    //! Camera Controller public variables
    [System.Serializable]
    public class CameraVariables
    {
        public bool IsCameraInverted = false;
        public float 
            MaxVerticalRotation = 110f,
            MinVeritcalRotation = -70f;
        public float
            MaxHorizontalRotation = 70f,
            MinHorizontalRotation = -70f;
        public float RotationDamping = 5f;
    }

    public CameraVariables CameraProperties;

    private void Start()
    {
		//charContr = gameObject.GetComponent<CharacterController>();
        ShipRigidBody = GetComponent<Rigidbody>();
		ShipExteriorCollider = gameObject.GetComponent<Collider>();

        fpsController = MainPlayer.GetComponent<FirstPersonController>();

        MainCamera = Camera.main;

        CameraDefaultRotation = MainCamera.transform.localEulerAngles;
        CameraTargetRotation = CameraDefaultRotation;

        CursorController();
    }

    [SerializeField]
    public bool PlayerCanControlShip = false;

    private float timer = 0f;

    //set it to fixed update and change cursor to update and other methods to update()

    private void FixedUpdate()
    {
		//Debug.Log(transform.forward * ForwardThrottle * ShipAccelerationAmmount * Time.deltaTime);
		//charContr.Move(transform.forward * ForwardThrottle * ShipAccelerationAmmount * Time.deltaTime * 0.001f);
		//charContr.Move(transform.up * UpwardThrottle * ShipAccelerationAmmount * Time.deltaTime * 0.001f);
		ShipRigidBody.velocity = (transform.forward * (int) ForwardThrottle* ShipAccelerationAmmount * Time.deltaTime);

		//ShipRigidBody.velocity += (transform.up * UpwardThrottle * ShipAccelerationAmmount * Time.deltaTime);
		if (fpsController.IsPlayerUsingShip)
        {
            MouseX = Input.GetAxis("Mouse X");
            MouseY = Input.GetAxis("Mouse Y");

            ControlShipRotation();
            ControlShipMovement();

            //MainPlayer.transform.position = SeatPosition.transform.position;
           // MainPlayer.transform.rotation = ShipRigidBody.transform.rotation;
        }

        /*if (Input.GetKeyDown(KeyCode.E))
        {
            if (!PlayerCanControlShip)
            {
               // fpsController.IsPlayerUsingShip = true;
                PlayerCanControlShip = true;
            }
            else if(PlayerCanControlShip)
            {
                if (ForwardThrottle == 0)
                {
                    MainPlayer.transform.position = SeatPosition.transform.position;

                    fpsController.ResetValues();

                    fpsController.playerTransformRotation = ShipRigidBody.transform.eulerAngles;
                   // fpsController.IsPlayerUsingShip = false;

                    PlayerCanControlShip = false;
                }
            }
        }*/

        ShipRigidBody.angularVelocity = Vector3.zero;
        CursorController();
    }

    //----------------------------------------------------------------
    // GUI && its local variables

    private float GUIMultiplyer = 5f;

    private int TypeCastedForwardThrottle = 0;
    private int TypeCastedUpwardThrottle = 0;

    void OnGUI()
    {
        TypeCastedForwardThrottle = (int) ForwardThrottle;
        TypeCastedUpwardThrottle = (int)UpwardThrottle;

        GUI.Label(new Rect(250, Screen.height / 2 - 25, 200, 25), "Throttle: " + TypeCastedForwardThrottle.ToString());
        GUI.Box(new Rect(250, Screen.height/2, ForwardThrottle * GUIMultiplyer, 50), "");

        GUI.Label(new Rect(250, Screen.height / 2 - 100, 200, 25), "Upward Throttle: " + TypeCastedUpwardThrottle.ToString());
        GUI.Box(new Rect(250, Screen.height / 2 - 75, UpwardThrottle * GUIMultiplyer, 50), "");
    }

    //----------------------------------------------------------------
    // Cursor Controller && its local variables

    private bool ShowCursor = false;

    private void CursorController()
    {
        if(Input.GetKeyDown(KeyCode.F4))
        {
            ShowCursor = !ShowCursor;
        }

        if(!ShowCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if(ShowCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //----------------------------------------------------------------
    // Rotation Controller && its local variables

    private float Roll, Pitch, Yaw;

    private Vector3 CurrentShipRotation;

    private Vector3 TargetShipRotation;

    private float InvertedMult;

    private void ControlShipRotation()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            ControlCamera = true;
        }
        else
        {
            ControlCamera = false;
            LerpCameraRotationToDefault();
        }

        if(!ControlCamera)
        {
            HandleShipRotation();
        }
        else if(ControlCamera)
        {
            RunCameraController();
        }
    }

    private void HandleInvertedControl()
    {
        if (RotationProperties.IsInverted)
        {
            InvertedMult = -1f;
        }
        else
        {
            InvertedMult = 1f;
        }
    }

    private void HandleShipRotation()
    {
        HandleInvertedControl();

        Roll = Input.GetAxis("Mouse X") * RotationProperties.ShipRotationSensitivity * Time.deltaTime;
        Yaw = Input.GetAxis("Horizontal") * RotationProperties.ShipRotationSensitivity * Time.deltaTime;
        Pitch = Input.GetAxis("Mouse Y") * RotationProperties.ShipRotationSensitivity * Time.deltaTime;

        CurrentShipRotation = new Vector3(Pitch * InvertedMult, Yaw, -Roll);
        TargetShipRotation = Vector3.Lerp(TargetShipRotation, CurrentShipRotation, RotationProperties.ShipRotationDamping);


        //! look into how to rotate objects from local space to world space
        ShipRigidBody.transform.Rotate(TargetShipRotation, Space.Self);
		//charContr.transform.Rotate(TargetShipRotation,Space.Self);
    }

    private void LerpCameraRotationToDefault()
    {
        CameraHorizontalRotation = 0.0f;
        CameraVerticalRotation = 0.0f;

        //! experiment with slerp and lerp
        CameraTargetRotation = Vector3.Slerp(CameraTargetRotation, CameraDefaultRotation, CameraProperties.RotationDamping * Time.deltaTime);
        MainCamera.transform.localEulerAngles = CameraTargetRotation;
    }

    //----------------------------------------------------------------
    // Camera Controller && its local variables

    [SerializeField]
    public bool ControlCamera = false;

    private float CameraHorizontalRotation = 0.0f, CameraVerticalRotation = 0.0f;

    private Vector3 CameraDefaultRotation;

    private Vector3 CameraCurrentRotation = Vector3.zero;

    private Vector3 CameraTargetRotation;

    private float CameraInvertedMult; 

    private void CameraInvertedControl()
    {
        if(CameraProperties.IsCameraInverted)
        {
            CameraInvertedMult = 1f;
        }
        else
        {
            CameraInvertedMult = -1f;
        }
    }

    private void RunCameraController()
    {
        CameraInvertedControl();
        CameraVerticalRotation += MouseY * MouseSensitivity * Time.deltaTime;
        CameraHorizontalRotation += MouseX * MouseSensitivity * Time.deltaTime;

        CameraVerticalRotation = Mathf.Clamp(CameraVerticalRotation, CameraProperties.MinVeritcalRotation, CameraProperties.MaxVerticalRotation);
        CameraHorizontalRotation = Mathf.Clamp(CameraHorizontalRotation, CameraProperties.MinHorizontalRotation, CameraProperties.MaxHorizontalRotation);

        CameraCurrentRotation = new Vector3(CameraVerticalRotation * CameraInvertedMult, CameraHorizontalRotation, 0);
        CameraTargetRotation = CameraCurrentRotation + CameraDefaultRotation;

        MainCamera.transform.localEulerAngles = CameraTargetRotation;
    }

    //----------------------------------------------------------------
    // Movement Controller && its local variables

	public float ForwardThrottle = 0.0f;

    private float UpwardThrottle = 0.0f;

    private float ThrottleAmmount = 0.0f;

    private Vector3 ShipAcceleration;

    private bool b_EmergencyStop = false;

    // change name to industrial space, civilian space, etc...

    private bool ShipWithinLocalSpace = false;

    private float 
        LocalSpaceFlightSpeed = 0.75f,
        UniversalSpaceFlightSpeed = 1f,
        CurrentSpaceFlightSpeed;

    private float
        MaximumLocalSpaceThrottle,
        MinimumLocalSpaceThrottle;

    private void ControlShipMovement()
    {
        GetThrottleAmmount();
    }

    private void GetThrottleAmmount()
    {
        CheckCurrentArea();

        // Smooth it out by possibly add a Mathf.Lerp?
        // possibly multiply by the engine type to get even more acceleration or reverse?

        ThrottleAmmount = ThrottleProperties.AccelerationAmmount * CurrentSpaceFlightSpeed * Time.deltaTime;

        if (!b_EmergencyStop)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                ForwardThrottle += ThrottleAmmount;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                ForwardThrottle -= ThrottleAmmount;
            }
            
            if(Input.GetKey(KeyCode.R))
            {
                UpwardThrottle += ThrottleAmmount * 2;
            }
            if (!IsGrounded)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    UpwardThrottle -= ThrottleAmmount * 2;
                }
            }

            if(UpwardThrottle > 0f || UpwardThrottle < 0f)
            {
                UpwardThrottle = Mathf.Lerp(UpwardThrottle, 0f, 0.025f);
            }

            //Debug.Log(UpwardThrottle);

            if (Input.GetKeyDown(KeyCode.X))
            {
                b_EmergencyStop = true;
            }
        }
        else
        {
            EmergencyStop(5f);
        }
    }

    private void CheckCurrentArea()
    {
        ClampThrottleAmmount();
        if (ShipWithinLocalSpace)
        {
            CurrentSpaceFlightSpeed = LocalSpaceFlightSpeed;
        }
        else if(!ShipWithinLocalSpace)
        {
            CurrentSpaceFlightSpeed = UniversalSpaceFlightSpeed;
        }
    }

    private void ClampThrottleAmmount()
    {
        UpwardThrottle = Mathf.Clamp(UpwardThrottle, -50f, 50f);
        if (ShipWithinLocalSpace)
        {
            // check if the player is speeding through?
            /*if(Throttle > MaximumLocalSpaceThrottle + 10f || Throttle < MinimumLocalSpaceThrottle - 10f)
            {
                EmergencyStop(MaximumLocalSpaceThrottle, MinimumLocalSpaceThrottle);
            }*/
            ForwardThrottle = Mathf.Clamp(ForwardThrottle, MinimumLocalSpaceThrottle, MaximumLocalSpaceThrottle);
        }
        else if(!ShipWithinLocalSpace)
        {
            ForwardThrottle = Mathf.Clamp(ForwardThrottle, -40f, 100f);
        }
    }

    private void EmergencyStop(float EmergencyThrottleAmmount)
    {
        if (b_EmergencyStop)
        {
            if (ForwardThrottle > 0f || ForwardThrottle < 0f)
            {
                ForwardThrottle = Mathf.Lerp(ForwardThrottle, 0f, 0.10f);

                if ((int) ForwardThrottle == 0)
                {
                    b_EmergencyStop = false;
                }
            }
        }
    }

    //----------------------------------------------------------------
    // Trigger && Collider Controller

    private bool IsGrounded = false;

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Local Flight Space")
        {
            ShipWithinLocalSpace = true;

            MinimumLocalSpaceThrottle = col.GetComponent<SpaceAreaController>().MinimumThrottle;
            MaximumLocalSpaceThrottle = col.GetComponent<SpaceAreaController>().MaximumThrottle;

            Debug.Log("You have entered Local Flight Space, " + col.name);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Local Flight Space")
        {
            ShipWithinLocalSpace = false;
            Debug.Log("You have exited Local Flight Space, " + col.name);
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        IsGrounded = true;
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        IsGrounded = false;
    }
}
