using UnityEngine;

public class DestroyTimer : MonoBehaviour {
	
	public float timer;

	void Awake(){
		if(timer == 0){
			timer = 3f;
		}
		
		Invoke("Destroy", timer);
	}

	void Destroy(){
		Destroy(gameObject);
	}
}
