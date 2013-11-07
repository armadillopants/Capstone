using System.Collections;
using UnityEngine;

public class LightningBlaster : BaseWeapon {
	
	private Transform[] targets = new Transform[2];
	public int zigs = 150;
	public float speed = 1f;
	public Light startLight;
	public Light endLight;
	
	public GameObject secondStrike;

	private Perlin noise;
	private float oneOverZigs;
	
	private Particle[] particles;
	
	void Start(){
		oneOverZigs = 1f / (float)zigs;
		
		hitParticles.Emit(zigs);
		particles = hitParticles.particles;
		
		noise = new Perlin();
	}
	
	void Update(){
		if(!isFiring){
			startLight.light.enabled = false;
			endLight.light.enabled = false;
			targets[0] = null;
		}
	}
	
	public override void Fire(){
		
		if(clips > 0){
			
			if(targets[0] != null){
				
				float timex = Time.time * speed * 0.1365143f;
				float timey = Time.time * speed * 1.21688f;
				float timez = Time.time * speed * 2.5564f;
				
				for(int i=0; i<particles.Length; i++){
					Vector3 position = Vector3.Lerp(transform.position, targets[0].position, oneOverZigs * (float)i);
					Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
												noise.Noise(timey + position.x, timey + position.y, timey + position.z),
												noise.Noise(timez + position.x, timez + position.y, timez + position.z));
					position += (offset * coneAngle * ((float)i * oneOverZigs));
					
					particles[i].position = position;
					particles[i].color = Color.white;
					particles[i].energy = 1f;
				}
				
				hitParticles.particles = particles;
				
				if(hitParticles.particleCount >= 2){
					if(startLight){
						startLight.light.enabled = true;
						startLight.transform.position = particles[0].position;
					}
					if(endLight){
						endLight.light.enabled = true;
						endLight.transform.position = particles[particles.Length-1].position;
						/*targets[1] = GameController.Instance.FindNearestTarget(Globals.ENEMY, endLight.transform).transform;
						
						if(targets[1] != null){
							GameObject strike = (GameObject)Instantiate(secondStrike, endLight.transform.position, Quaternion.identity);
							strike.GetComponent<LightningBolt>().target = targets[1];
							strike.transform.parent = endLight.transform;
						}*/	
					}
				}
			} else {
				targets[0] = GameController.Instance.FindNearestTarget(Globals.ENEMY, transform).transform;
			}
		}
		
		base.Fire();
	}
}