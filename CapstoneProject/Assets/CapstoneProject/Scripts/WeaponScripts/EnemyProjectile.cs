using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {
	
	private Transform trans;
	public float bulletSpeed = 0.0f;
	private BaseWeapon weapon;
	
	void Start(){
		trans = transform;
		weapon = GameObject.Find("EnemyFlamethrower").GetComponent<BaseWeapon>();
		Invoke("Kill", weapon.range);
	}
	
	void Update(){
		GameObject target = GameController.Instance.FindNearestTarget(Globals.PLAYER, this.trans);
		if(target){
			rigidbody.velocity = trans.TransformDirection(Vector3.forward*bulletSpeed);
			Quaternion rotate = Quaternion.LookRotation(target.transform.position - trans.position);
			trans.rotation = Quaternion.Slerp(trans.rotation, rotate, Time.deltaTime * 6f);
		} else {
			rigidbody.velocity = trans.TransformDirection(Vector3.forward*bulletSpeed);
		}
		//rigidbody.velocity = trans.TransformDirection(Vector3.forward*bulletSpeed);
	}
	
	void OnCollisionEnter(Collision collision){
		
		if(collision.collider.gameObject.tag == Globals.PLAYER){
			collision.collider.gameObject.SendMessageUpwards("TakeDamage", weapon.damage, SendMessageOptions.DontRequireReceiver);
			
			if(collision.rigidbody){
				Vector3 force = trans.forward * weapon.force;
				collision.rigidbody.AddForce(force, ForceMode.Impulse);
			}
			Kill();
		}
	}
	
	void Kill(){
		// Stop emitting particles in any children
		ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
		if(emitter){
			emitter.emit = false;
		}

		// Detach children
		//trans.DetachChildren();
		
		// Destroy the projectile
		Destroy(gameObject);
	}
}
