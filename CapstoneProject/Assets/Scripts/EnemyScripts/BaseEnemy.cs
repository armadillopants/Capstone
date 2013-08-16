using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {
	
	public enum EnemyState { CHASINGPLAYER, CHASINGSHIP, ATTACKINGPLAYER, ATTACKINGSHIP, ATTACKINGFORT };
	public EnemyState state = EnemyState.CHASINGPLAYER;
	
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
	public bool canAttackBoth = false;
	protected Transform trans;
	//private NavMeshAgent agent;

	public virtual void Awake(){
		target = GameObject.FindWithTag("Player").transform;
		playerTarget = target;
		defendTarget = GameObject.FindWithTag("Defend").transform;
		//agent = GetComponent<NavMeshAgent>();
	}
	
	public virtual void Start(){
		trans = transform;
		currentCoolDown = coolDownLength;
	}
	
	public virtual void Update(){
		switch(state){
		case EnemyState.CHASINGPLAYER:
			SwitchTarget("Player");
			//agent.destination = playerTarget.position;
			ChaseObject();
			break;
		case EnemyState.CHASINGSHIP:
			SwitchTarget("Defend");
			ChaseObject();
			break;
		case EnemyState.ATTACKINGPLAYER:
			break;
		case EnemyState.ATTACKINGFORT:
			break;
		case EnemyState.ATTACKINGSHIP:
			break;
		}
		
		if(defendTarget == null){
			SwitchTarget("Player");
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
		
		if(canAttackBoth){
			if(defendTarget){
				if(Vector3.Distance(playerTarget.position, trans.position) > distance){
					SwitchTarget("Defend");
				} else if(Vector3.Distance(playerTarget.position, trans.position) <= distance){
					SwitchTarget("Player");
				}
			}
		}
		
		ClampCoolDownTime();
	}
	
	public void ClampCoolDownTime(){
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
	
	public virtual void Attack(GameObject target){
		if(doDamage){
			if(currentCoolDown <= 0){
				target.gameObject.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
				currentCoolDown = coolDownLength;
			}
		}
	}
	
	void ChaseObject(){
		Vector3	adjustedTargetHeight = target.position; // Sets target's height to a variable
		adjustedTargetHeight.y = trans.position.y; // Target's height is always equal to enemy height
	
		trans.rotation = Quaternion.Slerp(trans.rotation, 
			Quaternion.LookRotation(adjustedTargetHeight - trans.position), turnSpeed);
	
		trans.position += trans.forward * moveSpeed * Time.deltaTime;
	}
}
