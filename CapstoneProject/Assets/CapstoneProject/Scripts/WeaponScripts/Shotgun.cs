using UnityEngine;
using System.Collections;

public class Shotgun : BaseWeapon {
	
	/*public override void Update(){
		if(Input.GetButtonDown("Fire1") && GameController.Instance.canShoot){
			Fire();
		}
	}*/
	
	public override void Fire(){
		int shotCounter = 0;
		
		if(bulletsLeft <= 0 && !isReloading){
			StartCoroutine("Reload");
			return;
		}
		
		// If there is more than one bullet between the last and this frame
		// Reset the next_fire_time
		if(Time.time - fireRate > nextFireTime){
			nextFireTime = Time.time - Time.deltaTime;
		}
		
		// Keep firing until we used up the fire time
		while(nextFireTime < Time.time && bulletsLeft != 0){
			while(shotCounter < roundsPerBurst){
				CreateProjectile();
				shotCounter++;
			}
			nextFireTime += fireRate;
		}
	}
}
