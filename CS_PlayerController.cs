using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_PlayerController : MonoBehaviour {

	// PLAYER MOTOR VARIABLES
	[SerializeField]
	private Camera cam;

	private Vector3 velocity = Vector3.zero;
	private Rigidbody rb;
	private CapsuleCollider col;
	private Vector3 rotation = Vector3.zero;
	private float camRotationX = 0f;
	private float currentCamRotX = 0f;

	[SerializeField]
	private float cameraRotLimit = 85f;
	[SerializeField]
	private float jumpForce = 5f;
	[SerializeField]
	private LayerMask groundLayers;
	// PLAYER CONTROLLER VARIABLES
	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 3f;

	//private animator anim;

	//OBJECT PICKUP & DROP VARIABLES
	static bool carrying;
	static GameObject carriedObject;
	public float distance;
	public float smooth;
	GameObject mainCam;

	//SCRIPT START FUNCTION
	void Start()
	{
		//PLAYER MOTOR
		rb = GetComponent<Rigidbody>();
		col = GetComponent<CapsuleCollider>();
		//PLAYER CONTROLLER
		//anim = GetComponent<Animator>();
		mainCam = GameObject.FindWithTag("MainCamera");
	}

	//PERFORMING CALCULATIONS
	void Update()
	{
		//Object Pickup & Drop
		if (carrying)
		{
			carry(carriedObject);
			checkDrop();
			//rotateObject();
		} else
		{
			pickup();
		}

		//Movement & Mouse Look
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		//Movement
		float xMov = Input.GetAxis("Horizontal");  // Between -1 and 1
		float zMov = Input.GetAxis("Vertical");    // Between -1 and 1
		Vector3 movHorizontal = transform.right * xMov;
		Vector3 movVertical = transform.forward * zMov;
		Vector3 _velocity = (movHorizontal + movVertical) * speed;
		Move(_velocity);

		//anim.SetFloat("zMov", zMov);
		//anim.SetFloat("xMov", xMov);

		//Mouse Look
		float yRot = Input.GetAxisRaw("Mouse X");
		Vector3 _rotation = new Vector3 (0f, yRot, 0f) * lookSensitivity;
		Rotate(_rotation);

		//Mouse Look
		float xRot = Input.GetAxisRaw("Mouse Y");
		float _camRotationX = xRot * lookSensitivity;
		RotateCam(-_camRotationX);

		//Debug Tools REMOVE IN PLAYABLE BUILDS
		if(Input.GetKeyDown(KeyCode.R))
		{
			if(carrying)
			{
				CS_PlayerController.dropObject();
			}
			SceneManager.LoadScene("US_MechanicTest");
		}
	}

	void Move (Vector3 _velocity)
	{
		velocity = _velocity;
	}
	void Rotate(Vector3 _rotation)
	{
		rotation = _rotation;
	}
	void RotateCam(float _camRotationX)
	{
		camRotationX = _camRotationX;
	}

	void FixedUpdate()
	{
		PerformMovement();
		PerformRotation();
		PerformJump();
	}
	void PerformMovement()
	{
		if(velocity != Vector3.zero)
		{
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
		}
	}
	void PerformRotation()
	{
		rb.MoveRotation(rb.rotation * Quaternion.Euler (rotation));
		if (cam != null)
		{
				currentCamRotX += camRotationX;
				currentCamRotX = Mathf.Clamp(currentCamRotX, -cameraRotLimit, cameraRotLimit);
				cam.transform.localEulerAngles = new Vector3(currentCamRotX, 0f, 0f);
		}
	}
	void PerformJump()
	{
		if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}
	public bool IsGrounded()
	{
		return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x,
			col.bounds.min.y, col.bounds.center.z), col.radius * .9f, groundLayers);
	}

	//OBJECT PICKUP & DROP
	void rotateObject()
	{
		carriedObject.transform.Rotate(5,10,15);
	}
	void carry(GameObject Obj)
	{
		Obj.transform.position = mainCam.transform.position + mainCam.transform.forward * distance;
		Obj.transform.rotation = Quaternion.identity;
	}
	void pickup()
	{
		if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			int x = Screen.width/2;
			int y = Screen.height/2;

			Ray ray = mainCam.GetComponent<Camera>().ScreenPointToRay(new Vector3(x,y));
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				Pickupable pObj = hit.collider.GetComponent<Pickupable>();
				if(pObj != null && Vector3.Distance(pObj.gameObject.transform.position, mainCam.transform.position) < 2)
				{
					carrying = true;
					carriedObject = pObj.gameObject;
					pObj.gameObject.GetComponent<Rigidbody>().useGravity = false;
				}
			}
		}
	}
	void checkDrop()
	{
		if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			CS_PlayerController.dropObject();
		}
	}
	public static void dropObject()
	{
		carrying = false;
		carriedObject.gameObject.GetComponent<Rigidbody>().useGravity = true;
		carriedObject = null;
	}
}
