using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
	
	private Animator animator;
	private BaseWeapon weapon;
	private LocalInput input;
	private WeaponSelection selection;
	
	private Transform trans;
	
	public static int VelZ = Animator.StringToHash("VelZ");
	public static int VelX = Animator.StringToHash("VelX");
	public static int Grip = Animator.StringToHash("Grip");
	public static int Dead = Animator.StringToHash("Dead");
	public static int HolsterWeapon = Animator.StringToHash("HolsterWeapon");
	public static int DrawWeapon = Animator.StringToHash("DrawWeapon");
	
	public Transform lowerBody;
	public Transform upperBody;
	
	private Vector3 lowerBodyForward = Vector3.forward;
	private Vector3 lowerBodyForwardTarget = Vector3.forward;
	private float lowerBodyDeltaAngle = 0;

	void Start(){
		trans = rigidbody.transform;
		
		upperBody = GameObject.Find("Spine1").transform;
		lowerBody = GameObject.Find("Hips").transform;
		
		animator = GetComponent<Animator>();
		weapon = GetComponentInChildren<BaseWeapon>();
		input = GetComponent<LocalInput>();
		selection = GetComponentInChildren<WeaponSelection>();
		
		if(animator.layerCount >= 2){
			animator.SetLayerWeight(1, 1);
		}
	}
	
	void Update(){
		if(animator){
			if(animator.layerCount >= 2){
				weapon = GetComponentInChildren<BaseWeapon>();
				animator.SetInteger(Grip, weapon.gripID);
				animator.SetBool(HolsterWeapon, selection.changingWeapons);
				animator.SetBool(DrawWeapon, selection.drawWeapon);
			}
			
			if(GameController.Instance.GetPlayerHealth().IsDead){
				animator.SetBool(Dead, GameController.Instance.GetPlayerHealth().IsDead);
			} else {
				animator.SetFloat(VelZ, input.controller.moveDirection.z);
				animator.SetFloat(VelX, input.controller.moveDirection.x);
			}
		}
	}
	
	/*void FixedUpdate(){
		// Turn lower body towards its target direction
		lowerBodyForward = Vector3.RotateTowards(lowerBodyForward, lowerBodyForwardTarget, Time.deltaTime * 520 * Mathf.Deg2Rad, 1);
		
		// Calculate delta angle to make the lower body stay in place
		lowerBodyDeltaAngle = Mathf.DeltaAngle(HorizontalAngle(trans.forward), HorizontalAngle(lowerBodyForward));
		
		// If the body is twisted more than 80 degrees, set a new target dir for lower body, so it begins turning
		if(Mathf.Abs(lowerBodyDeltaAngle) > 80){
			lowerBodyForwardTarget = trans.forward;
		}
		
		// Create a Quaternion rotation from the rotation angle
		Quaternion lowerBodyDeltaRotation = Quaternion.Euler(0, lowerBodyDeltaAngle, 0);
		
		// Rotate the whole body by the angle
		upperBody.rotation = lowerBodyDeltaRotation * upperBody.rotation;
		
		// Counter-rotate the upper body so it wont be affected
		//upperBody.rotation = Quaternion.Inverse(lowerBodyDeltaRotation) * upperBody.rotation;
	}*/
	
	private float HorizontalAngle(Vector3 dir){
		return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
	}
}
