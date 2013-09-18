using UnityEngine;
using System.Collections;

public class BurstShot : BaseWeapon {
	
	public override void Update(){
		if(Input.GetButtonDown("Fire1") && GameController.Instance.canShoot){
			StartCoroutine("BurstFire");
		}
	}
	
	public IEnumerator BurstFire(){
		
		int shotCounter = 0;
		
		if(bulletsLeft <= 0 && !isReloading){
			StartCoroutine("Reload");
			yield return null;
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
				yield return new WaitForSeconds(lagBetweenShots);
			}
			nextFireTime += fireRate;
		}
	}
}