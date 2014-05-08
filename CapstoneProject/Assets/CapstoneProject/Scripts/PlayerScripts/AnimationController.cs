using UnityEngine;

public class AnimationController : MonoBehaviour {
	
	private Animator animator;
	private BaseWeapon weapon;
	private LocalInput input;
	private WeaponSelection selection;
	
	public static int VelZ = Animator.StringToHash("VelZ");
	public static int VelX = Animator.StringToHash("VelX");
	public static int Grip = Animator.StringToHash("Grip");
	public static int Dead = Animator.StringToHash("Dead");
	public static int HolsterWeapon = Animator.StringToHash("HolsterWeapon");
	public static int DrawWeapon = Animator.StringToHash("DrawWeapon");
	public static int ReloadWeapon = Animator.StringToHash("Reloading");

	void Start(){
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
				animator.SetBool(ReloadWeapon, weapon.isReloading);
			}
			
			if(GameController.Instance.GetPlayerHealth().IsDead){
				animator.SetBool(Dead, GameController.Instance.GetPlayerHealth().IsDead);
			} else {
				animator.SetFloat(VelZ, input.controller.moveDirection.z);
				animator.SetFloat(VelX, input.controller.moveDirection.x);
			}
		}
	}
}
