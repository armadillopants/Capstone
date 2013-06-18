using UnityEngine;
using System.Collections;

public class OrbitAbility : MonoBehaviour {
	
	public Transform holder;
	private Transform[] hold = new Transform[4];
	private Vector3[] pos = new Vector3[4];
	private Transform player;
	private bool attachItemsToPlayer = false;
	private GameObject[] items;
	private GameObject target;
	
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
			CollectItems();
		}
	}
	
	void BeginAbility(){
		StartCoroutine("SpawnOrbitHolders");
	}
	
	private IEnumerator SpawnOrbitHolders(){
		for(int i=0; i<pos.Length; i++){
			hold[i] = (Transform)Instantiate(holder, player.position + new Vector3(pos[i].x, pos[i].y, pos[i].z), Quaternion.identity);
			hold[i].gameObject.AddComponent<BarrelOrbit>();
		}
		
		yield return new WaitForSeconds(0.001f);
		attachItemsToPlayer = true;
	}
	
	void CollectItems(){
		for(int i=0; i<hold.Length; i++){
			items[i] = InteractbleItems();
			items[i].transform.position = Vector3.Lerp(items[i].transform.position, hold[i].position, 10*Time.deltaTime);
		}
	}
	
	GameObject InteractbleItems(){
		GameObject closest = null;
		float distance = 30f;
		
		for(int i=0; i<items.Length; i++){
			Vector3 diff = items[i].transform.position - hold[i].position;
			float curDist = diff.sqrMagnitude;
			if(curDist < distance){
				closest = items[i];
				distance = curDist;
			}
		}
		return closest;
	}
}
