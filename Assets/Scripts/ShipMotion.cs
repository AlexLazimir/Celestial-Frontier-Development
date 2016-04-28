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
	private float rotY = 0.0f;
	private float rotX = 0.0f;
	void Start () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		rigBod = gameObject.GetComponent<Rigidbody>();
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.W) && speed < speedCap)
		{
			speed += 0.05f;
		}
		else if(Input.GetKey(KeyCode.S) && speed > 0)
		{
			speed -= 0.05f;
		}
		rigBod.AddForce(transform.forward * speed);
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		rotY += mouseX * turnSpeed * Time.deltaTime;
		rotX += mouseY * turnSpeed * Time.deltaTime;

		rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

		Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
		transform.rotation = localRotation;
	}
	void OnGUI()
	{
		GUI.Box(new Rect(Screen.width/2 -((speed*100)/2), Screen.height - 50,speed*100,50),"Speed");
	}
}
