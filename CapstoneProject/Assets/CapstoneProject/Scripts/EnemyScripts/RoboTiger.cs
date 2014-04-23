using UnityEngine;

public class RoboTiger : Enemy {
	
	private float spawningTime = Random.Range(30f, 60f);
	private int amountToSpawn = Random.Range(1, 5);
	public GameObject cyberCat;
	public GameObject beamDown;
	public AudioClip tigerGrowl;
	
	public override void Update(){
		base.Update();
		
		if(spawningTime > 0){
			spawningTime -= Time.deltaTime;
		} else {
			for(int i=0; i<amountToSpawn; i++){
				Vector3 pos = tr.position + new Vector3(Mathf.Cos(Random.Range(0,360)), 
											1, 
											Mathf.Sin(Random.Range(0,360)))*(Random.Range(5,5));
				ObjectPool.Spawn(beamDown, pos, Quaternion.identity);
				GameObject cat = ObjectPool.Spawn(cyberCat, pos, Quaternion.identity);
				// Don't want any more objects than needed, these are temps
				cat.AddComponent<DestroyWhenDead>();
				cat.name = cyberCat.name;
			}
			spawningTime = Random.Range(30f, 60f);
			amountToSpawn = Random.Range(1, 5);
			audio.PlayOneShot(tigerGrowl);
		}
	}
}
