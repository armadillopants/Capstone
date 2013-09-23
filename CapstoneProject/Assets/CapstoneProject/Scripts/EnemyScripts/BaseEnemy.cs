using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class BaseEnemy : AIPath {
	
	public enum EnemyState { CHASINGPLAYER, CHASINGSHIP, ATTACKINGPLAYER, ATTACKINGSHIP, ATTACKINGFORT };
	public EnemyState state = EnemyState.CHASINGPLAYER;
	
	private Transform playerTarget;
	private Transform defendTarget;
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
	
	public new void Start(){
		playerTarget = target;
		defendTarget = GameObject.FindWithTag(Globals.DEFEND).transform;
		currentCoolDown = coolDownLength;
		emitter = GetComponentInChildren<ParticleEmitter>();
		base.Start();
	}
	
	public override Vector3 GetFeetPosition(){
		return tr.position;
	}
	
	protected new void Update(){
		switch(state){
		case EnemyState.CHASINGPLAYER:
			SwitchTarget(Globals.PLAYER);
			ChaseObject();
			break;
		case EnemyState.CHASINGSHIP:
			SwitchTarget(Globals.DEFEND);
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
			SwitchTarget(Globals.PLAYER);
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
		
		if(canAttackBoth){
			if(defendTarget){
				if(Vector3.Distance(playerTarget.position, tr.position) > distance){
					SwitchTarget(Globals.DEFEND);
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
			SendMessage("TakeDamage", burnDamage, SendMessageOptions.DontRequireReceiver);
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
	
	void ChaseObject(){
		/*Vector3	adjustedTargetHeight = target.position; // Sets target's height to a variable
		adjustedTargetHeight.y = trans.position.y; // Target's height is always equal to enemy height
	
		trans.rotation = Quaternion.Slerp(trans.rotation, 
			Quaternion.LookRotation(adjustedTargetHeight - trans.position), turningSpeed);
	
		trans.position += trans.forward * speed * Time.deltaTime;*/
		
		// Get velocity in world-space
		Vector3 velocity;
		if(canMove){
			//Calculate desired velocity
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			
			//Rotate towards targetDirection (filled in by CalculateVelocity)
			if(targetDirection != Vector3.zero){
				RotateTowards(targetDirection);
			}
			
			if(dir.sqrMagnitude > sleepVelocity*sleepVelocity){
				//If the velocity is large enough, move
			} else {
				//Otherwise, just stand still (this ensures gravity is applied)
				dir = Vector3.zero;
			}
			
			if(navController != null){
				navController.SimpleMove(GetFeetPosition(), dir);
			} else if(controller != null){
				controller.SimpleMove(dir);
			} else {
				Debug.LogWarning ("No NavmeshController or CharacterController attached to GameObject");
			}
			
			velocity = controller.velocity;
		} else {
			velocity = Vector3.zero;
		}
	}
	
	void OnParticleCollision(GameObject other){
		BaseWeapon flame = other.transform.parent.GetComponent<BaseWeapon>();
		SendMessage("TakeDamage", flame.damage, SendMessageOptions.DontRequireReceiver);
		
		if(!isBurning){
			isBurning = true;
		} else {
			isBurning = false;
		}
	}
}
