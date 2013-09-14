using UnityEngine;

public class Spawner : MonoBehaviour {
	
	public GameObject enemy;
	private Vector3 pos;
	
	private GameObject[] spawnPoints;
	private Transform spawnPoint;

	void Start(){
		spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
	}
	
	void Update(){
	}
	
	public void SpawnEnemy(){
		spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)].transform;
		pos = spawnPoint.position + new Vector3(Mathf.Cos(Random.Range(0,360)), 
												0, 
												Mathf.Sin(Random.Range(0,360)))*(Random.Range(3,3));
		Instantiate(enemy, pos, Quaternion.identity);
		
		/*pos = transform.position + 
			new Vector3(Mathf.Cos(Random.Range(0,360)), transform.position.y+1, Mathf.Sin(Random.Range(0,360)))*(Random.Range(10,10));
		Instantiate(enemy, pos, Quaternion.identity);*/
	}
}
