using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {
	
	public Transform pivot;
	public float turnSpeed = 3f;
	private GameObject target;
	private BaseWeapon weapon;

	void Start(){
		weapon = GetComponentInChildren<BaseWeapon>();
	}
	
	void Update(){
		target = GameController.Instance.FindNearestTarget(Globals.ENEMY, this.pivot);
		
		if(target && weapon.clips > 0){
			Quaternion rotate = Quaternion.LookRotation(target.transform.position - pivot.position);
			pivot.rotation = Quaternion.Slerp(pivot.rotation, rotate, turnSpeed*Time.deltaTime);
			
			if(Vector3.Distance(target.transform.position, transform.position) < weapon.range){
				weapon.Fire();
				weapon.isFiring = true;
			} else {
				weapon.isFiring = false;
			}
		} else {
			pivot.eulerAngles = Vector3.Lerp(pivot.eulerAngles, new Vector3(30, transform.eulerAngles.y, transform.eulerAngles.z), turnSpeed*Time.deltaTime);
		}
	}
}
