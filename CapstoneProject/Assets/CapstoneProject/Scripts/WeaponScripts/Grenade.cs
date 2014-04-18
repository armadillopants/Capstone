using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	
	public GameObject explosion;
	private BaseWeapon weapon;
	
	void Start(){
		weapon = GameController.Instance.GetPlayer().GetComponentInChildren<BaseWeapon>();
		Invoke("Kill", 3f);
	}
	
	void Update(){
		rigidbody.velocity = transform.TransformDirection(new Vector3(0, Physics.gravity.y/3, weapon.range));
	}
	
	void OnCollisionEnter(Collision collision){
		// Instantiate explosion at the impact point and rotate the explosion
		// so that the y-axis faces along the surface normal
		ContactPoint contact = collision.contacts[0];
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Vector3 pos = contact.point;
		if(explosion){
			ObjectPool.Spawn(explosion, pos, rotation);
		}
		
		collision.collider.gameObject.SendMessageUpwards("TakeDamage", weapon.damage, SendMessageOptions.DontRequireReceiver);
		
		if(collision.rigidbody){
			Vector3 force = transform.forward * weapon.force;
			collision.rigidbody.AddForce(force, ForceMode.Impulse);
		}
	
		// Call function to destroy the rocket
		Kill();
	}
	
	void Kill(){
		// Stop emitting particles in any children
		ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
		if(emitter){
			emitter.emit = false;
		}
		
		// Destroy the projectile
		Destroy(gameObject);
	}
}
