﻿using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {
	
	public Transform pivot;
	public float turnSpeed = 3f;
	public bool isTower = false;
	private GameObject target;
	private BaseWeapon weapon;

	void Start(){
		weapon = GetComponentInChildren<BaseWeapon>();
	}
	
	void Update(){
		target = GameController.Instance.FindNearestTarget(Globals.ENEMY, this.pivot);
		
		if(target && weapon.clips > 0){
			if(isTower){
				Vector3 relative = transform.InverseTransformPoint(target.transform.position);
        		float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        		transform.Rotate(0, angle*Time.deltaTime, 0);
				/*pivot.LookAt(target.transform.position);
				float xRot = pivot.eulerAngles.x;
				xRot = Mathf.Clamp(xRot, -45, 45);
				pivot.eulerAngles = new Vector3(xRot, pivot.eulerAngles.y, pivot.eulerAngles.z);*/
			} else {
				Quaternion targetRot = Quaternion.LookRotation(target.transform.position - pivot.position);
				pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRot, turnSpeed*Time.deltaTime);
			}
			
			if(Vector3.Distance(target.transform.position, transform.position) < weapon.range){
				weapon.Fire();
				weapon.isFiring = true;
			} else {
				weapon.isFiring = false;
			}
		} else {
			//pivot.eulerAngles = Vector3.Lerp(pivot.eulerAngles, new Vector3(30, transform.eulerAngles.y, transform.eulerAngles.z), turnSpeed*Time.deltaTime);
		}
	}
}
