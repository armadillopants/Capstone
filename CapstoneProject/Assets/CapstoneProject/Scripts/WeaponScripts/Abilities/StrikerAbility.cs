using UnityEngine;
using System.Collections;

public class StrikerAbility : MonoBehaviour {
	
	private Transform holder;
	private Transform[] hold = new Transform[3];
	private Vector3[] pos = new Vector3[3];
	private GameObject[] strikers;
	private GameObject striker;
	private GameObject trigger;
	private Transform player;
	private bool attachStrikerToHolder = false;
	private float timer;
	private float maxTimer = 30f;

	void Start(){
		player = GameObject.FindWithTag(Globals.PLAYER).transform;
		striker = (GameObject)Resources.Load("BeamStrike", typeof(GameObject));
		holder = (Transform)Resources.Load("OrbitHolder", typeof(Transform));
		trigger = (GameObject)Resources.Load("Trigger", typeof(GameObject));
		
		pos[0] = new Vector3(0, 0, 6);
		pos[1] = new Vector3(6, 0, -6);
		pos[2] = new Vector3(-6, 0, -6);
		
		timer = maxTimer;
	}
	
	void Update(){
		if(attachStrikerToHolder){
			AttachStriker();
			
			timer -= 1f*Time.deltaTime;
			if(timer <= 0){
				foreach(GameObject strike in GameObject.FindGameObjectsWithTag(Globals.ABILITY)){
					Destroy(strike);
				}
				timer = maxTimer;
			}
		}
	}
	
	void BeginAbility(){
		StartCoroutine("SpawnOrbitHolders");
	}
	
	private IEnumerator SpawnOrbitHolders(){
		for(int i=0; i<pos.Length; i++){
			// Spawn containers at positions
			hold[i] = (Transform)Instantiate(holder, player.position + new Vector3(pos[i].x, pos[i].y, pos[i].z), Quaternion.identity);
			hold[i].gameObject.AddComponent<BarrelOrbit>();
			hold[i].gameObject.GetComponent<BarrelOrbit>().orbitalDegrees = 10f;
			hold[i].gameObject.GetComponent<BarrelOrbit>().randomRot = true;
		}
		
		SpawnOrbitalStriker();
		
		yield return new WaitForSeconds(0.00001f);
		attachStrikerToHolder = true;
	}

	void SpawnOrbitalStriker(){
		// Spawn orbital striker and trigger for damage
		for(int i=0; i<hold.Length; i++){
			GameObject s = (GameObject)Instantiate(striker, hold[i].position+new Vector3(0,20,0), Quaternion.identity);
			GameObject t = (GameObject)Instantiate(trigger, hold[i].position, Quaternion.identity);
			t.transform.parent = s.transform;
		}
		
		strikers = GameObject.FindGameObjectsWithTag(Globals.ABILITY);
	}

	void AttachStriker(){
		for(int i=0; i<hold.Length; i++){
			if(strikers[i]){
				// Constantly lerp position to container
				strikers[i].transform.position = Vector3.Lerp(strikers[i].transform.position, 
															hold[i].position+new Vector3(0,20,0), 10*Time.deltaTime);
			}
			
			if(strikers[i] == null){
				if(hold[i]){
					// Destroy container if object is destroyed
					Destroy(hold[i].gameObject);
				}
			}
			
			if(hold[0] == null && hold[1] == null && hold[2] == null){
				player.GetComponent<AbilitiesManager>().SetCoolDown();
				attachStrikerToHolder = false;
			}
		}
	}
}
