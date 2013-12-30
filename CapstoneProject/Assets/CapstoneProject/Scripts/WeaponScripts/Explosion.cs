using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	public float blastRadius = 5.0f;
	public float explosivePower = 10.0f;
	public float blastDamage = 100.0f;
	public float timeOut = 3.0f;
	public string[] targetName;
	public bool dealShieldDamage = false;

	void Start(){
		Vector3 pos = transform.position;
		
		// Apply damage to close by objects first
		Collider[] colliders = Physics.OverlapSphere(pos, blastRadius);
		foreach(Collider hit in colliders){
			// Calculate distance from the explosion position to the closest point on the collider
			Vector3 closestPoint = hit.ClosestPointOnBounds(pos);
			float distance = Vector3.Distance(closestPoint, pos);
			
			// The hit points we apply decrease with distance from the explosion point
			double hitPoints = 1.0 - Mathf.Clamp01(distance/blastRadius);
			hitPoints *= blastDamage;
			
			for(int i=0; i<targetName.Length; i++){
				if(hit.tag == targetName[i]){
					if(dealShieldDamage){
						if(hit.tag == "Shield"){
							hit.renderer.enabled = true;
						}
					}
					// Tell the rigidbody or any other script attached to the hit object how much damage is to be applied
					hit.SendMessageUpwards("TakeDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		// Apply explosion forces to all rigidbodies
		foreach(Collider hit in colliders){
			if(hit.rigidbody){
				hit.rigidbody.AddExplosionForce(explosivePower, pos, blastRadius, 1.0f);
			}
		}
		// Stop emitting particles
		if(particleEmitter){
			particleEmitter.emit = true;
			StartCoroutine("TurnOffParticles");
		}
		
		// Destroy the explosion after awhile
		Destroy(gameObject, timeOut);
	}
	
	IEnumerator TurnOffParticles(){
		yield return new WaitForSeconds(0.5f);
		particleEmitter.emit = false;
	}
}