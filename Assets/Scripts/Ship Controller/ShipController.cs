using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
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
        ShipRigidBody = GetComponent<Rigidbody>();
        ShipExteriorCollider = ShipRigidBody.GetComponent<Collider>();

        fpsController = MainPlayer.GetComponent<FirstPersonController>();

        MainCamera = Camera.main;

        CameraDefaultRotation = MainCamera.transform.localEulerAngles;
        CameraTargetRotation = CameraDefaultRotation;

        CursorController();
    }

    private bool PlayerCanControlShip = false;

    private float timer = 0f;

    private void Update()
    {
        if (PlayerCanControlShip)
        {
            MouseX = Input.GetAxis("Mouse X");
            MouseY = Input.GetAxis("Mouse Y");

            ControlShipRotation();
            ControlShipMovement();

            MainPlayer.transform.position = SeatPosition.transform.position;
            MainPlayer.transform.rotation = ShipRigidBody.transform.rotation;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!PlayerCanControlShip)
            {
                fpsController.IsPlayerUsingShip = true;
                PlayerCanControlShip = true;
            }
            else if(PlayerCanControlShip)
            {
                if (Throttle == 0)
                {
                    MainPlayer.transform.position = SeatPosition.transform.position;

                    fpsController.ResetValues();

                    fpsController.playerTransformRotation = ShipRigidBody.transform.eulerAngles;
                    fpsController.IsPlayerUsingShip = false;

                    PlayerCanControlShip = false;
                }
            }
        }

        ShipRigidBody.angularVelocity = Vector3.zero;
        CursorController();
    }

    //----------------------------------------------------------------
    // GUI && its local variables

    private float GUIMultiplyer = 5f;

    private int TypeCastedThrottle;

    void OnGUI()
    {
        TypeCastedThrottle = (int)Throttle;
        GUI.Box(new Rect(250, Screen.height/2, Throttle * GUIMultiplyer, 50), "");
        GUI.TextArea(new Rect(250, Screen.height / 2 - 25, 100, 25), "Throttle: " + TypeCastedThrottle.ToString());
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

    private bool ControlCamera;

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

    private float Throttle = 0.0f;

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

        ShipRigidBody.AddForce(transform.forward * Throttle * ShipAccelerationAmmount * Time.fixedDeltaTime, ForceMode.Acceleration);
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
                Throttle += ThrottleAmmount;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                Throttle -= ThrottleAmmount;
            }

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
        if (ShipWithinLocalSpace)
        {
            // check if the player is speeding through?
            /*if(Throttle > MaximumLocalSpaceThrottle + 10f || Throttle < MinimumLocalSpaceThrottle - 10f)
            {
                EmergencyStop(MaximumLocalSpaceThrottle, MinimumLocalSpaceThrottle);
            }*/
            Throttle = Mathf.Clamp(Throttle, MinimumLocalSpaceThrottle, MaximumLocalSpaceThrottle);
        }
        else if(!ShipWithinLocalSpace)
        {
            Throttle = Mathf.Clamp(Throttle, -40, 100);
        }
    }

    private void EmergencyStop(float EmergencyThrottleAmmount)
    {
        if (b_EmergencyStop)
        {
            if (Throttle > EmergencyThrottleAmmount)
            {
                Throttle -= ThrottleProperties.EmergencyStopAmmount * Time.deltaTime;
            }
            else if (Throttle < -EmergencyThrottleAmmount)
            {
                Throttle += ThrottleProperties.EmergencyStopAmmount * Time.deltaTime;
            }
            else
            {
                Throttle = 0;
                b_EmergencyStop = false;
            }
        }
    }

    //----------------------------------------------------------------
    // Trigger && Collider Controller

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
}
