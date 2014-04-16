using UnityEngine;
using System.Collections;

public class RoboTiger : Enemy {
	
	public Vector3 targetPointOne;
	public Vector3 targetPointTwo;
	private float spawningTime = Random.Range(30f, 60f);
	private int amountToSpawn = Random.Range(1, 5);
	public bool runToPointOne = true;
	public bool runToPointTwo = false;
	public GameObject cyberCat;
	public GameObject beamDown;
	public AudioClip tigerGrowl;
	
	public override void Start(){
		base.Start();
		
		targetPointOne = new Vector3(tr.position.x, tr.position.y, target.position.z+Random.Range(-10, 10));
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
				Instantiate(beamDown, pos, Quaternion.identity);
				GameObject cat = (GameObject)Instantiate(cyberCat, pos, Quaternion.identity);
				cat.name = cyberCat.name;
			}
			spawningTime = Random.Range(30f, 60f);
			amountToSpawn = Random.Range(1, 5);
			audio.PlayOneShot(tigerGrowl);
		}
	}
	
	/*public override void ChaseObject(){
		
		Vector3 velocity;
		Vector3 rotateDir = new Vector3();
		if(canMove && target != null){
			
			if(Vector3.Distance(target.position, tr.position) < distance){
				if(runToPointOne){
					rotateDir = targetPointOne;
					if(Vector3.Distance(targetPointOne, tr.position) < 3f){
						targetPointTwo = new Vector3(target.position.x+Random.Range(-10, 10), tr.position.y, tr.position.z);
						runToPointTwo = true;
						runToPointOne = false;
					}
				}
				if(runToPointTwo){
					rotateDir = targetPointTwo;
					if(Vector3.Distance(targetPointOne, tr.position) < 3f){
						targetPointOne = new Vector3(tr.position.x, tr.position.y, target.position.z+Random.Range(-10, 10));
						runToPointOne = true;
						runToPointTwo = false;
					}
				}
				RotateTowards(rotateDir);
			} else {
				if(targetDirection != Vector3.zero){
					RotateTowards(targetDirection);
				}
			}
			
			Vector3 dir = CalculateVelocity(GetFeetPosition());
			
			Vector3 adjustedTargetHeight = tr.position; // Set position to variable
			adjustedTargetHeight.y = targetHeight; // Adjust height to a set target
			tr.position = adjustedTargetHeight; // Commit the changes
			
			if(dir.sqrMagnitude > sleepVelocity*sleepVelocity){
				// Move the enemy
				if(rigid != null){
					rigid.velocity = dir * speed;
				}
			} else {
				dir = Vector3.zero;
			}
			
			velocity = rigid.velocity;
		} else {
			velocity = Vector3.zero;
		}
		
		if(anim && canMove){
			Vector3 relativeVelocity = tr.InverseTransformDirection(velocity);
			if(velocity.sqrMagnitude <= sleepVelocity*sleepVelocity){
				// Fade out walk animation
				anim.Blend("Walk", 0, 0.2f);
			} else {
				// Fade in walking animation
				//string curAnim = anim.clip.name;
				//anim.Blend(curAnim, 0, 0.2f);
				//anim.Blend("Walk", 1, 0.2f);
				anim.CrossFade("Walk", 0.2f);
				
				AnimationState state = anim["Walk"];
				
				float relSpeed = relativeVelocity.z;
				state.speed = relSpeed*animationSpeed;
			}
		}
	}*/
}
