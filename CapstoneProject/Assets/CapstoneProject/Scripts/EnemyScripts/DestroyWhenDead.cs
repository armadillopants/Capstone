using UnityEngine;

public class DestroyWhenDead : MonoBehaviour {
	
	void OnDisable(){
		Destroy(gameObject);
	}
}
