using UnityEngine;
using System.Collections;

public class FlameThrower : BaseWeapon {
	
	public override void Update(){
		if(Input.GetButton("Fire1") && selection.canShoot){
			Fire();
		} else {
			hitParticles.emit = false;
		}
		if(bulletsLeft <= 0){
			hitParticles.emit = false;
		}
	}
	
	public override void Fire(){
		hitParticles.emit = true;
		
		base.Fire();
	}
}
