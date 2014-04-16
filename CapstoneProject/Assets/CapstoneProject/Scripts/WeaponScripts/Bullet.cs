using UnityEngine;

public class Bullet : MonoBehaviour {
	
	private Transform trans;
	public float bulletSpeed = 0f;
	public float lifeTime = 0f;
	public float distance = 0f;
	private float spawnTime = 0f;

	void OnEnable(){
		trans = transform;
		spawnTime = Time.time;
	}
	
	void Update(){
		trans.position += trans.forward * bulletSpeed * Time.deltaTime;
		distance -= bulletSpeed * Time.deltaTime;
		if(Time.time > spawnTime + lifeTime || distance < 0){
			//Destroy(gameObject);
			ObjectPool.spawner.DestroyCachedObject(gameObject);
		}
	}
}
