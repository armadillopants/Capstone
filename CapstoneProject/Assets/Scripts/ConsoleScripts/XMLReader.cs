using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLReader : MonoBehaviour {
	
	private BaseWeapon[] weapons;
	public XmlDocument doc = new XmlDocument();
	public XmlNode firstNode;
	
	void Awake(){
		GameObject player = GameObject.Find("Player");
		weapons = player.GetComponentsInChildren<BaseWeapon>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WeaponData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		
		SetWeapon(0, "/WeaponData/MachineGun");
		SetWeapon(1, "/WeaponData/Pistol");
		SetWeapon(2, "/WeaponData/RocketLauncher");
		SetWeapon(3, "/WeaponData/FlameThrower");
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
