using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLReader : MonoBehaviour {
	
	private WeaponManager manager;
	public XmlDocument doc = new XmlDocument();
	public XmlNode firstNode;
	private FortificationData fortData;
	
	void Awake(){
		GameObject player = GameObject.FindWithTag(Globals.PLAYER);
		manager = player.GetComponentInChildren<WeaponManager>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WeaponData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		//doc.Load(Application.dataPath + "/WeaponData.xml");
		
		for(int i=0; i<manager.allWeapons.Count; i++){
			SetWeapon(manager.allWeapons[i].id, "/BaseValueData/WeaponData/"+manager.allWeapons[i].name);
		}
		
		ItemVendor vendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
		
		for(int i=0; i<vendor.itemVendor.Count; i++){
			fortData = vendor.itemVendor[i].GetComponent<FortificationData>();
			SetFortification(vendor.itemVendor[i].GetComponent<SellableItem>().itemName);
		}
	}
	
	public void Reset(){
		for(int i=0; i<manager.allWeapons.Count; i++){
			SetWeapon(manager.allWeapons[i].id, "/BaseValueData/WeaponData/"+manager.allWeapons[i].name);
		}
	}
	
	void SetWeapon(int i, string path){
		if(manager.allWeapons[i]){
			firstNode = doc.SelectSingleNode(path);
			manager.allWeapons[i].range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
			manager.allWeapons[i].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			manager.allWeapons[i].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			manager.allWeapons[i].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			manager.allWeapons[i].maxClips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			manager.allWeapons[i].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			manager.allWeapons[i].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			manager.allWeapons[i].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
			manager.allWeapons[i].costPerBullet = int.Parse(firstNode.Attributes.GetNamedItem("costPerBullet").Value);
			manager.allWeapons[i].GetComponent<SellableItem>().cost = int.Parse(firstNode.Attributes.GetNamedItem("cost").Value);
			manager.allWeapons[i].Replenish();
		}
	}
	
	public void SetFortification(string fort){
		string path = "/BaseValueData/FortificationData/" + fort.Replace(" " , "");
		if(fortData){
			firstNode = doc.SelectSingleNode(path);
			fortData.health.ModifyHealth(float.Parse(firstNode.Attributes.GetNamedItem("health").Value));
			fortData.fortDamage = float.Parse(firstNode.Attributes.GetNamedItem("fortDamage").Value);
			fortData.GetComponent<SellableItem>().cost = int.Parse(firstNode.Attributes.GetNamedItem("cost").Value);
			if(fortData.weapon != null){
				fortData.weapon.range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
				fortData.weapon.fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
				fortData.weapon.force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
				fortData.weapon.bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
				fortData.weapon.maxClips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
				fortData.weapon.reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
				fortData.weapon.damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
				fortData.weapon.coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
				fortData.weapon.Replenish();
			}
		}
	}
	
	public void SetFortData(){
		fortData = GameController.Instance.current.GetComponent<FortificationData>();
	}
}
