using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	private Transform trans;
	public float bulletSpeed = 0f;
	public float lifeTime = 0f;
	public float distance = 0f;
	private float spawnTime = 0f;

	void Start(){
		trans = transform;
		spawnTime = Time.time;
	}
	
	void Update(){
		rigidbody.velocity = trans.TransformDirection(Vector3.forward*bulletSpeed);
		distance -= bulletSpeed * Time.deltaTime;
		if(Time.time > spawnTime + lifeTime || distance < 0){
			Destroy(gameObject);
		}
	}
}
