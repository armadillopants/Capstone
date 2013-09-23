using UnityEngine;

public class DestroyTimer : MonoBehaviour {
	
	public float timer;

	void Awake(){
		Invoke("Destroy", timer);
	}

	void Destroy(){
		Destroy(gameObject);
	}
}
