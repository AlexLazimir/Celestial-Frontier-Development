using UnityEngine;
using System.Collections;

public class ShipControllerReference : MonoBehaviour {

    public Rigidbody m_Ship;
    public Camera m_Camera;
    public float m_MouseSensitivity = 100f;
    public float m_AccelerationAmmount = 0.1f;
    public float m_YawRotationAmmount = 10f;
    public bool clearedAltitude = false;

    float ship_Roll, ship_Pitch, ship_Yaw;
    Vector3 m_CurrentRotation;
    Vector3 m_TargetRotation;

    float m_Thrust, downwardThrust;
    Vector3 m_CurrentAcceleration;
    Vector3 m_TargetAcceleration;

    bool controlRotation = true;
    float camera_xRotation, camera_yRotation;
    Vector3 m_CameraDefaultRotation;
    Vector3 m_CameraCurrentRotation;
    Vector3 m_CameraTargetRotation;

	// Use this for initialization
	void Start () {
        m_Ship = GetComponent<Rigidbody>();
        m_CurrentRotation = Vector3.zero;
        m_TargetRotation = Vector3.zero;
        m_TargetAcceleration = Vector3.zero;
        m_CurrentAcceleration = Vector3.zero;

        m_CameraDefaultRotation = m_Camera.transform.localEulerAngles;
        m_CameraTargetRotation = m_CameraDefaultRotation;

        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        CheckCursorState();
        Debug.Log(downwardThrust);
        if (controlRotation)
        {
            ship_Roll = Input.GetAxis("Mouse X") * m_MouseSensitivity * Time.fixedDeltaTime;
            ship_Pitch = Input.GetAxis("Mouse Y") * m_MouseSensitivity * Time.fixedDeltaTime;
            ship_Yaw = Input.GetAxis("Horizontal") * m_YawRotationAmmount * Time.fixedDeltaTime;

            m_CurrentRotation = new Vector3(-ship_Roll, ship_Yaw, ship_Pitch);
            m_TargetRotation = Vector3.Lerp(m_TargetRotation, m_CurrentRotation, 1/m_Ship.mass);

            m_Ship.transform.Rotate(m_TargetRotation);
        }
        else
        {
            camera_xRotation += Input.GetAxis("Mouse Y") * m_MouseSensitivity * Time.fixedDeltaTime;
            camera_yRotation += Input.GetAxis("Mouse X") * m_MouseSensitivity * Time.fixedDeltaTime;

            camera_xRotation = Mathf.Clamp(camera_xRotation, -75, 100);
            camera_yRotation = Mathf.Clamp(camera_yRotation, -110, 110);

            m_CameraCurrentRotation = new Vector3(-camera_xRotation, camera_yRotation, 0);
            m_CameraTargetRotation = m_CameraCurrentRotation + m_CameraDefaultRotation;

            m_Camera.transform.localEulerAngles = m_CameraTargetRotation;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            controlRotation = false;
        }
        else
        {
            controlRotation = true;
            camera_xRotation = 0;
            camera_yRotation = 0;
            m_CameraTargetRotation = Vector3.Lerp(m_CameraTargetRotation, m_CameraDefaultRotation, 0.1f);
            m_Camera.transform.localEulerAngles = m_CameraTargetRotation;
        }

        //! redo
        m_TargetAcceleration = m_Ship.transform.rotation * new Vector3(m_Thrust, 0, 0);


        m_Thrust = Mathf.Clamp(m_Thrust, -50, 100);
        downwardThrust += Input.GetAxis("VerticalThrust") * m_AccelerationAmmount * Time.fixedDeltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            m_Thrust += m_AccelerationAmmount;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            m_Thrust -= m_AccelerationAmmount;
        }
        else if(Input.GetKey(KeyCode.X))
        {
            m_Thrust = 0;
        }

        if (m_Thrust > 0 || m_Thrust < 0)
        {
            m_Ship.AddForce(m_TargetAcceleration * 1000f * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        Vector3 down = m_Ship.transform.TransformDirection(Vector3.down);

        if(!Physics.Raycast(m_Ship.transform.position, down, 20f))
        {
            clearedAltitude = true;
        }
        else
        {
            clearedAltitude = false;
        }


        m_Ship.angularVelocity = Vector3.zero;
    }

    bool lockState = true;

    private void CheckCursorState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockState = !lockState;
        }
        if(lockState)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
