using UnityEngine;

public class BarrelDamage : MonoBehaviour {
	
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == Globals.ENEMY){
			collision.gameObject.SendMessageUpwards("TakeDamage", AbilitiesManager.Instance.orbitAbility.damage, SendMessageOptions.DontRequireReceiver);
			gameObject.GetComponent<Health>().TakeDamage(AbilitiesManager.Instance.orbitAbility.damage);
		}
	}
}
