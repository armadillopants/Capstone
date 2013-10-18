using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class Enemy : AIPath {
	
	public enum EnemyState { CHASING, ATTACKING, IDLE, DEAD };
	public EnemyState state = EnemyState.CHASING;
	
	private Animation anim;
	public float animationSpeed = 0.2f;
	
	private Transform playerTarget;
	private Transform shipTarget;
	public Transform lastTarget;
	public Transform curTarget;
	
	public bool doDamage = false;
	public bool chasePlayer = true;
	public bool canAttackBoth = false;
	public bool isPathBlocked = false;
	private bool isDead = false;
	
	private float currentCoolDown = 0f;
	public float coolDownLength = 1f;
	public float damageAmount = 10f;
	public float distance = 10f;
	public float targetHeight = 1f;
	public int amountToGive = 100;
	private float sleepVelocity = 0.4f;

	private bool isBurning = false;
	private ParticleEmitter emitter;
	public float burnDamage;
	
	private Health health;
	
	public new void Start(){
		playerTarget = GameController.Instance.GetPlayer();
		shipTarget = GameController.Instance.GetShip();
		
		anim = GetComponent<Animation>();
		
		health = GetComponent<Health>();
		
		if(chasePlayer){
			SwitchTarget(Globals.PLAYER);
		} else {
			SwitchTarget(Globals.SHIP);
		}
		
		lastTarget = target;
		
		if(anim){
			// Set all animations to loop for now
			anim.wrapMode = WrapMode.Loop;
			
			// Play walk animation
			anim.Play("Walk");
		}
		
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
			target = GameController.Instance.FindNearestTarget(Globals.FORTIFICATION, tr).transform;
			
			if(target == null){
				target = lastTarget;
				if(!doDamage){
					state = EnemyState.CHASING;
				}
			}
		} else {
			if(target == null){
				target = lastTarget;
				if(!doDamage){
					state = EnemyState.CHASING;
				}
			} else {
				target = lastTarget;
				if(!doDamage){
					state = EnemyState.CHASING;
				}
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
		
		if(GameController.Instance.GetShipHealth().IsDead){
			SwitchTarget(Globals.PLAYER);
			canAttackBoth = false;
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
		
		if(health.IsDead){
			state = EnemyState.DEAD;
			rigid.isKinematic = true;
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
		if(!rigid.isKinematic){
			rigid.velocity = new Vector3(0,0,0);
		}
		doDamage = false;
	}
	
	public virtual void Attack(GameObject target){
		if(doDamage){
			state = EnemyState.ATTACKING;
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
		Debug.Log("Reached Target");
		// TODO: Enemy attacks player
	}
	
	void FixedUpdate(){
		switch(state){
		case EnemyState.CHASING:
			ChaseObject();
			break;
		case EnemyState.ATTACKING:
			if(anim){
				PlayAttackAnimation();
			}
			break;
		case EnemyState.IDLE:
			break;
		case EnemyState.DEAD:
			if(!isDead && anim){
				PlayDeathAnimation();
				isDead = true;
			}
			break;
		}
	}
	
	void ChaseObject(){
		
		Vector3 velocity;
		if(canMove){
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			
			Vector3 adjustedTargetHeight = tr.position; // Set position to variable
			adjustedTargetHeight.y = targetHeight; // Adjust height to a set target
			tr.position = adjustedTargetHeight; // Commit the changes
			
			if(targetDirection != Vector3.zero){
				RotateTowards(targetDirection);
			}
			
			if(dir.sqrMagnitude > sleepVelocity*sleepVelocity){
				// Move the enemey
				if(rigid != null){
					rigid.velocity = dir * speed;
				}
			} else {
				dir = Vector3.zero;
			}
			
			velocity = rigid.velocity;
		} else {
			velocity = Vector3.zero;
		}
		
		if(anim){
			Vector3 relativeVelocity = tr.InverseTransformDirection(velocity);
			if(velocity.sqrMagnitude <= sleepVelocity*sleepVelocity){
				// Fade out walk animation
				anim.Blend("Walk", 0, 0.2f);
			} else {
				// Fade in walking animation
				anim.Blend("Attack", 0, 0.2f);
				anim.Blend("Walk", 1, 0.2f);
				
				AnimationState state = anim["Walk"];
				
				float relSpeed = relativeVelocity.z;
				state.speed = relSpeed*animationSpeed;
			}
		}
	}

	void PlayAttackAnimation(){
		anim.CrossFade("Attack", 0.2f);
		Vector3 dir = CalculateVelocity(GetFeetPosition());
		rigid.velocity = dir * (speed * 0.5f);
		
		if(targetDirection != Vector3.zero){
			RotateTowards(targetDirection);
		}
	}

	void PlayDeathAnimation(){
		anim["Death"].wrapMode = WrapMode.Once;
		anim.CrossFade("Death", 0.2f);
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
}