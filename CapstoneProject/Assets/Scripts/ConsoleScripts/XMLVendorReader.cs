using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLVendorReader : MonoBehaviour {
	
	private BaseWeapon[] weapons;
	public XmlDocument doc = new XmlDocument();
	public XmlNode firstNode;
	
	void Awake(){
		GameObject player = GameObject.FindWithTag("Player");
		weapons = player.GetComponentsInChildren<BaseWeapon>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("VendorData", typeof(TextAsset));
		doc.LoadXml(asset.text);
	}
	
	public int GetCurrentCost(int cost, int i, string itemName, int currentUpgrade){
		string result = itemName.Replace(" " , "");
		firstNode = doc.SelectSingleNode("/VendorData/" + result + "/Upgrades/" + "Upgrade" + currentUpgrade);
		cost = int.Parse(firstNode.Attributes.GetNamedItem("cost").Value);
		return cost;
	}
	
	public void UpgradeData(int i, string itemName, int currentUpgrade){
		string result = itemName.Replace(" " , "");
		if(weapons[i]){
			firstNode = doc.SelectSingleNode("/VendorData/" + result + "/Upgrades/" + "Upgrade" + currentUpgrade);
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