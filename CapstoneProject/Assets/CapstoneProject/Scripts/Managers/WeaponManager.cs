using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {
	
	public List<BaseWeapon> allWeapons = new List<BaseWeapon>(); // All weapons in the game
	
	public List<GameObject> equippedWeapons = new List<GameObject>(); // The four weapons currently equipped
	
	// These represent the four different weapon types
	public List<GameObject> rifleWeapons = new List<GameObject>();
	public List<GameObject> pistolWeapons = new List<GameObject>();
	public List<GameObject> launcherWeapons = new List<GameObject>();
	public List<GameObject> specialWeapons = new List<GameObject>();
	
	private string[] rifleTypes = new string[3]{ "MachineGun", "BurstRifle", "Shotgun" };
	private string[] pistolTypes = new string[3]{ "Pistol", "Revolver", "SMG" };
	private string[] launcherTypes = new string[3]{ "RocketLauncher", "RayBlaster", "GrenadeLauncher" };
	private string[] specialTypes = new string[3]{ "FlameThrower", "LightningBlaster", "ThunderGun" };
	
	void Awake(){
		Reset();
	}
	
	public void Reset(){
		equippedWeapons[0] = null;
		equippedWeapons[1] = null;
		equippedWeapons[2] = null;
		equippedWeapons[3] = null;
		rifleWeapons.Clear();
		pistolWeapons.Clear();
		launcherWeapons.Clear();
		specialWeapons.Clear();
		equippedWeapons[1] = GameObject.Find(rifleTypes[0]);
		equippedWeapons[0] = GameObject.Find(pistolTypes[0]);
		rifleWeapons.Add(GameObject.Find(rifleTypes[0]));
		pistolWeapons.Add(GameObject.Find(pistolTypes[0]));
	}
	
	public void DetermineWeaponType(SellableItem item){
		for(int i=0; i<rifleTypes.Length; i++){
			if(item.name == rifleTypes[i]){
				rifleWeapons.Add(item.gameObject);
			}
		}
		
		for(int i=0; i<pistolTypes.Length; i++){
			if(item.name == pistolTypes[i]){
				pistolWeapons.Add(item.gameObject);
			}
		}
		
		for(int i=0; i<launcherTypes.Length; i++){
			if(item.name == launcherTypes[i]){
				launcherWeapons.Add(item.gameObject);
			}
		}
		
		for(int i=0; i<specialTypes.Length; i++){
			if(item.name == specialTypes[i]){
				specialWeapons.Add(item.gameObject);
			}
		}
	}
	
	public WeaponType GetWeaponType(BaseWeapon weapon){
		return weapon.weaponType;
	}
}
