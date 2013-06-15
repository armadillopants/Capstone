using UnityEngine;
using System.Collections;

public class OrbitAbility : MonoBehaviour {
	
	public Transform holder;
	private Transform[] hold = new Transform[4];
	private Vector3[] pos = new Vector3[4];
	private Transform player;
	private bool attachItemsToPlayer = false;
	private GameObject[] items;
	
	void Start(){
		player = GameObject.FindWithTag("Player").transform;
		items = GameObject.FindGameObjectsWithTag("InteractableItem");
		
		pos[0] = new Vector3(0,0,3);
		pos[1] = new Vector3(3,0,0);
		pos[2] = new Vector3(0,0,-3);
		pos[3] = new Vector3(-3,0,0);
	}
	
	void Update(){
		if(attachItemsToPlayer){
			for(int i=0; i<items.Length; i++){
				if(Vector3.Distance(items[i].transform.position, hold[i].position) < 30f){
					CollectItems();
				}
			}
		}
	}
	
	void BeginAbility(){
		StartCoroutine("SpawnOrbitHolders");
	}
	
	private IEnumerator SpawnOrbitHolders(){
		hold[0] = (Transform)Instantiate(holder, player.position + new Vector3(pos[0].x, pos[0].y, pos[0].z), Quaternion.identity);
		hold[1] = (Transform)Instantiate(holder, player.position + new Vector3(pos[1].x, pos[1].y, pos[1].z), Quaternion.identity);
		hold[2] = (Transform)Instantiate(holder, player.position + new Vector3(pos[2].x, pos[2].y, pos[2].z), Quaternion.identity);
		hold[3] = (Transform)Instantiate(holder, player.position + new Vector3(pos[3].x, pos[3].y, pos[3].z), Quaternion.identity);
		foreach(Transform obj in hold){
			obj.gameObject.AddComponent<BarrelOrbit>();
		}
		yield return new WaitForSeconds(0.001f);
		attachItemsToPlayer = true;
	}
	
	void CollectItems(){
		items[0].transform.position = Vector3.Lerp(items[0].transform.position, hold[0].position, 3*Time.deltaTime);
		items[1].transform.position = Vector3.Lerp(items[1].transform.position, hold[1].position, 3*Time.deltaTime);
		items[2].transform.position = Vector3.Lerp(items[2].transform.position, hold[2].position, 3*Time.deltaTime);
		items[3].transform.position = Vector3.Lerp(items[3].transform.position, hold[3].position, 3*Time.deltaTime);
	}
}
