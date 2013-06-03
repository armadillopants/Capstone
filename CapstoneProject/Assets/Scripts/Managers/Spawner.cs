using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public GameObject enemy;
	private Vector3 pos;

	// Use this for initialization
	void Start(){
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SpawnEnemy(){
		pos = transform.position + 
			new Vector3(Mathf.Cos(Random.Range(0,360)), transform.position.y, Mathf.Sin(Random.Range(0,360)))*(Random.Range(40,40));
		Instantiate(enemy, pos, Quaternion.identity);
	}
}
