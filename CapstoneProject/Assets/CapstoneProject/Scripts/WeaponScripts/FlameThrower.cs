using UnityEngine;
using System.Collections;

public class FlameThrower : BaseWeapon {
	
	public override void Update(){
		if(Input.GetButton("Fire1") && GameController.Instance.canShoot){
			ParticleManager.MoveParticlesWithPlayerVelocity();
			Vector3 vel = hitParticles.localVelocity;
			vel.z = range;
			hitParticles.localVelocity = vel;
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
