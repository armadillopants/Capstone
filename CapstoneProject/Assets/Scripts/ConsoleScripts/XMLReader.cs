using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLReader : MonoBehaviour {
	
	private BaseWeapon[] weapons;
	private WeaponManager manager;
	public XmlDocument doc = new XmlDocument();
	public XmlNode firstNode;
	
	void Awake(){
		GameObject player = GameObject.FindWithTag("Player");
		weapons = player.GetComponentsInChildren<BaseWeapon>();
		manager = player.GetComponentInChildren<WeaponManager>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WeaponData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		
		SetWeapon(manager.allWeapons[0].id, "/WeaponData/MachineGun");
		SetWeapon(manager.allWeapons[1].id, "/WeaponData/Pistol");
		SetWeapon(manager.allWeapons[2].id, "/WeaponData/RocketLauncher");
		SetWeapon(manager.allWeapons[3].id, "/WeaponData/FlameThrower");
		SetWeapon(manager.allWeapons[4].id, "/WeaponData/ThreeBurst");
		SetWeapon(manager.allWeapons[5].id, "/WeaponData/Shotgun");
		SetWeapon(manager.allWeapons[6].id, "/WeaponData/MissileLauncher");
	}
	
	void SetWeapon(int i, string path){
		if(weapons[i]){
			firstNode = doc.SelectSingleNode(path);
			weapons[i].range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
			weapons[i].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			weapons[i].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			weapons[i].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			weapons[i].clips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			weapons[i].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			weapons[i].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			weapons[i].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
		}
	}
}
