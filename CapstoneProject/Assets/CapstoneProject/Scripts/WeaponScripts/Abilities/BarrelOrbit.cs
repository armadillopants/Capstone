using UnityEngine;

public class BarrelOrbit : MonoBehaviour {
	
	private Transform player;
	private float orbitalDegrees = 360f;
	private Vector3 distance = Vector3.zero;

	void Start(){
		player = GameObject.FindWithTag("Player").transform;
		
		if(player != null){
			distance = transform.position - player.position;
		}
	}
	
	void LateUpdate(){
		Orbit();
	}
	
	void Orbit(){
		if(player != null){
			transform.position = player.position + distance;
			transform.RotateAround(player.position, Vector3.up, orbitalDegrees * Time.deltaTime);
			distance = transform.position - player.position;
		}
	}
}