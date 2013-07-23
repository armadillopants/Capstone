using UnityEngine;
using System.Collections;

public class RockRainAbility : MonoBehaviour {
	
	private Rigidbody rock;
	private Transform player;
	private int spawnAmount = 8;
	private Vector3 pos;

	void Start(){
		rock = (Rigidbody)Resources.Load("Rock", typeof(Rigidbody));
		player = GameObject.FindWithTag("Player").transform;
	}
	
	void BeginAbility(){
		for(int i=0; i<spawnAmount; i++){
			pos = player.position + 
				new Vector3(Mathf.Cos(Random.Range(0, 360)), player.position.y+3, Mathf.Sin(Random.Range(0, 360)))*(Random.Range(5, 10));
			Rigidbody r =  (Rigidbody)Instantiate(rock, pos, Quaternion.identity);
			r.velocity = transform.TransformDirection(Vector3.down * 50f);
		}
	}
}
