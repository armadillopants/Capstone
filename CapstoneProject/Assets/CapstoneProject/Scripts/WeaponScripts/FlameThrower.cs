using UnityEngine;
using System.Collections;

public class FlameThrower : BaseWeapon {
	
	public override void Update(){
		if(!isFiring){
			hitParticles.emit = false;
		}
		
		if(bulletsLeft <= 0){
			hitParticles.emit = false;
		}
		
		base.Update();
	}
	
	public override void Fire(){
		if(transform.parent == GameController.Instance.GetPlayer()){
			ParticleManager.MoveParticlesWithPlayerVelocity();
		}
		Vector3 vel = hitParticles.localVelocity;
		vel.z = range;
		hitParticles.localVelocity = vel;
		hitParticles.emit = true;
		
		base.Fire();
	}
	
	void OnDisable(){
		hitParticles.emit = false;
	}
}
