using UnityEngine;

public class PlayerMovement : PlayerMotor {
	
	public float moveSpeed = 6f;
	private float moveSnap = 1f;
	private float turnSmooth = 0.3f;
	
	private Transform trans;
	
	void Start(){
		trans = transform;
	}
	
	void FixedUpdate(){
		// Handle movement of player
		Vector3 targetVel = moveDirection * moveSpeed;
		Vector3 deltaVel = targetVel - rigidbody.velocity;
		
		if(rigidbody.useGravity){
			deltaVel.y = 0;
		}
		rigidbody.position += deltaVel * moveSnap * Time.deltaTime;
		
		// Handle facing direction
		Vector3 faceDir = faceDirection;
		if(faceDir == Vector3.zero){
			faceDir = moveDirection;
		}
		
		// Make character rotate towards target rotation
		if(faceDir == Vector3.zero){
			rigidbody.angularVelocity = Vector3.zero;
		} else {
			float rotAngle = AngleAroundAxis(trans.forward, faceDir, Vector3.up);
			rigidbody.angularVelocity = (Vector3.up * rotAngle * turnSmooth);
		}
	}
	
	private float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis){
		// Project A and B onto the plane orthogonal target axis
		dirA = dirA - Vector3.Project(dirA, axis);
		dirB = dirB - Vector3.Project(dirB, axis);
		
		// Find (positive) angle between A and B
		float angle = Vector3.Angle(dirA, dirB);
		
		// Return angle multiplied with 1 or -1
		return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
	}
}