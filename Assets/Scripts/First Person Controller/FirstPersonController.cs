using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour {

    public float MovementSpeed;
    public float MouseSensitivity;

    public Camera cam;
    public Rigidbody playerRBody;

    public Vector3 playerTransformRotation = Vector3.zero;

    public float shipGravity = -5f;

    float horizontal, vertical, playerGravity;
    float horizontalRotation, verticalRotation;

	public GameObject attatchedObj;
	Vector3 attachedPos = Vector3.zero;
	Quaternion attatchedRot = Quaternion.identity;

	bool isAttached = false;

    public bool IsPlayerUsingShip = false;

    Vector3 gravityDirection;
    float gravityDir;

    bool IsGrounded = false;

    void Start()
    {
        //cam = Camera.main;
        playerRBody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCursor();
        if (Input.GetKeyDown(KeyCode.E))
		{
			if (!IsPlayerUsingShip)
			{
				IsPlayerUsingShip = true;
			}
			else if(IsPlayerUsingShip)
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

            Vector3 pos = new Vector3(horizontal, gravityDir, vertical);

            pos = transform.rotation * pos;

            cam.transform.localEulerAngles = new Vector3(-verticalRotation, 0f, 0f);
            playerRBody.transform.Rotate(new Vector3(0f, horizontalRotation, 0f));
            playerRBody.AddForce(pos);

            if (!IsGrounded)
            {
                gravityDir += Physics.gravity.y * Time.deltaTime;
            }
            else if (IsGrounded)
            {
                gravityDir = 0f;
            }
        }
		else if (IsPlayerUsingShip)
		{
			
			attachedPos = attatchedObj.gameObject.transform.transform.position;
			attatchedRot = attatchedObj.gameObject.transform.parent.GetComponent<Rigidbody>().rotation;
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
		Debug.Log(attatchedObj.name.ToString());
		Debug.Log("asdadsas");
	}

}
