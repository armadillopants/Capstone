using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class BaseEnemy : AIPath {
	
	public enum EnemyState { CHASINGPLAYER, CHASINGSHIP, CHASINGFORT, ATTACKINGPLAYER, ATTACKINGSHIP, ATTACKINGFORT };
	public EnemyState state = EnemyState.CHASINGPLAYER;
	
	private Transform playerTarget;
	private Transform shipTarget;
	private Transform lastTarget;
	public bool doDamage = false;
	public float currentCoolDown = 0f;
	public float coolDownLength = 1f;
	public float damageAmount = 10f;
	public float distance = 10f;
	public bool canAttackBoth = false;
	private float sleepVelocity = 0.4f;
	private bool isBurning = false;
	private ParticleEmitter emitter;
	public float burnDamage;
	public bool isPathBlocked = false;
	
	public new void Start(){
		playerTarget = GameController.Instance.GetPlayer();
		shipTarget = GameController.Instance.GetShip();
		lastTarget = playerTarget;
		currentCoolDown = coolDownLength;
		emitter = GetComponentInChildren<ParticleEmitter>();
		base.Start();
	}
	
	public override Vector3 GetFeetPosition(){
		return rigid.position;
	}
	
	protected new void Update(){
		if(target != null){
			isPathBlocked = CheckIfPathIsPossible(tr.position, lastTarget.position);
		}
		Debug.Log("Path Blocked: " + isPathBlocked);
		
		if(isPathBlocked){
			state = EnemyState.CHASINGFORT;
			target = FindNearestTarget().transform;
			
			if(target == null){
				target = lastTarget;
				state = EnemyState.CHASINGPLAYER;
			}
		} else {
			if(target == null){
				target = lastTarget;
				state = EnemyState.CHASINGPLAYER;
			} else {
				//target = lastTarget;
				//state = EnemyState.CHASINGPLAYER;
			}
		}
		
		switch(state){
		case EnemyState.CHASINGPLAYER:
			SwitchTarget(Globals.PLAYER);
			lastTarget = playerTarget;
			//ChaseObject();
			break;
		case EnemyState.CHASINGSHIP:
			SwitchTarget(Globals.SHIP);
			lastTarget = shipTarget;
			//ChaseObject();
			break;
		case EnemyState.CHASINGFORT:
			ChaseObject();
			break;
		case EnemyState.ATTACKINGPLAYER:
			break;
		case EnemyState.ATTACKINGFORT:
			break;
		case EnemyState.ATTACKINGSHIP:
			break;
		}
		
		if(shipTarget == null){
			SwitchTarget(Globals.PLAYER);
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
		
		if(canAttackBoth){
			if(shipTarget){
				if(Vector3.Distance(playerTarget.position, tr.position) > distance){
					SwitchTarget(Globals.SHIP);
				} else if(Vector3.Distance(playerTarget.position, tr.position) <= distance){
					SwitchTarget(Globals.PLAYER);
				}
			}
		}
		
		ClampCoolDownTime();
		
		if(!isBurning){
			emitter.emit = false;
		} else {
			emitter.emit = true;
			SendMessage("TakeDamage", 1.0f-Mathf.Clamp01(burnDamage/Time.time), SendMessageOptions.DontRequireReceiver);
		}
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
		rigid.velocity = new Vector3(0,0,0);
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
	
	// Checks if path is blocked
	public bool CheckIfPathIsPossible(Vector3 pathStart, Vector3 pathEnd){
		Node node1 = AstarPath.active.GetNearest(pathStart, NNConstraint.Default).node;
		Node node2 = AstarPath.active.GetNearest(pathEnd, NNConstraint.Default).node;
		
		// If path isnt traversible return true, else return false
		if(!PathUtilities.IsPathPossible(node1, node2)){
			return true;
		} else {
			return false;
		}
	}
	
	public override void OnTargetReached(){
		// TODO: Enemy attacks player
	}
	
	void FixedUpdate(){
		ChaseObject();
	}
	
	void ChaseObject(){
		/*Vector3	adjustedTargetHeight = target.position; // Sets target's height to a variable
		adjustedTargetHeight.y = tr.position.y; // Target's height is always equal to enemy height
	
		tr.rotation = Quaternion.Slerp(tr.rotation, 
			Quaternion.LookRotation(adjustedTargetHeight - tr.position), turningSpeed);
	
		tr.position += tr.forward * speed * Time.deltaTime;*/
		
		if(canMove){
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			
			if(targetDirection != Vector3.zero){
				RotateTowards(targetDirection);
			}
			
			if(rigid != null){
				rigid.velocity = dir * speed;
			}
		}
		
		/*if(canMove){
			// Calculate desired velocity
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			Vector3 targetVel = dir;// * speed;
			targetVel = tr.TransformDirection(targetVel);
			targetVel *= speed;
			Vector3 deltaVel = targetVel - rigid.velocity;

			if(rigid.useGravity){
				deltaVel.x = Mathf.Clamp(deltaVel.x, -5f, 5f);
				deltaVel.z = Mathf.Clamp(deltaVel.z, -5f, 5f);
				deltaVel.y = 0;
			}
			
			if(dir.sqrMagnitude > sleepVelocity*sleepVelocity){
				// If the velocity is large enough, move
				rigid.AddForce(deltaVel, ForceMode.VelocityChange);
			} else {
				// Otherwise, just stand still (this ensures gravity is applied)
				dir = Vector3.zero;
			}
			
			//Rotate towards targetDirection (filled in by CalculateVelocity)
			if(targetDirection != Vector3.zero){
				RotateTowards(targetDirection);
			} else {
				rigid.angularVelocity = Vector3.zero;
			}
		}*/
	}
	
	void OnParticleCollision(GameObject other){
		BaseWeapon incomingParticle = other.transform.parent.GetComponent<BaseWeapon>();
		SendMessage("TakeDamage", 1.0f-Mathf.Clamp01(incomingParticle.damage/Time.time), SendMessageOptions.DontRequireReceiver);
		
		if(!isBurning){
			isBurning = true;
		} else {
			isBurning = false;
		}
	}
	
	GameObject FindNearestTarget(){
		GameObject[] targets;
		targets = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		
		foreach(GameObject targetCheck in targets){
			Vector3 diff = targetCheck.transform.position - tr.position;
			float curDist = diff.sqrMagnitude;
			if(curDist < distance){
				closest = targetCheck;
				distance = curDist;
			}
		}
		return closest;
	}
}
