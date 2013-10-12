using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	
	//[System.NonSerialized]
	public Vector3 moveDirection = Vector3.zero;
	public float moveSpeed = 6f;
	private float moveSnap = 50f;
	
	private CharacterController controller;
	private Transform trans;
	
	public GameObject currentCell;
	
	void Start(){
		controller = GetComponent<CharacterController>();
		trans = transform;
	}
	
	void Update(){
		PlayerLookDirection();
		moveDirection *= moveSpeed;
		moveDirection.y -= 50f * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.Space)){
			RaycastHit hit = new RaycastHit();
			Vector3 pos = trans.position;
			pos.y = 0.5f;
			if(Physics.Raycast(pos, trans.forward, out hit, 1f)){
				if(hit.collider.tag == Globals.FORTIFICATION){
					Vector3 axis = Vector3.Cross(trans.up+trans.right, hit.normal);
					if(axis != Vector3.zero){
						float angle = Mathf.Atan2(Vector3.Magnitude(axis), Vector3.Dot(trans.up+trans.right, hit.normal));
						trans.RotateAround(axis, angle);
						trans.position += trans.forward * 4f * Time.deltaTime;
					}
				}
			}
		}
	}
	
	/*void FixedUpdate(){
		Vector3 targetVel = moveDirection * moveSpeed;
		Vector3 deltaVel = targetVel - rigidbody.velocity;
		if(rigidbody.useGravity){
			deltaVel.y = 0;
		}
		rigidbody.AddForce(deltaVel * moveSnap, ForceMode.Acceleration);
	}*/
	
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
	
	void OnTriggerStay(Collider other){
		if(other.tag == "AIPathCell"){
			currentCell = other.gameObject;
		}
	}
	
	void OnTriggerExit(){
		currentCell = null;
	}
}