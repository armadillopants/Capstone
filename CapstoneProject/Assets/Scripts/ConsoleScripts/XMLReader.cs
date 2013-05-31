using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLReader : MonoBehaviour {
	
	private BaseWeapon[] weapons;
	
	void Awake(){
		GameObject player = GameObject.Find("Player");
		weapons = player.GetComponentsInChildren<BaseWeapon>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WeaponData", typeof(TextAsset));
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(asset.text);
		XmlNode firstNode;

		if(weapons[0].name == "Machine Gun"){
			firstNode = doc.SelectSingleNode("/WeaponData/MachineGun");
			weapons[0].range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
			weapons[0].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			weapons[0].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			weapons[0].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			weapons[0].clips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			weapons[0].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			weapons[0].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			weapons[0].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
		}
		if(weapons[1].name == "Pistol"){
			firstNode = doc.SelectSingleNode("/WeaponData/Pistol");
			weapons[1].range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
			weapons[1].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			weapons[1].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			weapons[1].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			weapons[1].clips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			weapons[1].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			weapons[1].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			weapons[1].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
		}
		if(weapons[2].name == "Rocket Launcher"){
			firstNode = doc.SelectSingleNode("/WeaponData/RocketLauncher");
			weapons[2].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			weapons[2].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			weapons[2].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			weapons[2].clips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			weapons[2].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			weapons[2].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			weapons[2].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
		}
		if(weapons[3].name == "Flame Thrower"){
			firstNode = doc.SelectSingleNode("/WeaponData/FlameThrower");
			weapons[3].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			weapons[3].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			weapons[3].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			weapons[3].clips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			weapons[3].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			weapons[3].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			weapons[3].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
		}
	}
}
