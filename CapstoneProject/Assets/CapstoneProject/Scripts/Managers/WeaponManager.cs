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
	private string[] specialTypes = new string[2]{ "FlameThrower", "LightningBlaster" };
	
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
