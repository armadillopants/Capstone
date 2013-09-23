using UnityEngine;
using System.Collections;

public class OrbitAbility : MonoBehaviour {
	
	private Transform holder;
	private Transform[] hold = new Transform[4];
	private Vector3[] pos = new Vector3[4];
	private Transform player;
	private bool attachItemsToPlayer = false;
	private GameObject[] items;
	
	void Start(){
		player = GameObject.FindWithTag(Globals.PLAYER).transform;
		
		holder = (Transform)Resources.Load("OrbitHolder", typeof(Transform));
		
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
		Collider[] hits = Physics.OverlapSphere(player.position, Mathf.Infinity);
		foreach(Collider hit in hits){
			if(hit.tag == Globals.INTERACTABLE_ITEM){
				// Set items to objects hit
				items = GameObject.FindGameObjectsWithTag(hit.tag);
				DistanceComparer dComp = new DistanceComparer();
				dComp.SetTarget(player.gameObject);
				// Sort objects based on distance from player
				System.Array.Sort(items, dComp);
			}
		}
		
		// TODO: Possibly fix this to make it more dynamic...work with less than 4 objects
		if(items.Length >= 4){
			StartCoroutine("SpawnOrbitHolders");
		}
	}
	
	private IEnumerator SpawnOrbitHolders(){
		for(int i=0; i<pos.Length; i++){
			hold[i] = (Transform)Instantiate(holder, player.position + new Vector3(pos[i].x, pos[i].y, pos[i].z), Quaternion.identity);
			hold[i].gameObject.AddComponent<BarrelOrbit>();
			items[i].gameObject.AddComponent<Health>().curHealth = 100;
			items[i].gameObject.AddComponent<BarrelDamage>();
		}
		
		yield return new WaitForSeconds(0.00001f);
		attachItemsToPlayer = true;
	}
	
	void CollectItems(){
		for(int i=0; i<hold.Length; i++){
			if(items[i]){
				items[i].transform.position = Vector3.Lerp(items[i].transform.position, hold[i].position, 10*Time.deltaTime);
			}
			
			if(items[i] == null){
				if(hold[i]){
					Destroy(hold[i].gameObject);
				}
			}
			
			if(hold[0] == null && hold[1] == null && hold[2] == null && hold[3] == null){
				player.GetComponent<AbilitiesManager>().SetCoolDown();
				attachItemsToPlayer = false;
			}
		}
	}
}
