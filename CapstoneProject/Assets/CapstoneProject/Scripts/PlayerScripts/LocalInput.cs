using UnityEngine;
using System.Collections;

public class LocalInput : MonoBehaviour {
	
	[HideInInspector]
	public PlayerMovement controller;
	private Transform trans;
	
	private Quaternion screenMovementSpace;
	private Vector3 screenMovementForward;
	private Vector3 screenMovementRight;
	
	private float camSmooth = 0.1f;
	private float camDistFromPlayer = 1.5f;
	
	private Vector3 playerOffset;
	private Vector3 cameraVelocity = Vector3.zero;
	
	private Transform cam;

	void Awake(){
		cam = Camera.main.transform;
		
		trans = transform;
		
		playerOffset = cam.position - transform.position;
	}
	
	void Start(){
		controller = GetComponent<PlayerMovement>();
		screenMovementSpace = Quaternion.Euler(0, cam.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;
	}
	
	void Update(){
		PlayerLookDirection();
		
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
	
		
	void PlayerLookDirection(){
		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.up, trans.position);
		// Generate a ray from the cursor position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Determine the point where the cursor ray intersects the plane.
		float hitDist = 0;
		// If the ray is parallel to the plane, Raycast will return false.
		if(playerPlane.Raycast(ray, out hitDist)){
		    // Get the point along the ray that hits the calculated distance.
		    Vector3 targetPoint = ray.GetPoint(hitDist);
		    // Determine the target rotation.
		    Quaternion targetRotation = Quaternion.LookRotation(targetPoint - trans.position);
			// Rotate towards the target point.
			trans.rotation = Quaternion.Slerp(trans.rotation, targetRotation, 1);
		}
	}
}
