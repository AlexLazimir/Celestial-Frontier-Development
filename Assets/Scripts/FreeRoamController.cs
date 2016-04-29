using UnityEngine;
using System.Collections;

public class FreeRoamController : MonoBehaviour {

    public float MouseSensitivity = 100f;
    public float MovementSpeed = 10f;

    private Camera MainCamera;

    private float CameraVerticalRotation = 0.0f, CameraHorizontalRotation = 0.0f;
    private float ForwardMovement = 0.0f, SidewaysMovement = 0.0f, VertialMovement;

    void Start()
    {
        MainCamera = Camera.main;
    }

    private Vector3 CameraRotation = Vector3.zero;
    private Vector3 CameraMovement = Vector3.zero;

    void Update()
    {
        CameraVerticalRotation += Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        CameraHorizontalRotation += Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;

        ForwardMovement = Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime;
        SidewaysMovement = Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime;

        CameraRotation = new Vector3(-CameraVerticalRotation, CameraHorizontalRotation, 0f);
        CameraMovement = new Vector3(SidewaysMovement, 0f, ForwardMovement);

        MainCamera.transform.localEulerAngles = CameraRotation;
        MainCamera.transform.Translate(CameraMovement);
    }

}
