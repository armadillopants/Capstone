using UnityEngine;
using System.Collections;

public class FortificationData : MonoBehaviour {
	
	public Health health;
	public float damage = 0f;

	void Awake(){
		health = GetComponent<Health>();
	}
	
	void OnCollisionStay(Collision collision){
		if(collision.gameObject.tag == "Enemy"){
			collision.gameObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
		}
	}
}
