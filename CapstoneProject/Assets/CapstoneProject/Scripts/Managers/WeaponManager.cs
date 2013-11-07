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
	
	private string[] rifleTypes = new string[3]{ "Machine Gun", "Burst Rifle", "Shotgun" };
	private string[] pistolTypes = new string[1]{ "Pistol" };
	private string[] launcherTypes = new string[2]{ "Rocket Launcher", "Missile Launcher" };
	private string[] specialTypes = new string[2]{ "Flame Thrower", "Lightning Blaster" };
	
	public void DetermineWeaponType(SellableItem item){
		for(int i=0; i<rifleTypes.Length; i++){
			if(item.itemName == rifleTypes[i]){
				rifleWeapons.Add(item.gameObject);
			}
		}
		
		for(int i=0; i<pistolTypes.Length; i++){
			if(item.itemName == pistolTypes[i]){
				pistolWeapons.Add(item.gameObject);
			}
		}
		
		for(int i=0; i<launcherTypes.Length; i++){
			if(item.itemName == launcherTypes[i]){
				launcherWeapons.Add(item.gameObject);
			}
		}
		
		for(int i=0; i<specialTypes.Length; i++){
			if(item.itemName == specialTypes[i]){
				specialWeapons.Add(item.gameObject);
			}
		}
	}
}
