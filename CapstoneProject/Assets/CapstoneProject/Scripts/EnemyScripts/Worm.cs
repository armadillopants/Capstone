﻿using UnityEngine;
using System.Collections;

public class Worm : Enemy {
	
	private float attackRot = -90f;
	private float attackHeight = 5f;
	public ParticleSystem flamethrower;
	public ParticleEmitter dust;
	private float popUpTimer = Random.Range(5f, 20f);
	private float shootTimer = Random.Range(5f, 10f);
	public AudioClip acidSpit;
	public AudioClip groundCrawl;
	private bool popUp = false;
	
	public override void Update(){
		if(popUpTimer > 0){
			popUpTimer -= Time.deltaTime;
		} else {
			popUpTimer = 0;
		}
		
		if(shootTimer > 0){
			shootTimer -= Time.deltaTime;
		} else {
			shootTimer = 0;
		}
		
		if(curTarget != null){
			lastTarget = curTarget;
		} else {
			SwitchTarget(lastTarget.tag);
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
		
		ClampCoolDownTime();
		
		if(GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION) != null){
			GameObject[] nearestFort = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
			for(int i=0; i<nearestFort.Length; i++){
				if(Vector3.Distance(nearestFort[i].transform.position, tr.position) <= 10f && 
					Vector3.Distance(lastTarget.position, tr.position) >= 3f){
					target = GameController.Instance.FindNearestTarget(Globals.FORTIFICATION, tr).transform;
				} else {
					SwitchTarget(lastTarget.tag);
				}
			}
		}
		
		if(shipTarget){
			if(Vector3.Distance(playerTarget.position, tr.position) > distance){
				SwitchTarget(Globals.SHIP);
				lastTarget = playerTarget;
			} else if(Vector3.Distance(playerTarget.position, tr.position) <= distance){
				SwitchTarget(Globals.PLAYER);
				lastTarget = shipTarget;
			}
		}
		
		if(health.IsDead){
			StopAllCoroutines();
			state = EnemyState.DEAD;
			rigid.constraints = RigidbodyConstraints.FreezeAll;
			rigid.isKinematic = true;
			flamethrower.Stop();
			audio.clip = groundCrawl;
			audio.loop = true;
			audio.Stop();
		} else {
			if(popUp){
				tr.position = Vector3.Lerp(tr.position, 
					new Vector3(tr.position.x, attackHeight, tr.position.z), 0.5f*Time.deltaTime);
				rigid.constraints = RigidbodyConstraints.FreezePosition;
				
				Vector3 lookPos = target.position;
				lookPos.y = attackHeight;
				Vector3 dir = (lookPos-tr.position).normalized;
				float targetRot = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
				tr.rotation = Quaternion.Euler(attackRot, targetRot, 0f);
				dust.emit = false;
			} else {
				rigid.constraints &= ~RigidbodyConstraints.FreezePositionX;
				rigid.constraints &= ~RigidbodyConstraints.FreezePositionZ;
				flamethrower.Stop();
				dust.emit = true;
				canMove = true;
				state = Enemy.EnemyState.CHASING;
			}
		}
		
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
	}
	
	public override void FixedUpdate(){
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
					ObjectPool.Spawn(money, pos, Quaternion.identity);
				}
				popUp = false;
				popUpTimer = Random.Range(5f, 20f);
				shootTimer = Random.Range(5f, 10f);
				isDead = true;
			}
			break;
		}
	}
	
	void PlayHoverAnimation(){
		anim["Attack"].wrapMode = WrapMode.Once;
		anim.CrossFade("Attack", 0.2f);
		StartCoroutine(WaitToEnterShootMode());
	}
	
	void PlayShootAnimation(){
		StartCoroutine(WaitToTakeDamage());
		anim.CrossFade("Shoot", 0.2f);
		flamethrower.Play();
		
		if(shootTimer <= 0){
			popUp = false;
			popUpTimer = Random.Range(5f, 20f);
			audio.clip = groundCrawl;
			audio.loop = true;
			audio.Play();
		}
		
		if(target != null){
			Vector3 dir = (target.position - tr.position).normalized;
			
			if(Vector3.Distance(target.position, tr.position) <= 10f){
				if(Vector3.Dot(dir, -tr.up) > 0){
					if(currentCoolDown <= 0){
						target.gameObject.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
						currentCoolDown = coolDownLength;
					}
				}
			}
		} else {
			target = lastTarget;
		}
	}
	
	void PlayDeathAnimation(){
		anim["Death"].wrapMode = WrapMode.Once;
		anim.CrossFade("Death", 0.2f);
	}
	
	IEnumerator WaitToEnterShootMode(){
		yield return new WaitForSeconds(2f);
		shootTimer = Random.Range(5f, 10f);
		audio.clip = acidSpit;
		audio.loop = true;
		audio.Play();
		state = Enemy.EnemyState.SHOOTING;
	}
	
	IEnumerator WaitToTakeDamage(){
		yield return new WaitForSeconds(1f);
		collider.isTrigger = false;
	}
	
	public override void ChaseObject(){
		Vector3 velocity = new Vector3();
		if(canMove){
			collider.isTrigger = true;
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			
			if(popUpTimer <= 0 && Vector3.Distance(target.position, tr.position) < distance){
					popUp = true;
					state = Enemy.EnemyState.HOVER;
					canMove = false;
					audio.Stop();
					ObjectPool.Spawn(Spawner.spawner.explosion, new Vector3(tr.position.x, 0.1f, tr.position.z), Quaternion.identity);
					ObjectPool.Spawn(Spawner.spawner.hole, new Vector3(tr.position.x, 0.1f, tr.position.z), Quaternion.identity);
			} else {
				Vector3 adjustedTargetHeight = tr.position; // Set position to variable
				adjustedTargetHeight.y = targetHeight; // Adjust height to a set target
				tr.position = adjustedTargetHeight; // Commit the changes
				
				tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.identity, 0.5f*Time.deltaTime);
				tr.position = Vector3.Lerp(tr.position, new Vector3(tr.position.x, targetHeight, tr.position.z), 0.5f*Time.deltaTime);
				
				if(tr.position.y < targetHeight){
					tr.position = new Vector3(tr.position.x, targetHeight, tr.position.z);
				}
				
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
