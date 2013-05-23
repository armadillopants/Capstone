using UnityEngine;
using System.Collections;

public class LocalInput : MonoBehaviour {
	
	private PlayerMovement controller;
	
	private Quaternion screenMovementSpace;
	private Vector3 screenMovementForward;
	private Vector3 screenMovementRight;
	private float camSmooth = 0.1f;
	private float camDistFromPlayer = 1.5f;
	private Vector3 playerOffset;
	private Vector3 cameraVelocity = Vector3.zero;
	
	private Transform cam;

	void Awake(){
		controller = GetComponent<PlayerMovement>();
		cam = Camera.main.transform;
		
		playerOffset = cam.position - transform.position;
	}
	
	void Start(){
		screenMovementSpace = Quaternion.Euler(0, cam.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;	
	}
	
	void Update(){
		// Get the input vector from keyboard or analog stick
		Vector3 moveDir = Input.GetAxis("Horizontal") * screenMovementRight + Input.GetAxis("Vertical") * screenMovementForward;
	
		if(moveDir != Vector3.zero){
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float dirLength = moveDir.magnitude;
			moveDir /= dirLength;
			
			// Make sure the length is no bigger than 1
			dirLength = Mathf.Min(1, dirLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			dirLength = dirLength * dirLength;
			
			// Multiply the normalized direction vector by the modified length
			moveDir = moveDir * dirLength;
		}
		
		Vector3 camAdjustment = Vector3.zero;
		
		Vector3 cursorScreenPos = Input.mousePosition;
		float halfWidth = Screen.width / 2.0f;
		float halfHeight = Screen.height / 2.0f;
		float maxHalf = Mathf.Max(halfWidth, halfHeight);
		
		// Acquire the relative screen position	
		Vector3 posRel = cursorScreenPos - new Vector3(halfWidth, halfHeight, cursorScreenPos.z);
		posRel.x /= maxHalf;
		posRel.y /= maxHalf;
		
		camAdjustment = posRel.x * screenMovementRight + posRel.y * screenMovementForward;
		camAdjustment.y = 0;
		
		Vector3 camTargetPos = transform.position + playerOffset + camAdjustment * camDistFromPlayer;
	
		// Apply some smoothing to the camera movement
		cam.position = Vector3.SmoothDamp(cam.position, camTargetPos, ref cameraVelocity, camSmooth);
	
		// Apply the direction to PlayerMovement
		controller.moveDirection = moveDir;
	}
}
