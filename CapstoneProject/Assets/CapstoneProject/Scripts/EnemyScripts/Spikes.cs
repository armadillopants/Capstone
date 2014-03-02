using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour {
	
	void OnTriggerEnter(Collider hit){
		if(hit.tag == Globals.FORTIFICATION){
			GameController.Instance.UpdateGraphOnDestroyedObject(hit.gameObject.collider, hit.gameObject);
		}
		
		if(hit.tag == Globals.PLAYER){
			hit.collider.gameObject.SendMessageUpwards("TakeDamage", 5f, SendMessageOptions.DontRequireReceiver);
		}
	}
}
