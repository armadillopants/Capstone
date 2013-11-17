using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLVendorReader : MonoBehaviour {
	
	private BaseWeapon[] weapons;
	private FortificationData fortData;
	public XmlDocument doc = new XmlDocument();
	public XmlNode firstNode;
	
	void Awake(){
		GameObject player = GameObject.FindWithTag(Globals.PLAYER);
		weapons = player.GetComponentsInChildren<BaseWeapon>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("VendorData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		//doc.Load(Application.dataPath + "/VendorData.xml");
	}
	
	public int GetCurrentWeaponCost(int cost, int i, string itemName, int currentUpgrade){
		string result = itemName.Replace(" " , "");
		firstNode = doc.SelectSingleNode("/VendorData/Weapons/" + result + "/Upgrades/" + "Upgrade" + currentUpgrade);
		cost = int.Parse(firstNode.Attributes.GetNamedItem("cost").Value);
		return cost;
	}
	
	public void UpgradeWeaponData(int i, string itemName, int currentUpgrade){
		string result = itemName.Replace(" " , "");
		if(weapons[i]){
			firstNode = doc.SelectSingleNode("/VendorData/Weapons/" + result + "/Upgrades/" + "Upgrade" + currentUpgrade);
			weapons[i].range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
			weapons[i].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			weapons[i].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			weapons[i].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			weapons[i].maxClips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			weapons[i].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			weapons[i].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			weapons[i].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
			weapons[i].Replenish();
		}
	}
	
	public int GetCurrentFortificationCost(int cost, int i, string itemName, int currentUpgrade){
		string result = itemName.Replace(" " , "");
		firstNode = doc.SelectSingleNode("/VendorData/Fortifications/" + result + "/Upgrades/" + "Upgrade" + currentUpgrade);
		cost = int.Parse(firstNode.Attributes.GetNamedItem("cost").Value);
		return cost;
	}
	
	public void UpgradeFortificationData(int i, string itemName, int currentUpgrade){
		string result = itemName.Replace(" " , "");
		if(fortData){
			firstNode = doc.SelectSingleNode("/VendorData/Fortifications/" + result + "/Upgrades/" + "Upgrade" + currentUpgrade);
			fortData.health.ModifyHealth(float.Parse(firstNode.Attributes.GetNamedItem("health").Value));
			fortData.fortDamage = float.Parse(firstNode.Attributes.GetNamedItem("fortDamage").Value);
			if(fortData.GetComponentInChildren<BaseWeapon>() != null){
				BaseWeapon weapon = fortData.GetComponentInChildren<BaseWeapon>();
				weapon.range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
				weapon.fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
				weapon.force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
				weapon.bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
				weapon.maxClips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
				weapon.reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
				weapon.damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
				weapon.coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
				weapon.Replenish();
			}
		}
	}
	
	public void SetFortData(GameObject item){
		fortData = item.GetComponent<FortificationData>();
	}
}