using UnityEngine;
using System.Collections;

public class Crusher : Enemy {
	
	private float spawningTime = Random.Range(30f, 60f);
	private int amountToSpawn = Random.Range(1, 5);
	public GameObject scavenger;
	public AudioClip crusherGrowl;
	
	void OnDisable(){
		spawningTime = Random.Range(30f, 60f);
		amountToSpawn = Random.Range(1, 5);
	}
	
	public override void Update(){
		base.Update();
		
		if(spawningTime > 0){
			spawningTime -= Time.deltaTime;
		} else {
			for(int i=0; i<amountToSpawn; i++){
				Vector3 pos = tr.position + new Vector3(Mathf.Cos(Random.Range(0,360)), 
											1, 
											Mathf.Sin(Random.Range(0,360)))*(Random.Range(5,5));
				GameObject scav = ObjectPool.Spawn(scavenger, new Vector3(pos.x, -1f, pos.z), Quaternion.Euler(-90f,0,0));
				// Don't want any more objects than needed, these are temps
				scav.GetComponent<Enemy>().isUnderground = true;
				scav.AddComponent<DigOutOfGround>();
				scav.AddComponent<DestroyWhenDead>();
				scav.name = scavenger.name;
			}
			spawningTime = Random.Range(30f, 60f);
			amountToSpawn = Random.Range(1, 5);
			audio.PlayOneShot(crusherGrowl);
		}
	}
}
