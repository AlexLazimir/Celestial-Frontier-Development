using UnityEngine;
using System.Collections;

public class SteeringController : MonoBehaviour {

    public Transform TargetObject;
    public GameObject ShipObject;

    ShipController shipController;
    float MouseX, MouseY;

    Vector3 TargetRotation = Vector3.zero;

    void Start()
    {
        shipController = ShipObject.GetComponent<ShipController>();
        TargetRotation = TargetObject.transform.localEulerAngles;
    }

    private Vector3 InputTest;

    void Update()
    {
        if (shipController.PlayerCanControlShip && !shipController.ControlCamera)
        {
            MouseX += Input.GetAxis("Mouse X");
            MouseY -= Input.GetAxis("Mouse Y");

            MouseY = Mathf.Clamp(MouseY, -40f, 40f);
            MouseX = Mathf.Clamp(MouseX, -60f, 60f);

            if(MouseY > 0f || MouseY < 0f)
            {
                MouseY = Mathf.Lerp(MouseY, 0f, 0.1f);
            }

            if(MouseX > 0f || MouseX < 0f)
            {
                MouseX = Mathf.Lerp(MouseX, 0f, 0.1f);
            }

            InputTest = new Vector3(-MouseX, 0f, -MouseY);

            TargetObject.transform.localRotation = Quaternion.Euler(InputTest);
        }
    }
}
