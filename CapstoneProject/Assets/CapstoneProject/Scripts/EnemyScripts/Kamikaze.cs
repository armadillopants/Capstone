using UnityEngine;
using System.Collections;

public class Kamikaze : BaseEnemy {
	
	public GameObject explosion;
	
	public override void Attack(GameObject target){
		if(doDamage){
			if(currentCoolDown <= 0){
				target.gameObject.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
				Instantiate(explosion, tr.position, Quaternion.identity);
				Destroy(gameObject);
			}
		}
	}
}
