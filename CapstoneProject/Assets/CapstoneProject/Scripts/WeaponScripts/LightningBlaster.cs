using UnityEngine;

public class LightningBlaster : BaseWeapon {
	
	public int zigs = 150;
	public float speed = 1f;
	public Light startLight;
	public Light endLight;

	private Perlin noise;
	private float oneOverZigs;
	
	private Particle[] particles;
	
	void Start(){
		oneOverZigs = 1f / (float)zigs;
		
		hitParticles.Emit(zigs);
		particles = hitParticles.particles;
		
		noise = new Perlin();
	}
	
	public override void Update(){
		base.Update();
		
		if(!isFiring){
			startLight.light.enabled = false;
			endLight.light.enabled = false;
		}
	}
	
	public override void Fire(){
		base.Fire();
		
		float timex = Time.time * speed * 0.1365143f;
		float timey = Time.time * speed * 1.21688f;
		float timez = Time.time * speed * 2.5564f;
		
		for(int i=0; i<particles.Length; i++){
			Vector3 position = Vector3.Lerp(muzzlePos.position, muzzlePos.position+muzzlePos.TransformDirection(muzzlePos.position.x,muzzlePos.position.y,range), oneOverZigs * (float)i);
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
			}
		}
	}
}