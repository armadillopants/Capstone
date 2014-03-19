using UnityEngine;
using System.Collections;

public class DeflectBullets : MonoBehaviour {
	
	void OnCollisionEnter(Collision collision){
		if(collision.collider.gameObject.name == "RedLaser"){
			Debug.Log("I'm HIT!!!!!");
		}
	}
}
