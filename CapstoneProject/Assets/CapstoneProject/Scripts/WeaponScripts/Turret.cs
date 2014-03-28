using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {
	
	public Transform pivot;
	public Transform constraint;
	public float turnSpeed = 3f;
	public bool isTower = false;
	private GameObject target;
	private BaseWeapon weapon;
	private Animation anim;

	void Start(){
		weapon = GetComponentInChildren<BaseWeapon>();
		anim = GetComponentInChildren<Animation>();
		if(anim){
			anim.Play("Spawn");
		}
	}
	
	void Update(){
		target = GameController.Instance.FindNearestTarget(Globals.ENEMY, transform);
		
		if(anim){
			if(GetComponent<Health>().curHealth <= 0){
				anim.Play("Death");
			} else {
				anim.Play("Idle");
			}
		}
		
		if(target && weapon.clips >= 0){
			if(isTower){
				Vector3 relative = pivot.InverseTransformPoint(target.transform.position);
        		float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        		pivot.Rotate(0, angle*Time.deltaTime, 0);
				float xRot = constraint.eulerAngles.x;
				xRot = Mathf.Clamp(xRot, -30, 30);
				constraint.eulerAngles = new Vector3(xRot, constraint.eulerAngles.y, constraint.eulerAngles.z);
				constraint.LookAt(target.transform.position);
			} else {
				Quaternion targetRot = Quaternion.LookRotation(target.transform.position - pivot.position);
				pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRot, turnSpeed*Time.deltaTime);
			}
			
			if(Vector3.Distance(target.transform.position, transform.position) <= weapon.range){
				weapon.Fire();
				weapon.isFiring = true;
			} else {
				weapon.isFiring = false;
			}
		} else {
			if(constraint){
				constraint.eulerAngles = Vector3.Lerp(constraint.eulerAngles, new Vector3(30, constraint.eulerAngles.y, constraint.eulerAngles.z), turnSpeed*Time.deltaTime);
			}
		}
	}
}
