using UnityEngine;
using System.Collections;

public class Worm : Enemy {
	
	private float attackRot = -90f;
	private float attackHeight = 5f;
	private float startSpeed;
	public ParticleSystem flamethrower;
	public ParticleEmitter dust;
	private BaseWeapon weapon;
	private float chargeTimer;
	private float chargeTimerMax = 30f;
	private Vector3 playerPos;
	
	public override void Start(){
		base.Start();
		
		startSpeed = speed;
		weapon = flamethrower.GetComponent<BaseWeapon>();
		weapon.damage = damageAmount;
		weapon.bulletsLeft = 100000;
		weapon.bulletsPerClip = 100000;
		chargeTimer = chargeTimerMax;
		flamethrower.Stop();
	}
	
	public override void Update(){
		if(isUnderground){
			return;
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
		
		if(chargeTimer > 0){
			chargeTimer -= Time.deltaTime;
		} else if(chargeTimer <= 0 && state == Enemy.EnemyState.CHASING){
			StartCoroutine(ChargePlayer());
			speed = 4f;
			transform.position = tr.TransformDirection(Vector3.forward*speed);
			transform.rotation = Quaternion.Slerp(tr.rotation, 
				Quaternion.LookRotation(playerPos - tr.position), turningSpeed*Time.deltaTime);
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
			} else if(curDamageMat == lightningMat){
				SendMessage("TakeDamage", 1.0f-Mathf.Clamp01(lightningDamage/Time.time), SendMessageOptions.DontRequireReceiver);
			} else {
				// No extra damage taken
			}
		}
		
		if(chargeTimer > 0){
			if(Vector3.Distance(target.position, transform.position) > distance){
				canMove = true;
				state = Enemy.EnemyState.CHASING;
				rigid.constraints &= ~RigidbodyConstraints.FreezePositionX;
				rigid.constraints &= ~RigidbodyConstraints.FreezePositionZ;
				flamethrower.Stop();
				dust.emit = true;
			} else {
				transform.position = Vector3.Lerp(transform.position, 
					new Vector3(transform.position.x, attackHeight, transform.position.z), 0.5f*Time.deltaTime);
				rigid.constraints = RigidbodyConstraints.FreezePosition;
				
				Vector3 lookPos = target.position;
				lookPos.y = attackHeight;
				Vector3 dir = (lookPos-transform.position).normalized;
				float targetRot = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler(attackRot, targetRot, 0f);
				dust.emit = false;
			}
		}
	}
	
	public override void Attack(GameObject target){
		// Do not enter attack state, i hate hacks
	}
	
	public override void FixedUpdate(){
		if(isUnderground){
			return;
		}
		
		switch(state){
		case EnemyState.CHASING:
			ChaseObject();
			break;
		case EnemyState.HOVER:
			if(anim){
				PlayHoverAnimation();
			}
			break;
		case EnemyState.SHOOTING:
			if(anim){
				PlayShootAnimation();
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
	
	void PlayHoverAnimation(){
		collider.isTrigger = true;
		anim["Attack"].wrapMode = WrapMode.Once;
		anim.CrossFade("Attack", 0.2f);
		StartCoroutine(WaitToEnterShootMode());
	}
	
	void PlayShootAnimation(){
		anim.CrossFade("Shoot", 0.2f);
		flamethrower.Play();
		
		//Vector3 dir = (target.position - transform.position).normalized;
		//float targetRot = Mathf.Atan2(dir.y, dir.x) * 180.0f / Mathf.PI;
		
		//float val = Mathf.Abs(transform.rotation.y-targetRot);
		
		//Debug.Log("Dir: "+val);
		if(currentCoolDown <= 0){
			weapon.Fire();
			currentCoolDown = coolDownLength;
		}
		
		/*if(Vector3.Distance(target.position, transform.position) < distance){
			if(Mathf.Abs(transform.rotation.y - targetRot) < 100f){
				if(currentCoolDown <= 0){
					target.gameObject.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
					currentCoolDown = coolDownLength;
				}
			}
		}*/
		StartCoroutine(WaitToTakeDamage());
	}
	
	void PlayDeathAnimation(){
		anim["Death"].wrapMode = WrapMode.Once;
		anim.CrossFade("Death", 0.2f);
	}
	
	IEnumerator WaitToEnterShootMode(){
		yield return new WaitForSeconds(2f);
		state = Enemy.EnemyState.SHOOTING;
	}
	
	IEnumerator WaitToTakeDamage(){
		yield return new WaitForSeconds(2f);
		collider.isTrigger = false;
	}
	
	IEnumerator ChargePlayer(){
		playerPos = target.position;
		yield return new WaitForSeconds(3f);
		chargeTimer = chargeTimerMax;
	}
	
	public override void ChaseObject(){
		
		Vector3 velocity = new Vector3();
		if(canMove){
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			
			if(Vector3.Distance(target.position, transform.position) < distance && chargeTimer > 0){
				speed = 0f;
				state = Enemy.EnemyState.HOVER;
				canMove = false;
				GameObject hole = (GameObject)Resources.Load("Hole", typeof(GameObject));
				GameObject explosion = (GameObject)Resources.Load("DigExplosion", typeof(GameObject));
		
				Instantiate(explosion, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
				Instantiate(hole, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
			} else if(Vector3.Distance(target.position, transform.position) > distance && chargeTimer > 0){
				Vector3 adjustedTargetHeight = tr.position; // Set position to variable
				adjustedTargetHeight.y = targetHeight; // Adjust height to a set target
				tr.position = adjustedTargetHeight; // Commit the changes
				
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.5f*Time.deltaTime);
				transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetHeight, transform.position.z), 0.5f*Time.deltaTime);
				speed = startSpeed;
				
				if(targetDirection != Vector3.zero){
					RotateTowards(targetDirection);
				}
				
				if(dir.sqrMagnitude > sleepVelocity*sleepVelocity){
					// Move the enemy
					if(rigid != null && !rigid.isKinematic){
						rigid.velocity = dir * speed;
					}
				} else {
					dir = Vector3.zero;
				}
				
				velocity = rigid.velocity;
			}
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
				anim.CrossFade("Walk", 0.2f);
				
				AnimationState state = anim["Walk"];
				
				float relSpeed = relativeVelocity.z;
				state.speed = relSpeed*animationSpeed;
			}
		}
	}
}
