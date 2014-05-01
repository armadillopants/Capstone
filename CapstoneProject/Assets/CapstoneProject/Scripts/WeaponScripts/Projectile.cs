using UnityEngine;

public class Projectile : MonoBehaviour {
	
	private Transform trans;
	public GameObject explosion;
	public BaseWeapon weapon;
	public float bulletSpeed = 0.0f;
	public bool isHoming = false;
	public float damp = 6.0f;
	public bool isPlayers = true;
	
	private float lifeTime = 0f;
	private float distance = 1000f;
	private float spawnTime = 0f;
	
	void OnEnable(){
		trans = transform;
		spawnTime = Time.time;
		if(isPlayers){
			weapon = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponManager>().allWeapons[6];
			lifeTime = weapon.range;
			distance = 1000f;
			bulletSpeed = weapon.projectile.GetComponent<Projectile>().bulletSpeed;
			isHoming = weapon.projectile.GetComponent<Projectile>().isHoming;
			damp = weapon.projectile.GetComponent<Projectile>().damp;
		} else {
			lifeTime = 3f;
			distance = 1000f;
		}
	}
	
	void Update(){
		if(isHoming){
			GameObject target = GameController.Instance.FindNearestTarget(Globals.ENEMY, this.trans);
			if(target){
				trans.position += trans.forward * bulletSpeed * Time.deltaTime;
				Quaternion rotate = Quaternion.LookRotation(target.transform.position - trans.position);
				trans.rotation = Quaternion.Slerp(trans.rotation, rotate, Time.deltaTime * damp);
			} else {
				trans.position += trans.forward * bulletSpeed * Time.deltaTime;
			}
		} else {
			trans.position += trans.forward * bulletSpeed * Time.deltaTime;
		}
		
		distance -= bulletSpeed * Time.deltaTime;
		if(Time.time > spawnTime + lifeTime || distance < 0){
			ObjectPool.DestroyCachedObject(gameObject);
		}
	}
	
	void OnCollisionEnter(Collision collision){
		
		if(weapon){
			collision.collider.gameObject.SendMessageUpwards("TakeDamage", weapon.damage, SendMessageOptions.DontRequireReceiver);
		
			if(collision.rigidbody){
				Vector3 force = trans.forward * weapon.force;
				collision.rigidbody.AddForce(force, ForceMode.Impulse);
			}
		}
		
		if(explosion){
			// Instantiate explosion at the impact point and rotate the explosion
			// so that the y-axis faces along the surface normal
			ContactPoint contact = collision.contacts[0];
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
			Vector3 pos = contact.point;
			
			ObjectPool.Spawn(explosion, pos, rotation);
		
			// Call function to destroy the rocket
			Kill();
		}
	}
	
	void Kill(){
		// Stop emitting particles in any children
		//ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
		/*if(emitter){
			emitter.emit = false;
		}*/

		// Detach children
		//trans.DetachChildren();
		
		// Destroy the projectile
		//Destroy(gameObject);
		ObjectPool.DestroyCachedObject(gameObject);
	}
}
