using UnityEngine;

public class FilmLookAt : MonoBehaviour {
	
	public Transform target;
	
	void Start(){
		target = GameObject.FindWithTag(Globals.PLAYER).transform;
	}
	
	void Update(){
		if(target){
			transform.rotation = Quaternion.Slerp(transform.rotation, 
				Quaternion.LookRotation(target.position-transform.position), 1f*Time.deltaTime);
		}
	}
}
