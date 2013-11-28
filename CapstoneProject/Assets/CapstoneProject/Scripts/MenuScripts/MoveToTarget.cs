using UnityEngine;
using System.Collections;

public class MoveToTarget : MonoBehaviour {
	
	public Transform[] wayPoints;
	public int curWaypoint = 0;
	public int totalWayPoints;
	private Transform trans;
	private Vector3 curTarget;
	private Vector3 moveDirection;
	private float moveSpeed = 15f;

	void Start(){
		trans = transform;
		totalWayPoints = wayPoints.Length;
	}
	
	void Update(){
		if(curWaypoint == totalWayPoints){
			trans.rotation = Quaternion.Euler(0, 90, 0);
			if(trans.GetComponent<Collider>() != null){
				trans.GetComponent<Collider>().enabled = true;
			}
			Destroy(this);
			return;
		} else {
			curTarget = wayPoints[curWaypoint].position;
			moveDirection = curTarget - trans.position;
			trans.rotation = Quaternion.Slerp(trans.rotation, Quaternion.LookRotation(moveDirection), 4*Time.deltaTime);
			if(moveDirection.sqrMagnitude < 0.1f){
				curWaypoint++;
			}
		}
		trans.Translate(Vector3.forward*moveSpeed*Time.deltaTime);
	}
}
