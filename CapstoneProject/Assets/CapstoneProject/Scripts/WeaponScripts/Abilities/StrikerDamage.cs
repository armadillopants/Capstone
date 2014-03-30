using UnityEngine;

public class StrikerDamage : MonoBehaviour {
	
	void OnTriggerStay(Collider other){
		if(other.gameObject.tag == Globals.ENEMY){
			other.gameObject.collider.SendMessageUpwards("TakeDamage", AbilitiesManager.Instance.strikerAbility.damage, SendMessageOptions.DontRequireReceiver);
		}
	}
}
