using UnityEngine;

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
			trans.position += trans.forward * bulletSpeed * Time.deltaTime;
			Quaternion rotate = Quaternion.LookRotation(target.transform.position - trans.position);
			trans.rotation = Quaternion.Slerp(trans.rotation, rotate, Time.deltaTime * 6f);
		} else {
			trans.position += trans.forward * bulletSpeed * Time.deltaTime;
		}
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
