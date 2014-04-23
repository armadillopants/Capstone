using UnityEngine;

public class DestroyWhenDead : MonoBehaviour {
	
	private Health health;
	
	void Start(){
		health = GetComponent<Health>();
	}
	
	void OnDisable(){
		Destroy(gameObject);
	}
}
