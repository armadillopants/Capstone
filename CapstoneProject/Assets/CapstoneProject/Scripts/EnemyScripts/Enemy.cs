using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class Enemy : AIPath {
	
	public enum EnemyState { CHASING, ATTACKING, IDLE, DEAD };
	public EnemyState state = EnemyState.CHASING;
	
	private Transform playerTarget;
	private Transform shipTarget;
	public Transform lastTarget;
	public Transform curTarget;
	
	public bool doDamage = false;
	public bool chasePlayer = true;
	public bool canAttackBoth = false;
	public bool isPathBlocked = false;
	
	private float currentCoolDown = 0f;
	public float coolDownLength = 1f;
	public float damageAmount = 10f;
	public float distance = 10f;
	public float targetHeight = 1f;
	public int amountToGive = 100;

	private bool isBurning = false;
	private ParticleEmitter emitter;
	public float burnDamage;
	
	public new void Start(){
		playerTarget = GameController.Instance.GetPlayer();
		shipTarget = GameController.Instance.GetShip();
		
		if(chasePlayer){
			SwitchTarget(Globals.PLAYER);
		} else {
			SwitchTarget(Globals.SHIP);
		}
		
		lastTarget = target;
		
		currentCoolDown = coolDownLength;
		emitter = GetComponentInChildren<ParticleEmitter>();
		base.Start();
	}
	
	public override Vector3 GetFeetPosition(){
		return rigid.position;
	}
	
	protected new void Update(){
		if(curTarget != null){
			lastTarget = curTarget;
			isPathBlocked = CheckIfPathIsPossible(tr.position, lastTarget.position);
		}
		
		if(isPathBlocked){
			target = FindNearestTarget().transform;
			
			if(target == null){
				target = lastTarget;
				state = EnemyState.CHASING;
			}
		} else {
			if(target == null){
				target = lastTarget;
				state = EnemyState.CHASING;
			} else {
				target = lastTarget;
				state = EnemyState.CHASING;
			}
			
			if(canAttackBoth){
				if(shipTarget){
					if(Vector3.Distance(playerTarget.position, tr.position) > distance){
						SwitchTarget(Globals.SHIP);
						lastTarget = playerTarget;
					} else if(Vector3.Distance(playerTarget.position, tr.position) <= distance){
						SwitchTarget(Globals.PLAYER);
						lastTarget = shipTarget;
					}
				}
			}
		}
		
		if(shipTarget == null){
			SwitchTarget(Globals.PLAYER);
			canAttackBoth = false;
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
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
		curTarget = target; // Store current target
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
	
	/** Checks if path is blocked
	*	If path isnt traversible return true, else
	*	return false
	**/
	public bool CheckIfPathIsPossible(Vector3 pathStart, Vector3 pathEnd){
		Node node1 = AstarPath.active.GetNearest(pathStart, NNConstraint.Default).node;
		Node node2 = AstarPath.active.GetNearest(pathEnd, NNConstraint.Default).node;
		
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
		switch(state){
		case EnemyState.CHASING:
			ChaseObject();
			break;
		case EnemyState.ATTACKING:
			break;
		case EnemyState.IDLE:
			break;
		case EnemyState.DEAD:
			break;
		}
	}
	
	void ChaseObject(){
		
		if(canMove){
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			
			Vector3 adjustedTargetHeight = tr.position; // Set position to variable
			adjustedTargetHeight.y = targetHeight; // Adjust height to a set target
			tr.position = adjustedTargetHeight; // Commit the changes
			
			if(targetDirection != Vector3.zero){
				RotateTowards(targetDirection);
			}
			
			if(rigid != null){
				rigid.velocity = dir * speed;
			}
		}
	}
	
	void OnParticleCollision(GameObject other){
		BaseWeapon flame = other.transform.parent.GetComponent<BaseWeapon>();
		SendMessage("TakeDamage", 1.0f-Mathf.Clamp01(flame.damage/Time.time), SendMessageOptions.DontRequireReceiver);
		
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