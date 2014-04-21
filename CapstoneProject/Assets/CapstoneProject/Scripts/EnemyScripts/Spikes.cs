using UnityEngine;

public class Spikes : MonoBehaviour {
	
	private float hitTimer = 0;
	private float hitTimerMax = 1f;
	
	void Update(){
		if(hitTimer > 0){
			hitTimer -= Time.deltaTime;
		}
	}
	
	void OnTriggerStay(Collider hit){
		if(hitTimer <= 0){
			if(hit.tag == Globals.FORTIFICATION){
				hit.GetComponent<Health>().TakeDamage(5f);
			}
			hitTimer = hitTimerMax;
		}
		
		if(hitTimer <= 0){
			if(hit.tag == Globals.PLAYER){
				hit.collider.gameObject.SendMessageUpwards("TakeDamage", 5f, SendMessageOptions.DontRequireReceiver);
			}
			hitTimer = hitTimerMax;
		}
	}
}
