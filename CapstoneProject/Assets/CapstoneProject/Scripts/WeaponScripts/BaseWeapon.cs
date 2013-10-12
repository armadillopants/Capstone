using UnityEngine;
using System.Collections;

public class BaseWeapon : MonoBehaviour {
	
	public enum WeaponState { RAYCAST, PROJECTILE };
	public WeaponState state;
	
	public float fireRate = 0.09f;
	public float range = 100f;
	public float force = 1.0f;
	public int bulletsLeft = 0;
	public int bulletsPerClip = 40;
	public int clips = 6;
	public int maxClips = 0;
	public float reloadSpeed = 1.2f;
	public float damage = 10.0f;
	public float coneAngle = 1.5f;
	public float roundsPerBurst = 0f;
	public float lagBetweenShots = 0f;
	public int id;
	public Rigidbody projectile;
	public Transform muzzlePos;
	
	private Renderer muzzle;
	private Light gunFlash;
	public Renderer muzzleFlash;
	public Light lightFlash;
	
	public bool isReloading = false;
	protected float nextFireTime = 0.0f;
	protected float lastFrameShot = -1;
	public ParticleEmitter hitParticles;
	
	void Start(){
		hitParticles = GetComponentInChildren<ParticleEmitter>();
		maxClips = clips;
		
		if(hitParticles){
			hitParticles.emit = false;
			bulletsLeft = bulletsPerClip;
		} else {
			bulletsLeft = bulletsPerClip;
		}
		
		if(muzzleFlash){
			muzzle = (Renderer)Instantiate(muzzleFlash, muzzlePos.position, Quaternion.identity);
			muzzle.transform.parent = muzzlePos.transform;
		}
		if(lightFlash){
			gunFlash = (Light)Instantiate(lightFlash, muzzlePos.position, Quaternion.identity);
			gunFlash.transform.parent = muzzlePos.transform;
		}
	}
	
	public virtual void Update(){
		if(Input.GetButton("Fire1") && GameController.Instance.canShoot){
			Fire();
		}
	}
	
	void LateUpdate(){
		if(muzzleFlash && lightFlash){
			// We shot this frame, enable the muzzle flash
			if(lastFrameShot == Time.frameCount){
				muzzle.transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
				muzzle.enabled = true;
				gunFlash.enabled = true;
				
				if(audio){
					if(!audio.isPlaying){
						audio.Play();
						audio.loop = true;
					}
				}
			} else {
				// We didn't disable the muzzle flash
				muzzle.enabled = false;
				gunFlash.enabled = false;
				
				// Play sound
				if(audio){
					audio.loop = false;
				}
			}
		}
	}

	public virtual void Fire(){
		if(bulletsLeft <= 0 && !isReloading){
			StartCoroutine("Reload");
			return;
		}
		
		if(Time.time - fireRate > nextFireTime){
			nextFireTime = Time.time - Time.deltaTime;
		}
		
		while(nextFireTime < Time.time && bulletsLeft != 0){
			CreateProjectile();
			nextFireTime += fireRate;
		}
	}

	public void CreateProjectile(){
		// Spawn visual bullet
		Quaternion coneRandomRotation = 
			Quaternion.Euler(Random.Range(-coneAngle, coneAngle), Random.Range(-coneAngle, coneAngle), 0);
		Rigidbody visibleProj = null;
		if(projectile){
			visibleProj = (Rigidbody)Instantiate(projectile, muzzlePos.position, muzzlePos.rotation * coneRandomRotation);
		}
		
		switch(state){
		case WeaponState.RAYCAST:
			Bullet bullet = null;
			if(visibleProj){
				bullet = visibleProj.GetComponent<Bullet>();
			}
		    Vector3 direction = transform.TransformDirection(Vector3.forward);
		  	RaycastHit hit;
			
			LayerMask layerMaskPlayer = 8;
			LayerMask layerMaskFort = 9;
			LayerMask layerMaskFinal = ~((1<<layerMaskPlayer)|1<<layerMaskFort);
			
		  	// Does the ray intersect any objects excluding the player and fort layer
		  	if(Physics.Raycast(transform.position, direction, out hit, range, layerMaskFinal)){
				// Apply a force to the rigidbody we hit
				if(hit.rigidbody){
					hit.rigidbody.AddForceAtPosition(force * direction, hit.point, ForceMode.Impulse);
				}
				if(bullet){
					bullet.distance = hit.distance;
				}
				// Place the particle system for spawing out of place where we hit the surface!
				// And spawn a couple of particles
				/*if(hitParticles){
					hitParticles.transform.position = hit.point;
					hitParticles.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					hitParticles.Emit();
				}*/
				Debug.DrawRay(transform.position, direction * hit.distance, Color.blue);
				// Send a damage message to the hit object
				hit.collider.gameObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
		  	} else {
				if(bullet){
					bullet.distance = range;
				}
		    	Debug.DrawRay(transform.position, direction * range, Color.green);
		  	}
			
			bulletsLeft--;
			
			// Register that we shot this frame,
			// so that the LateUpdate function enabled the muzzleflash renderer for one frame
			lastFrameShot = Time.frameCount;
			enabled = true;
			break;
		case WeaponState.PROJECTILE:
			bulletsLeft--;
			
			// Register that we shot this frame,
			// so that the LateUpdate function enabled the muzzleflash renderer for one frame
			lastFrameShot = Time.frameCount;
			enabled = true;
			break;
		}
	}
	
	private IEnumerator Reload(){
		// Wait for reload time first and then add more bullets!
		isReloading = true;
		yield return new WaitForSeconds(reloadSpeed);
		
		// We have a clip left to reload
		if(clips > 0){
			clips--;
			bulletsLeft = bulletsPerClip;
		}
		isReloading = false;
	}
}
