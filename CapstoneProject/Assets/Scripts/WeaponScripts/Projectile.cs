using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	private Transform trans;
	public GameObject explosion;
	public float bulletSpeed = 0.0f;
	public float lifeTime = 0.0f;
	public float radius = 0.0f;
	public float damage = 0f;
	public float forceAmount = 0f;
	public GameObject target;
	public bool isHoming = false;
	public float damp = 6.0f;

	// Use this for initialization
	void Start(){
		trans = transform;
		Invoke("Kill", lifeTime);
	}
	
	void Update(){
		if(isHoming){
			target = FindNearestTarget();
			if(target){
				trans.Translate(Vector3.forward*bulletSpeed*Time.deltaTime);
				Quaternion rotate = Quaternion.LookRotation(target.transform.position - trans.position);
				trans.rotation = Quaternion.Slerp(trans.rotation, rotate, Time.deltaTime * damp);
			} else {
				trans.Translate(Vector3.forward*bulletSpeed*Time.deltaTime);
			}
		} else {
			trans.Translate(Vector3.forward*bulletSpeed*Time.deltaTime);
		}
		
		Collider[] hits = Physics.OverlapSphere(trans.position, radius);
		bool collided = false;
		foreach(Collider hit in hits){
			if(hit.isTrigger){
				continue;
			}
			hit.collider.gameObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			
			if(hit.rigidbody){
				Vector3 force = trans.forward * forceAmount;
				hit.rigidbody.AddForce(force, ForceMode.Impulse);
			}
			collided = true;
		}
		if(collided){
			Kill();
		}
	}
	
	void Kill(){
		if(explosion != null){
			Instantiate(explosion, trans.position, trans.rotation);
		}
		// Stop emitting particles in any children
		ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
		if(emitter){
			emitter.emit = false;
		}

		// Detach children
		trans.DetachChildren();
		
		// Destroy the projectile
		Destroy(gameObject);
	}
	
	GameObject FindNearestTarget(){
		GameObject[] targets;
		targets = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		
		foreach(GameObject targetCheck in targets){
			Vector3 diff = (targetCheck.transform.position - trans.position);
			float curDist = diff.sqrMagnitude;
			if(curDist < distance){
				closest = targetCheck;
				distance = curDist;
			}
		}
		return closest;
	}
}
