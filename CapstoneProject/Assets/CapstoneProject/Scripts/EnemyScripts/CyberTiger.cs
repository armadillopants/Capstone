using UnityEngine;
using System.Collections;

public class CyberTiger : BaseEnemy {
	
	private bool moveTowardsPlayer = true;
	
	protected new void Update(){
		Vector3	adjustedTargetHeight = target.position; // Sets target's height to a variable
		adjustedTargetHeight.y = tr.position.y; // Target's height is always equal to enemy height
			
		if(moveTowardsPlayer){
			tr.rotation = Quaternion.Slerp(tr.rotation, 
				Quaternion.LookRotation(adjustedTargetHeight - tr.position), turningSpeed);
			tr.position += tr.forward * speed * Time.deltaTime;
		}
		
		if(Vector3.Distance(target.position, tr.position) < 15f){
			moveTowardsPlayer = false;
		} else {
			moveTowardsPlayer = true;
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
	}
}
