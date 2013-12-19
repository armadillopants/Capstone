using UnityEngine;

public class Projectile : MonoBehaviour {
	
	private Transform trans;
	public GameObject explosion;
	private BaseWeapon weapon;
	public float bulletSpeed = 0.0f;
	public GameObject target;
	public bool isHoming = false;
	public float damp = 6.0f;
	
	void Awake(){
		Transform player = GameController.Instance.GetPlayer();
		weapon = player.GetComponentInChildren<BaseWeapon>();
	}
	
	void Start(){
		trans = transform;
		Invoke("Kill", weapon.range);
	}
	
	void Update(){
		if(isHoming){
			target = GameController.Instance.FindNearestTarget(Globals.ENEMY, this.trans);
			if(target){
				rigidbody.velocity = trans.TransformDirection(Vector3.forward*bulletSpeed);
				Quaternion rotate = Quaternion.LookRotation(target.transform.position - trans.position);
				trans.rotation = Quaternion.Slerp(trans.rotation, rotate, Time.deltaTime * damp);
			} else {
				rigidbody.velocity = trans.TransformDirection(Vector3.forward*bulletSpeed);
			}
		} else {
			rigidbody.velocity = trans.TransformDirection(Vector3.forward*bulletSpeed);
		}
	}
	
	void OnCollisionEnter(Collision collision){
		// Instantiate explosion at the impact point and rotate the explosion
		// so that the y-axis faces along the surface normal
		ContactPoint contact = collision.contacts[0];
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Vector3 pos = contact.point;
		if(explosion){
			Instantiate(explosion, pos, rotation);
		}
		
		collision.collider.gameObject.SendMessageUpwards("TakeDamage", weapon.damage, SendMessageOptions.DontRequireReceiver);
		
		if(collision.rigidbody){
			Vector3 force = trans.forward * weapon.force;
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
		//trans.DetachChildren();
		
		// Destroy the projectile
		Destroy(gameObject);
	}
}
