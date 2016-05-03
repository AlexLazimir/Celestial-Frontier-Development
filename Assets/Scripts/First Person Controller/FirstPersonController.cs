using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {

    public float MovementSpeed;
    public float MouseSensitivity;

    public Camera cam;
    public Rigidbody playerRBody;

    public Vector3 playerTransformRotation = Vector3.zero;

    //public float shipGravity = -5f;

    float horizontal, vertical;
    float horizontalRotation, verticalRotation;

	public GameObject attatchedObj;
	Vector3 attachedPos = Vector3.zero;
	Quaternion attatchedRot = Quaternion.identity;
	//CharacterController charContr;

	bool isAttached = false;

    public bool IsPlayerUsingShip = false;

    float gravityDir;

    bool IsGrounded = false;

    void Start()
    {
		//charContr = gameObject.GetComponent<CharacterController>();
        //cam = Camera.main;
        playerRBody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
		//charContr.transform.localPosition.Set(charContr.transform.localPosition.x,1.07f,charContr.transform.localPosition.z);
        HandleCursor();
		if (Input.GetKeyDown(KeyCode.E) && attatchedObj.name == "SeatPosition")
		{
			if (!IsPlayerUsingShip)
			{
				IsPlayerUsingShip = true;
			}
			else if(IsPlayerUsingShip && attatchedObj.name == "SeatPosition")
			{

				IsPlayerUsingShip = false;
			}
		}
        if (!IsPlayerUsingShip)
		{
            horizontal = Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime;
            vertical = Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime;

            horizontalRotation = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            verticalRotation += Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

            verticalRotation = Mathf.Clamp(verticalRotation, -70, 110);

            Vector3 pos = new Vector3(horizontal, 0f, vertical);

            pos = transform.rotation * pos;

            //Debug.Log(IsGrounded);

            cam.transform.localEulerAngles = new Vector3(-verticalRotation, 0f, 0f);
			playerRBody.transform.Rotate(new Vector3(0f, horizontalRotation, 0f));
            //playerRBody.AddForce(pos);
			playerRBody.velocity = transform.parent.GetComponent<Rigidbody>().velocity + pos * 500f;
            //Debug.Log("Player Velocity: " + playerRBody.velocity + " " + "Parent Velocity: " + transform.parent.GetComponent<Rigidbody>().velocity);
            playerRBody.AddForce(transform.up * gravityDir);
			//charContr.Move(transform.forward * vertical);
			//charContr.Move(transform.right * horizontal);
			//charContr.Move(transform.parent.forward * transform.parent.gameObject.GetComponent<ShipController>().ForwardThrottle * transform.parent.gameObject.GetComponent<ShipController>().ShipAccelerationAmmount * Time.deltaTime * 0.001f);
			//charContr.SimpleMove(transform.forward * vertical);
			if (!IsGrounded)
            {
				//charContr.Move(-transform.up * 0.5f);
               gravityDir += Physics.gravity.y;
            }
            else if (IsGrounded)
            {
               gravityDir = 0f;
            }
        }
		else if (IsPlayerUsingShip)
		{
			
			attachedPos = attatchedObj.gameObject.transform.transform.position;
			attatchedRot = attatchedObj.gameObject.transform.parent.transform.rotation;
			gameObject.transform.position = attachedPos;
			gameObject.transform.rotation = attatchedRot;
		}
    }

    public void ResetValues()
    {
        vertical = 0f;
        horizontal = 0f;
        verticalRotation = 0f;
        horizontalRotation = 0f;
    }

    private void HandleCursor()
    {
        if(Input.GetKeyDown(KeyCode.F4))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else if(Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
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
	void OnTriggerEnter (Collider colTrig)
	{
		attatchedObj = colTrig.gameObject;
		//Debug.Log(attatchedObj.name.ToString());
	}
}
