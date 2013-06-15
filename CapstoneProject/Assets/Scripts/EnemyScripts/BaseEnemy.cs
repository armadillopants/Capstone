using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {
	
	public Transform target;
	private Transform playerTarget;
	private Transform defendTarget;
	public float moveSpeed = 0f;
	public bool doDamage = false;
	public float currentCoolDown = 0f;
	public float coolDownLength = 1f;
	public float damageAmount = 10f;
	public float distance = 10f;
	public float turnSpeed = 1f;
	private Transform trans;

	void Awake(){
		target = GameObject.FindWithTag("Player").transform;
		playerTarget = target;
		defendTarget = GameObject.FindWithTag("Defend").transform;
	}
	
	void Start(){
		trans = transform;
		currentCoolDown = coolDownLength;
	}
	
	void Update(){
		if(defendTarget == null){
			SwitchTarget("Player");
		}
		
		if(playerTarget == null){
			Destroy(gameObject);
			return;
		}
		
		Vector3	adjustedTargetHeight = target.position; // Sets target's height to a variable
		adjustedTargetHeight.y = trans.position.y; // Target's height is always equal to enemy height
		
		trans.rotation = Quaternion.Slerp(trans.rotation, 
			Quaternion.LookRotation(adjustedTargetHeight - trans.position), turnSpeed);
		
		trans.position += trans.forward * moveSpeed * Time.deltaTime;
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
		
		if(defendTarget){
			if(Vector3.Distance(playerTarget.position, trans.position) > distance){
				SwitchTarget("Defend");
			} else if(Vector3.Distance(playerTarget.position, trans.position) <= distance){
				SwitchTarget("Player");
			}
		}
		
		ClampCoolDownTime();
	}
	
	void ClampCoolDownTime(){
		currentCoolDown = Mathf.Clamp(currentCoolDown, 0, coolDownLength);
	}
	
	void SwitchTarget(string targetName){
		target = GameObject.FindWithTag(targetName).transform;
	}
	
	void OnCollisionStay(Collision collision){
		// If we collide with the target
		if(target != null){
			if(collision.gameObject.tag == target.tag){
				// Start dealing damage
				doDamage = true;
				Attack(collision.gameObject);
			}
		}
	}
	
	void OnCollisionExit(){
		rigidbody.velocity = new Vector3(0,0,0);
		doDamage = false;
	}
	
	void Attack(GameObject target){
		if(doDamage){
			if(currentCoolDown <= 0){
				target.gameObject.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
				currentCoolDown = coolDownLength;
			}
		}
	}
}
