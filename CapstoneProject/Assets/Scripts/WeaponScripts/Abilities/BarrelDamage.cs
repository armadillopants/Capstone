using UnityEngine;

public class BarrelDamage : MonoBehaviour {
	
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "Enemy"){
			collision.gameObject.SendMessageUpwards("TakeDamage", 10f, SendMessageOptions.DontRequireReceiver);
			gameObject.GetComponent<Health>().TakeDamage(10f);
		}
	}
}
