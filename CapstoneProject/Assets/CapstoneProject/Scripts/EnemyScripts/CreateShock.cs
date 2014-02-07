using UnityEngine;

public class CreateShock : MonoBehaviour {
	
	public GameObject shockwave;
	
	void OnTriggerEnter(Collider hit){
		if(hit.tag == "Ground"){
			GameObject shock = (GameObject)Instantiate(shockwave, transform.position, Quaternion.identity);
			shock.AddComponent<DestroyTimer>();
		}
	}
}
