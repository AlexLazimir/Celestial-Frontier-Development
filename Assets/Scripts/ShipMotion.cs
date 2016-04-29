using UnityEngine;
using System.Collections;

public class ShipMotion : MonoBehaviour {
	// Use this for initialization
	float speed = 0;

    public float speedCap =10;
	public float turnSpeed = 10;

    Rigidbody rigBod;
	Vector3 movementDir = Vector3.zero;

    public float clampAngle = 80.0f;

	private float 
        pitchControl = 0.0f, 
        rollControl = 0.0f, 
        yawControl = 0.0f;

    private bool CursorIsVisible = false;

	void Start () {
        HandleCursorVisibility();

        rigBod = gameObject.GetComponent<Rigidbody>();

		Vector3 rot = transform.localRotation.eulerAngles;
        yawControl = rot.y;
        rollControl = rot.z;
	}
	
	void Update () {
        HandleCursorVisibility();
        if (Input.GetKey(KeyCode.W) && speed < speedCap)
		{
			speed += 0.05f;
		}
		else if(Input.GetKey(KeyCode.S) && speed > 0)
		{
			speed -= 0.05f;
		}
		rigBod.AddForce(transform.forward * speed * 100f);

        float roll = Input.GetAxis("Mouse X");
		float pitch = -Input.GetAxis("Mouse Y");
        float yaw = Input.GetAxis("Horizontal");


		rollControl = roll * turnSpeed * Time.deltaTime;
		pitchControl = pitch * turnSpeed * Time.deltaTime;
        yawControl = yaw * turnSpeed/5 * Time.deltaTime;

        //rollControl = Mathf.Clamp(rollControl, -clampAngle, clampAngle);

        //Quaternion localRotation = Quaternion.Euler(pitchControl, yawControl, rollControl);
        transform.Rotate(new Vector3(pitchControl, yawControl, rollControl));
	}
	void OnGUI()
	{
		GUI.Box(new Rect(Screen.width/2 -((speed*100)/2), Screen.height - 50,speed*100,50),"Speed");
	}

    private void HandleCursorVisibility()
    {
        if(!CursorIsVisible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(CursorIsVisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            CursorIsVisible = !CursorIsVisible;
        }
    }
}
