using UnityEngine;

public class StrikerDamage : MonoBehaviour {
	
	private float hitTimer = 0;
	private float hitTimerMax = 1f;
	
	void Update(){
		if(hitTimer > 0){
			hitTimer -= Time.deltaTime;
		}
	}
	
	void OnTriggerStay(Collider other){
		if(hitTimer <= 0){
			if(other.gameObject.tag == Globals.ENEMY){
				other.gameObject.collider.SendMessageUpwards("TakeDamage", AbilitiesManager.Instance.strikerAbility.damage, SendMessageOptions.DontRequireReceiver);
			}
			hitTimer = hitTimerMax;
		}
	}
}
