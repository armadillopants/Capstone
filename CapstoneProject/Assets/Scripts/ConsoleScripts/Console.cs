using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Console : MonoBehaviour {
	
	public class WeaponInfo {
		public string range;
		public string fireRate;
		public string force;
		public string bulletsPerClip;
		public string clips;
		public string reloadSpeed;
		public string damage;
		public string coneAngle;
	}
	
	WeaponInfo info = new WeaponInfo();
	
	public BaseWeapon weapon;
	public List<GameObject> equippedWeapons = new List<GameObject>();
	private WeaponSelection selection;
	GameObject player;
	public bool displayValues = false;
	private Rect screen = new Rect(0, 0, 400, 300);

	void Start(){
		GameObject player = GameObject.Find("Player");
		selection = player.GetComponentInChildren<WeaponSelection>();
		weapon = player.GetComponentInChildren<BaseWeapon>();
	}
	
	void Update(){
		if(!displayValues){
			info.range = weapon.range.ToString();
			info.bulletsPerClip = weapon.bulletsPerClip.ToString();
			info.clips = weapon.clips.ToString();
			info.coneAngle = weapon.coneAngle.ToString();
			info.damage = weapon.damage.ToString();
			info.fireRate = weapon.fireRate.ToString();
			info.force = weapon.force.ToString();
			info.reloadSpeed = weapon.reloadSpeed.ToString();
		}
		equippedWeapons = selection.weaponSlots;
		
		if(Input.GetKeyDown(KeyCode.Return)){
			displayValues = !displayValues;
		}
		
		if(displayValues){
			selection.changingWeapons = false;
			selection.canShoot = false;
		}
		
		if(equippedWeapons[0].activeInHierarchy){
			weapon = equippedWeapons[0].GetComponent<BaseWeapon>();
		} else if(equippedWeapons[1].activeInHierarchy){
			weapon = equippedWeapons[1].GetComponent<BaseWeapon>();
		} else if(equippedWeapons[2].activeInHierarchy){
			weapon = equippedWeapons[2].GetComponent<BaseWeapon>();
		} else if(equippedWeapons[3].activeInHierarchy){
			weapon = equippedWeapons[3].GetComponent<BaseWeapon>();
		}
	}
	
	void OnGUI(){
		if(displayValues){
			GUI.BeginGroup(screen);
			
			GUI.Box(screen, "\n\n\n\n\n\n\n\n\n\n\n\n\n\nWEAPON DATA");
			
			GUILayout.Label("RANGE: ");
			info.range = GUI.TextField(new Rect(150, 0, 100, 20), info.range, 5);
			info.range = Regex.Replace(info.range, @"[^0-9.]", "");
			if(info.range.Length > 0){
				weapon.range = float.Parse(info.range);
			}
			
			GUILayout.Label("FIRE RATE: ");
			info.fireRate = GUI.TextField(new Rect(150, 25, 100, 20), info.fireRate, 5);
			info.fireRate = Regex.Replace(info.fireRate, @"[^0-9.]", "");
			if(info.fireRate.Length > 0){
				weapon.fireRate = float.Parse(info.fireRate);
			}
			
			GUILayout.Label("FORCE: ");
			info.force = GUI.TextField(new Rect(150, 50, 100, 20), info.force, 5);
			info.force = Regex.Replace(info.force, @"[^0-9.]", "");
			if(info.force.Length > 0){
				weapon.force = float.Parse(info.force);
			}
			
			GUILayout.Label("BULLETS PER CLIP: ");
			info.bulletsPerClip = GUI.TextField(new Rect(150, 75, 100, 20), info.bulletsPerClip, 5);
			info.bulletsPerClip = Regex.Replace(info.bulletsPerClip, @"[^0-9]", "");
			if(info.bulletsPerClip.Length > 0){
				weapon.bulletsPerClip = int.Parse(info.bulletsPerClip);
			}
			
			GUILayout.Label("CLIPS: ");
			info.clips = GUI.TextField(new Rect(150, 100, 100, 20), info.clips, 5);
			info.clips = Regex.Replace(info.clips, @"[^0-9]", "");
			if(info.clips.Length > 0){
				weapon.clips = int.Parse(info.clips);
			}
			
			GUILayout.Label("RELOAD SPEED: ");
			info.reloadSpeed = GUI.TextField(new Rect(150, 125, 100, 20), info.reloadSpeed, 5);
			info.reloadSpeed = Regex.Replace(info.reloadSpeed, @"[^0-9.]", "");
			if(info.reloadSpeed.Length > 0){
				weapon.reloadSpeed = float.Parse(info.reloadSpeed);
			}
			
			GUILayout.Label("DAMAGE: ");
			info.damage = GUI.TextField(new Rect(150, 150, 100, 20), info.damage, 5);
			info.damage = Regex.Replace(info.damage, @"[^0-9.]", "");
			if(info.damage.Length > 0){
				weapon.damage = float.Parse(info.damage);
			}
			
			GUILayout.Label("CONE ANGLE: " + weapon.coneAngle.ToString());
			info.coneAngle = GUI.TextField(new Rect(150, 175, 100, 20), info.coneAngle, 5);
			info.coneAngle = Regex.Replace(info.coneAngle, @"[^0-9.]", "");
			if(info.coneAngle.Length > 0){
				weapon.coneAngle = float.Parse(info.coneAngle);
			}
			
			GUI.EndGroup();
		}
	}
}
