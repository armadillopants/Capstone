using UnityEngine;

public class FortificationData : MonoBehaviour {
	
	public Health health;
	public float fortDamage = 0f;
	public BaseWeapon weapon;
	
	private float hitTimer = 0;
	private float hitTimerMax = 0.5f;
	
	void Update(){
		if(hitTimer > 0){
			hitTimer -= Time.deltaTime;
		}
	}
	
	void OnCollisionStay(Collision collision){
		if(hitTimer <= 0){
			if(collision.gameObject.tag == Globals.ENEMY){
				collision.gameObject.SendMessageUpwards("TakeDamage", fortDamage, SendMessageOptions.DontRequireReceiver);
			}
			hitTimer = hitTimerMax;
		}
	}
}
