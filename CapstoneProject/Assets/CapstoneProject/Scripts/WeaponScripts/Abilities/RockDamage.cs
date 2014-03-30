using UnityEngine;

public class RockDamage : MonoBehaviour {
	
	public GameObject explosion;
	
	void OnCollisionEnter(Collision collision){
		// Instantiate explosion at the impact point and rotate the explosion
		// so that the y-axis faces along the surface normal
		ContactPoint contact = collision.contacts[0];
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Vector3 pos = contact.point;
		if(explosion){
			Instantiate(explosion, pos, rotation);
		}
		
		if(collision.transform.tag == Globals.ENEMY){
			collision.collider.gameObject.SendMessageUpwards("TakeDamage", AbilitiesManager.Instance.rockRainAbility.damage, SendMessageOptions.DontRequireReceiver);
		}
		
		if(collision.rigidbody){
			Vector3 force = transform.forward * 10f;
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

		// Detach children
		transform.DetachChildren();
		
		// Destroy the projectile
		Destroy(gameObject);
	}
}
