using UnityEngine;
using System.Collections;

public class BaseWeapon : MonoBehaviour {
	
	public enum WeaponState { RAYCAST, PROJECTILE };
	public WeaponState state;
	
	public string weaponName = "";
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
	public GameObject projectile;
	public Transform muzzlePos;
	
	public bool isReloading = false;
	protected float nextFireTime = 0.0f;
	protected float lastFrameShot = -1;
	protected ParticleEmitter hitParticles;

	public virtual void Start(){
		maxClips = clips;
		bulletsLeft = bulletsPerClip;
	}
	
	public virtual void Update(){
		if(Input.GetButton("Fire1") && WeaponSelection.canShoot){
			Fire();
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
			Quaternion.Euler(Random.Range(-coneAngle, coneAngle),Random.Range(-coneAngle, coneAngle),0);
		GameObject visibleProj = (GameObject)Instantiate(projectile, muzzlePos.position, muzzlePos.rotation * coneRandomRotation);
		
		switch(state){
		case WeaponState.RAYCAST:
			Bullet bullet = visibleProj.GetComponent<Bullet>();
		    Vector3 direction = transform.TransformDirection(Vector3.forward);
		  	RaycastHit hit;
			//LayerMask layermaskPlayer = 8;
			//LayerMask layermaskFort = 9;
			//LayerMask layermaskFinal = ~((1<<layermaskPlayer)|1<<layermaskFort);
			
		  	// Does the ray intersect any objects excluding the player and fort layer
		  	if(Physics.Raycast(transform.position, direction, out hit, range)){//, layermaskFinal)){
				// Apply a force to the rigidbody we hit
				if(hit.rigidbody){
					hit.rigidbody.AddForceAtPosition(force * direction, hit.point, ForceMode.Impulse);
				}
				bullet.distance = hit.distance;
				// Place the particle system for spawing out of place where we hit the surface!
				// And spawn a couple of particles
				if(hitParticles){
					hitParticles.transform.position = hit.point;
					hitParticles.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					hitParticles.Emit();
				}
				Debug.DrawRay(transform.position, direction * hit.distance, Color.blue);
				// Send a damage message to the hit object
				hit.collider.gameObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
		  	} else {
				bullet.distance = range;
		    	Debug.DrawRay(transform.position, direction * range, Color.green);
		  	}
			
			bulletsLeft--;
			
			// Register that we shot this frame,
			// so that the LateUpdate function enabled the muzzleflash renderer for one frame
			lastFrameShot = Time.frameCount;
			//enabled = true;
			break;
		case WeaponState.PROJECTILE:
			bulletsLeft--;
			
			// Register that we shot this frame,
			// so that the LateUpdate function enabled the muzzleflash renderer for one frame
			lastFrameShot = Time.frameCount;
			//enabled = true;
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
