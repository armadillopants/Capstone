using UnityEngine;
using System.Collections;

public class CyberTiger : BaseEnemy {
	
	private bool moveTowardsPlayer = true;
	
	public override void Update(){
		Vector3	adjustedTargetHeight = target.position; // Sets target's height to a variable
		adjustedTargetHeight.y = trans.position.y; // Target's height is always equal to enemy height
			
		if(moveTowardsPlayer){
			trans.rotation = Quaternion.Slerp(trans.rotation, 
				Quaternion.LookRotation(adjustedTargetHeight - trans.position), turnSpeed);
			trans.position += trans.forward * moveSpeed * Time.deltaTime;
		}
		
		if(Vector3.Distance(target.position, trans.position) < 15f){
			moveTowardsPlayer = false;
		} else {
			moveTowardsPlayer = true;
		}
		
		if(currentCoolDown > 0){
			currentCoolDown -= Time.deltaTime;
		}
	}
}
