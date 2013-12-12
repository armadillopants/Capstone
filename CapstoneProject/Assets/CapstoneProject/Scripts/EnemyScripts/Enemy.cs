﻿using UnityEngine;
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
	private int amountToGive = 0;
	private float sleepVelocity = 0.4f;

	private bool isTakingExtraDamage = false;
	private ParticleEmitter emitter;
	public float burnDamage;
	public float lightningDamage;
	
	private Health health;
	
	public GameObject money;
	private Camera cam;
	
	private Material curDamageMat;
	public Material fireMat;
	public Material lightningMat;
	
	public int AmountToGive(){
		return amountToGive;
	}
	
	public new void Start(){
		playerTarget = GameController.Instance.GetPlayer();
		shipTarget = GameController.Instance.GetShip();
		
		anim = GetComponent<Animation>();
		
		Spawner spawner = GameObject.Find("WaveController").GetComponentInChildren<Spawner>();
		
		health = GetComponent<Health>();
		
		health.ModifyHealth(spawner.SetEnemyHealth(gameObject.name));
		speed = spawner.SetEnemyMoveSpeed(gameObject.name);
		turningSpeed = spawner.SetEnemyTurnSpeed(gameObject.name);
		coolDownLength = spawner.SetEnemyAttackSpeed(gameObject.name);
		damageAmount = spawner.SetEnemyDamageAmount(gameObject.name);
		
		for(int i=0; i<health.curHealth; i++){
			if(health.curHealth % i == 0){
				amountToGive += 10;
			}
		}
		
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
		
		cam = Camera.main;
		currentCoolDown = coolDownLength;
		emitter = GetComponentInChildren<ParticleEmitter>();
		base.Start();
	}
	
	public Animation GetAnim(){
		return anim;
	}
	
	public override Vector3 GetFeetPosition(){
		return rigid.position;
	}
	
	protected new void Update(){
		
		RenderEnemy();
		
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
			
			if(GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION) != null){
				GameObject[] nearestFort = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
				for(int i=0; i<nearestFort.Length; i++){
					if(Vector3.Distance(nearestFort[i].transform.position, tr.position) < 10f && 
						Vector3.Distance(lastTarget.position, tr.position) > 10f){
						target = GameController.Instance.FindNearestTarget(Globals.FORTIFICATION, tr).transform;
					} else {
						SwitchTarget(lastTarget.tag);
					}
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
		
		if(!isTakingExtraDamage){
			emitter.emit = false;
		} else {
			emitter.emit = true;
			if(curDamageMat == fireMat){
				SendMessage("TakeDamage", 1.0f-Mathf.Clamp01(burnDamage/Time.time), SendMessageOptions.DontRequireReceiver);
			} else {
				SendMessage("TakeDamage", 1.0f-Mathf.Clamp01(lightningDamage/Time.time), SendMessageOptions.DontRequireReceiver);
			}
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
			if(anim){
				PlayIdleAnimation();
			}
			break;
		case EnemyState.DEAD:
			if(!isDead && anim){
				PlayDeathAnimation();
				for(int i=0; i<5; i++){
					Vector3 pos = tr.position + new Vector3(Mathf.Cos(Random.Range(0,360)), 
												1, 
												Mathf.Sin(Random.Range(0,360)))*(Random.Range(3,3));
					Instantiate(money, pos, Quaternion.identity);
				}
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
		
		if(anim && canMove){
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
	
	void PlayIdleAnimation(){
		anim.CrossFade("Idle", 0.2f);
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
		BaseWeapon incomingParticle = other.transform.parent.GetComponent<BaseWeapon>();
		SendMessage("TakeDamage", 1.0f-Mathf.Clamp01(incomingParticle.damage/Time.time), SendMessageOptions.DontRequireReceiver);
		
		ParticleRenderer curMat = incomingParticle.GetComponentInChildren<ParticleRenderer>();
		if(curMat.material == fireMat){
			curDamageMat = fireMat;
		} else if(curMat.material == lightningMat){
			curDamageMat = lightningMat;
		} else {
			Debug.Log("WRONG MATERIAL");
		}
		
		if(!isTakingExtraDamage){
			isTakingExtraDamage = true;
		} else {
			isTakingExtraDamage = false;
		}
	}
	
	void RenderEnemy(){
		Renderer data = transform.GetComponentInChildren<Renderer>().renderer;
		if(!data.IsVisibleFrom(cam)){
			data.enabled = false;
		}
		if(data.IsVisibleFrom(cam)){
			data.enabled = true;
		}
	}
}