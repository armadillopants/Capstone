﻿using System.Collections;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {
	
	private BaseWeapon[] guns;
	private Animation anim;
	private Enemy cyborg;
	private Health health;
	public GameObject gunObject;
	public float distance = 15f;
	
	private float coolDownTimer;
	private float coolDownLength = 5f;
	
	private float tempSpeed = 0f;
	private bool dropGun = false;
	public AudioClip loseWeapon;
	public AudioClip targetAquired;

	void Start(){
		guns = transform.GetComponentsInChildren<BaseWeapon>();
		health = GetComponent<Health>();
		cyborg = GetComponent<Enemy>();
		tempSpeed = cyborg.speed;
		anim = cyborg.GetAnim();
	}
	
	void Update(){
		if(gunObject){
			GameObject target = GameController.Instance.FindNearestTarget(Globals.PLAYER, this.transform);
			
			if(target){
				if(Vector3.Distance(target.transform.position, transform.position) < distance){
					if(coolDownTimer <= 0){
						coolDownTimer = 0;
						cyborg.speed = 0;
						audio.PlayOneShot(targetAquired);
						anim.CrossFade("Shoot", 0.2f);
						guns[0].Fire();
						guns[1].Fire();
						StartCoroutine(Firing());
					}
				}
			}
			
			coolDownTimer -= Time.deltaTime;
			
			if(health.curHealth < health.GetMaxHealth()/Random.Range(2,4) && !dropGun){
				StopCoroutine("Firing");
				audio.PlayOneShot(loseWeapon);
				gunObject.AddComponent<Rigidbody>().AddForce(new Vector3(transform.position.x+2,0,transform.position.z+2));
				gunObject.AddComponent<DestroyTimer>();
				gunObject.transform.parent = null;
				gunObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				gunObject = null;
				anim.CrossFade("Walk", 0.2f);
				cyborg.speed = tempSpeed+0.5f;
				dropGun = true;
			}
		}
	}
	
	IEnumerator Firing(){
		yield return new WaitForSeconds(coolDownLength);
		coolDownTimer = Random.Range(coolDownLength, coolDownLength*2);
		cyborg.speed = tempSpeed;
		anim.CrossFade("Walk", 0.2f);
	}
}
