using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointActor : MonoBehaviour {
	
	public GameObject path; // Waypoint master
	public float moveSpeed;
	public float turnSpeed;
	public float gravity;
	public float minDist = 0;
	public float rayRate;
	
	private WaypointMaster master;
	private List<Vector3> pathToFollow = new List<Vector3>();
	private CharacterController controller;
	private bool coroutine = true;
	private bool seeDest = true;
	
	private Vector3 destination; // Go to position
	public GameObject target = null; // Game object to follow
	private bool nearDest = false; // If actor near destination
	private Vector3 lastPos; // Check if actor isnt stuck
	private Vector3 moveDir;
	
	void Awake(){
		target = GameObject.FindWithTag("Player");
		path = GameObject.Find("WaypointMaster");
		master = path.GetComponent<WaypointMaster>();
		controller = GetComponent<CharacterController>();
	}
	
	void Update(){
		if(master){
			if(coroutine){
				StartCoroutine(LookForDestination());
				StartCoroutine(FindPath());
				StartCoroutine(CheckLastPosition());
			}
			
			// Move actor along path
			if(pathToFollow.Count != 0 && !nearDest){
				if(target){
					pathToFollow[pathToFollow.Count-1] = target.transform.position;
				} else {
					pathToFollow[pathToFollow.Count-1] = destination;
				}
				
				Vector3 pathPos = pathToFollow[0];
				Vector3 dir = pathPos - transform.position;
				Quaternion rot = Quaternion.LookRotation(dir);
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
				dir.Normalize();
				
				if(controller.isGrounded){
					//transform.position += transform.forward * moveSpeed * Time.deltaTime;
					moveDir = transform.forward * (moveSpeed/100);
				}
				
				if(Vector3.Distance(transform.position, pathToFollow[0]) < minDist){
					pathToFollow.RemoveAt(0);
				}
			}
			
			//moveDir.y -= gravity * Time.deltaTime;
			
			controller.Move(moveDir);
		}
	}
	
	void GoTo(Vector3 dest){
		destination = dest;
		
		// Try to find path
		pathToFollow.Clear();
		master.FindPath(transform.position, dest);
		if(master.pathList.Count != 0){
			Debug.Log("Path is found");
			foreach(Vector3 node in master.pathList){
				pathToFollow.Add(node);
			}
		}
	}
	
	IEnumerator FindPath(){
		coroutine = false;
		Debug.Log("Heeyyyyyyyyyyyyy");
		if(!seeDest){
			if(target){
				destination = target.transform.position;
			}
			Debug.Log("Find path called");
			pathToFollow.Clear();
			master.FindPath(transform.position, destination);
			if(master.pathList.Count != 0){
				foreach(Vector3 node in master.pathList){
					pathToFollow.Add(node);
				}
			}
		}
		
		yield return new WaitForSeconds(rayRate);
		coroutine = true;
	}
	
	IEnumerator LookForDestination(){
		coroutine = false;
		
		if(target){
			destination = target.transform.position;
			Collider targetCollider = target.GetComponent<Collider>();
			nearDest = false;
			if(targetCollider){
				float boundBox = Vector3.Distance(targetCollider.bounds.max, targetCollider.bounds.center);
				if(Vector3.Distance(transform.position, destination) <= (controller.radius+boundBox+minDist*2)){
					nearDest = true;
				}
			}
		}
		
		if(pathToFollow.Count != 0 || (target && !nearDest)){
			GameObject temp = new GameObject();
			seeDest = false;
			Debug.Log("Callleeedddddd");
			
			if(pathToFollow.Count > 1){
				temp.transform.position = pathToFollow[pathToFollow.Count-2];
			} else {
				temp.transform.position = transform.position;
			}
			
			temp.transform.LookAt(destination);
			
			float dist = Vector3.Distance(temp.transform.position, destination);
			if((Physics.Raycast(temp.transform.TransformPoint(controller.radius,0,0), temp.transform.forward, dist, master.layerMask)) == false){
				if((Physics.Raycast(temp.transform.TransformPoint(-controller.radius, 0, 0), temp.transform.forward, dist, master.layerMask)) == false){
					seeDest = true;
				}
			}
			
			// Check to see if actor can see shorter path to the next node of pathToFollow list
			if(pathToFollow.Count > 1){
				temp.transform.position = transform.position;
				temp.transform.LookAt(pathToFollow[1]);
				dist = Vector3.Distance(transform.position, pathToFollow[1]);
				if((Physics.Raycast(temp.transform.TransformPoint(controller.radius, 0, 0), temp.transform.forward, dist, master.layerMask)) == false){
					if((Physics.Raycast(temp.transform.TransformPoint(-controller.radius, 0, 0), temp.transform.forward, dist, master.layerMask)) == false){
						pathToFollow.RemoveAt(0);
					}
				}
			}
			
			Destroy(temp);
		}
		
		yield return new WaitForSeconds(rayRate);
		coroutine = true;
	}
	
	IEnumerator CheckLastPosition(){
		coroutine = false;
		
		if((Vector3.Distance(lastPos, transform.position) < 0.6f) && !nearDest && pathToFollow.Count != 0){
			seeDest = false;
			StartCoroutine(FindPath());
			Debug.Log("Check last pos called");
		}
		
		lastPos = transform.position;
		
		yield return new WaitForSeconds(rayRate);
		coroutine = true;
	}
}