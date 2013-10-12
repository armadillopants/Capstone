using UnityEngine;

public class ShipDecay : MonoBehaviour {
	
	public GameObject[] particles;
	
	public void Release(){
		int i = Random.Range(0, particles.Length);
		if(!particles[i].particleEmitter.emit){
			particles[i].particleEmitter.emit = true;
		}
	}
}
