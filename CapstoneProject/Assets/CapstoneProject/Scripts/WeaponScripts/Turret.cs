using UnityEngine;

public class Turret : MonoBehaviour {
	
	public Transform pivot;
	public Transform constraint;
	public float turnSpeed = 3f;
	public bool isTower = false;
	private BaseWeapon weapon;
	private Animation anim;
	private Health health;

	void Start(){
		weapon = GetComponentInChildren<BaseWeapon>();
		anim = GetComponentInChildren<Animation>();
		health = GetComponent<Health>();
		if(anim){
			anim.Play("Spawn");
		}
	}
	
	void Update(){
		GameObject target = GameController.Instance.FindNearestTarget(Globals.ENEMY, transform);
		
		if(anim){
			if(health.curHealth <= 0){
				anim.Play("Death");
			} else {
				anim.Play("Idle");
			}
		}
		
		if(target){
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
				if(weapon.projectile.name == "Rocket"){
					GameObject projectile = ObjectPool.GetCachedObject(weapon.projectile);
					projectile.GetComponent<Projectile>().weapon = weapon;
					projectile.GetComponent<Projectile>().weapon.damage = weapon.damage;
					projectile.GetComponent<Projectile>().weapon.force = weapon.force;
				}
			} else {
				weapon.isFiring = false;
			}
		} else {
			weapon.isFiring = false;
		}
	}
}
