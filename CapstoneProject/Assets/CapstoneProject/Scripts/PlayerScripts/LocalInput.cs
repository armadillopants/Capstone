using UnityEngine;

public class LocalInput : MonoBehaviour {
	
	[HideInInspector]
	public PlayerMovement controller;
	private Transform trans;
	private GameObject cursorPrefab;
	private Transform cursorObject;
	
	private Quaternion screenMovementSpace;
	private Vector3 screenMovementForward;
	private Vector3 screenMovementRight;
	
	private float camSmooth = 0.1f;
	private float camDistFromPlayer = 5f;
	
	private Vector3 playerOffset;
	private Vector3 cameraVelocity = Vector3.zero;
	
	private Transform cam;
	private float cursorPlaneHeight = 0;
	private float cursorFacingCam = 0;
	private float cursorSmallerWithDistance = 0;
	private Plane playerMovementPlane;
	
	private Texture2D pistolCursor;
	private Texture2D rifleCursor;
	private Texture2D launcherCursor;
	private Texture2D specialCursor;
	
	private WeaponSelection selection;

	void Awake(){
		cam = Camera.main.transform;
		
		trans = transform;
		
		playerOffset = cam.position - transform.position;
		
		cursorPrefab = (GameObject)Resources.Load("Cursor", typeof(GameObject));
		pistolCursor = (Texture2D)Resources.Load("pistols_blue", typeof(Texture2D));
		rifleCursor = (Texture2D)Resources.Load("rifles_blue", typeof(Texture2D));
		launcherCursor = (Texture2D)Resources.Load("launchers_blue", typeof(Texture2D));
		specialCursor = (Texture2D)Resources.Load("specials_blue", typeof(Texture2D));
		
		cursorObject = ((GameObject)Instantiate(cursorPrefab)).transform;
		cursorObject.name = cursorPrefab.name;
		cursorObject.GetComponentInChildren<MeshRenderer>().material.mainTexture = rifleCursor;
		
		playerMovementPlane = new Plane(trans.up, trans.position+trans.up*cursorPlaneHeight);
	}
	
	void Start(){
		controller = GetComponent<PlayerMovement>();
		selection = GetComponentInChildren<WeaponSelection>();
		screenMovementSpace = Quaternion.Euler(0, cam.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;
	}
	
	void Update(){
		PlayerLookDirection();
		
		// Get the input vector from keyboard or analog stick
		Vector3 moveDir = Input.GetAxis("Horizontal") * screenMovementRight + Input.GetAxis("Vertical") * screenMovementForward;
		
		if(trans.position.y > 0){
			moveDir.y = 0;
			trans.position = new Vector3(moveDir.x, moveDir.y, moveDir.z);
		}
	
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
		
		playerMovementPlane.normal = trans.up;
		playerMovementPlane.distance = -trans.position.y + cursorPlaneHeight;
		
		Vector3 camAdjustment = Vector3.zero;
		
		Vector3 cursorScreenPos = Input.mousePosition;
		Vector3 cursorWorldPos = ScreenPointToWorldPointOnPlane(cursorScreenPos, playerMovementPlane, Camera.main);
		
		float halfWidth = Screen.width / 2.0f;
		float halfHeight = Screen.height / 2.0f;
		float maxHalf = Mathf.Max(halfWidth, halfHeight);
		
		// Acquire the relative screen position	
		Vector3 posRel = cursorScreenPos - new Vector3(halfWidth, halfHeight, cursorScreenPos.z);
		posRel.x /= maxHalf;
		posRel.y /= maxHalf;
		
		camAdjustment = posRel.x * screenMovementRight + posRel.y * screenMovementForward;
		camAdjustment.y = 0;
		
		controller.faceDirection = (cursorWorldPos - trans.position);
		controller.faceDirection.y = 0;
		
		HandleCursorAlignment(cursorWorldPos);
		
		Vector3 camTargetPos = transform.position + playerOffset + camAdjustment * camDistFromPlayer;
	
		// Apply some smoothing to the camera movement
		cam.position = Vector3.SmoothDamp(cam.position, camTargetPos, ref cameraVelocity, camSmooth);
	
		// Apply the direction to PlayerMovement
		controller.moveDirection = moveDir;
		
		if(UIManager.Instance.displayUI && !UIManager.Instance.isPaused && !selection.changingWeapons){
			cursorObject.GetComponentInChildren<MeshRenderer>().enabled = true;
		} else {
			cursorObject.GetComponentInChildren<MeshRenderer>().enabled = false;
		}
		
		for(int i=0; i<selection.weaponSlots.Count; i++){
			if(selection.weaponSlots[i] != null){
				if(selection.weaponSlots[i].gameObject.activeSelf){
					switch(i){
					case 0:
						cursorObject.GetComponentInChildren<MeshRenderer>().material.mainTexture = pistolCursor;
						break;
					case 1:
						cursorObject.GetComponentInChildren<MeshRenderer>().material.mainTexture = rifleCursor;
						break;
					case 2:
						cursorObject.GetComponentInChildren<MeshRenderer>().material.mainTexture = launcherCursor;
						break;
					case 3:
						cursorObject.GetComponentInChildren<MeshRenderer>().material.mainTexture = specialCursor;
						break;
					}
				}
			}
		}
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
	
	private Vector3 PlaneRayIntersection(Plane plane, Ray ray){
		float dist;
		plane.Raycast(ray, out dist);
		return ray.GetPoint(dist);
	}

	private Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera cam){
		// Set up a ray corresponding to the screen position
		Ray ray = cam.ScreenPointToRay(screenPoint);
		
		// Find out where the ray intersects with the plane
		return PlaneRayIntersection(plane, ray);
	}
	
	void HandleCursorAlignment(Vector3 cursorWorldPosition) {
		if(!cursorObject){
			return;
		}
		
		// HANDLE CURSOR POSITION
		// Set the position of the cursor object
		cursorObject.position = cursorWorldPosition;

		// Hide mouse cursor when within screen area, since we're showing game cursor instead
		Screen.showCursor = (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height);
		
		
		// HANDLE CURSOR ROTATION
		Quaternion cursorWorldRotation = cursorObject.rotation;
		if(controller.faceDirection != Vector3.zero){
			cursorWorldRotation = Quaternion.LookRotation(controller.faceDirection);
		}
		
		// Calculate cursor billboard rotation
		Vector3 cursorScreenspaceDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position + trans.up * cursorPlaneHeight);
		cursorScreenspaceDirection.z = 0;
		Quaternion cursorBillboardRotation = cam.rotation * Quaternion.LookRotation(cursorScreenspaceDirection, -Vector3.forward);
		
		// Set cursor rotation
		cursorObject.rotation = Quaternion.Slerp(cursorWorldRotation, cursorBillboardRotation, cursorFacingCam);
		
		
		// HANDLE CURSOR SCALING
		// The cursor is placed in the world so it gets smaller with perspective.
		// Scale it by the inverse of the distance to the camera plane to compensate for that.
		float compensatedScale = 0.1f * Vector3.Dot(cursorWorldPosition - cam.position, cam.forward);
		
		// Make the cursor smaller when close to character
		float cursorScaleMultiplier = Mathf.Lerp(0.7f, 1.0f, Mathf.InverseLerp(0.5f, 4.0f, controller.faceDirection.magnitude));
		
		// Set the scale of the cursor
		cursorObject.localScale = Vector3.one * Mathf.Lerp(compensatedScale, 1, cursorSmallerWithDistance) * cursorScaleMultiplier;
		
		cursorSmallerWithDistance = Mathf.Clamp01(cursorSmallerWithDistance);
	}
}
