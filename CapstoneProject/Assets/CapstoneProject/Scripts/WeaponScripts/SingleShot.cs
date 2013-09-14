using UnityEngine;
using System.Collections;

public class SingleShot : BaseWeapon {
	
	public override void Update(){
		if(Input.GetButtonDown("Fire1") && GameController.Instance.canShoot){
			Fire();
		}
	}
}
