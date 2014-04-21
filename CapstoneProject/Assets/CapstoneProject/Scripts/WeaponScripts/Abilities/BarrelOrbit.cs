using UnityEngine;

public class BarrelOrbit : MonoBehaviour {
	
	private Transform player;
	public float orbitalDegrees = 360f;
	public bool randomRot = false;
	private Vector3 distance = Vector3.zero;

	void Start(){
		player = GameController.Instance.GetPlayer();
		
		if(player != null){
			distance = transform.position - player.position;
		}
	}
	
	void LateUpdate(){
		Orbit();
	}
	
	void Orbit(){
		if(player != null){
			if(!randomRot){
				transform.position = player.position + distance;
				transform.RotateAround(player.position, Vector3.up, orbitalDegrees * Time.deltaTime);
				distance = transform.position - player.position;
			} else {
				transform.position = player.position + distance;
				transform.RotateAround(new Vector3(Random.Range(player.position.x-10, player.position.x+10), 
													0, Random.Range(player.position.z-10, player.position.z+10)), 
													Vector3.up, orbitalDegrees * Time.deltaTime);
				distance = transform.position - player.position;
			}
		}
	}
}