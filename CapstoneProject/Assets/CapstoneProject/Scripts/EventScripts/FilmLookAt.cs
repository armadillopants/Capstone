using UnityEngine;

public class FilmLookAt : MonoBehaviour {
	
	public Transform target;
	
	void Update(){
		transform.rotation = Quaternion.Slerp(transform.rotation, 
			Quaternion.LookRotation(target.position-transform.position), 1f*Time.deltaTime);
	}
}
