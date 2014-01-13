using System.Collections;
using UnityEngine;

public class RayBlaster : BaseWeapon {
	
	public AudioClip chargeClip;
	private float chargeTime;
	private float maxChargeTime = 5f;
	private float chargeRegenSpeed = 0.5f;
	private bool canFire = true;
	private float initDamage;
	private float extraDamage;
	
	private bool createBullet = false;
	private Rigidbody visibleProj = null;
	private float bulletsToSubtract;
	
	void Start(){
		chargeTime = maxChargeTime;
		initDamage = damage;
	}
	
	void OnEnable(){
		canFire = true;
		hitParticles.emit = false;
		extraDamage = 0;
	}
	
	public override void Update(){
		
		if(!isFiring){
			chargeTime = Mathf.Min(maxChargeTime, chargeTime+chargeRegenSpeed*Time.deltaTime);
			damage = Mathf.Max(initDamage, damage-maxChargeTime*Time.deltaTime);
		}
		
		if(visibleProj && !visibleProj.GetComponent<Projectile>().enabled){
			visibleProj.transform.position = muzzlePos.position;
			visibleProj.transform.rotation = muzzlePos.rotation;
			ParticleEmitter emitter = visibleProj.GetComponentInChildren<ParticleEmitter>();
			emitter.minSize = Mathf.Min(1f, emitter.minSize+0.1f*Time.deltaTime);
			emitter.maxSize = Mathf.Min(1f, emitter.maxSize+0.1f*Time.deltaTime);
			Vector3 boundSize = visibleProj.GetComponent<BoxCollider>().size;
			boundSize.Set(	Mathf.Min(1f, boundSize.x+0.1f*Time.deltaTime),
							Mathf.Min(1f, boundSize.y+0.1f*Time.deltaTime),
							Mathf.Min(1f, boundSize.z+0.1f*Time.deltaTime) );
			visibleProj.GetComponent<BoxCollider>().size = boundSize;
			if(Input.GetButtonUp("Fire1")){
				chargeTime -= chargeRegenSpeed*chargeRegenSpeed;
				damage += Mathf.RoundToInt(extraDamage);
				
				if(Time.time - fireRate > nextFireTime){
					nextFireTime = Time.time - Time.deltaTime;
				}
		
				while(nextFireTime < Time.time && bulletsLeft != 0){
					CreateProjectile();
					nextFireTime += fireRate;
				}
				
				if(chargeTime <= 0){
					canFire = false;
					chargeTime = 0;
					StartCoroutine("CoolDown");
				}
			}
		}
		
		base.Update();
	}
	
	public override void Fire(){
		if(bulletsLeft <= 0 && !isReloading && clips > 0){
			StartCoroutine("Reload");
			return;
		}
		
		if(chargeTime > 0 && canFire && bulletsLeft > 0){
			chargeTime -= 1f*Time.deltaTime;
			bulletsToSubtract = Mathf.Min(5, bulletsToSubtract+1f*Time.deltaTime);
			extraDamage = Mathf.Min(extraDamage+50, extraDamage+5f*Time.deltaTime);
			
			if(!createBullet){
				audio.PlayOneShot(chargeClip);
				// Spawn visual bullet
				Quaternion coneRandomRotation = 
					Quaternion.Euler(Random.Range(-coneAngle, coneAngle), Random.Range(-coneAngle, coneAngle), 0);
				if(projectile){
					visibleProj = (Rigidbody)Instantiate(projectile, muzzlePos.position, muzzlePos.rotation * coneRandomRotation);
				}
				createBullet = true;
			}
			
			if(chargeTime <= 0){
				if(Time.time - fireRate > nextFireTime){
					nextFireTime = Time.time - Time.deltaTime;
				}
		
				while(nextFireTime < Time.time && bulletsLeft != 0){
					CreateProjectile();
					nextFireTime += fireRate;
				}
				damage += Mathf.RoundToInt(extraDamage);
				canFire = false;
				chargeTime = 0;
				StartCoroutine("CoolDown");
			}
		}
	}
	
	public override void CreateProjectile(){
		visibleProj.GetComponent<Projectile>().enabled = true;
		visibleProj.GetComponent<BoxCollider>().enabled = true;
		createBullet = false;
		audio.Stop();
		
		if(bulletsToSubtract > 1){
			bulletsLeft -= Mathf.RoundToInt(bulletsToSubtract);
		} else {
			bulletsLeft--;
		}
		
		bulletsToSubtract = 0;
		
		// Register that we shot this frame,
		// so that the LateUpdate function enabled the muzzleflash renderer for one frame
		lastFrameShot = Time.frameCount;
		enabled = true;
	}
	
	private IEnumerator CoolDown(){
		hitParticles.emit = true;
		yield return new WaitForSeconds(maxChargeTime);
		damage = initDamage;
		extraDamage = 0;
		hitParticles.emit = false;
		canFire = true;
	}
}
