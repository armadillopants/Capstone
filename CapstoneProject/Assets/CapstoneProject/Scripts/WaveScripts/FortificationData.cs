using UnityEngine;
using System.Collections;

public class FortificationData : MonoBehaviour {
	
	public Health health;
	public float fortDamage = 0f;
	public BaseWeapon weapon;
	
	void OnCollisionStay(Collision collision){
		if(collision.gameObject.tag == Globals.ENEMY){
			collision.gameObject.SendMessageUpwards("TakeDamage", fortDamage, SendMessageOptions.DontRequireReceiver);
		}
	}
}
