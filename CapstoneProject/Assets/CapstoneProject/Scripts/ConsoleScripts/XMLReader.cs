using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLReader : MonoBehaviour {
	
	private BaseWeapon[] weapons;
	private WeaponManager manager;
	public XmlDocument doc = new XmlDocument();
	public XmlNode firstNode;
	private FortificationData fortData;
	
	void Awake(){
		GameObject player = GameObject.FindWithTag(Globals.PLAYER);
		weapons = player.GetComponentsInChildren<BaseWeapon>();
		manager = player.GetComponentInChildren<WeaponManager>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WeaponData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		//doc.Load(Application.dataPath + "/WeaponData.xml");
		
		for(int i=0; i<manager.allWeapons.Count; i++){
			SetWeapon(manager.allWeapons[i].id, "/BaseValueData/WeaponData/"+manager.allWeapons[i].name.Replace(" " , ""));
		}
		
		/*SetWeapon(manager.allWeapons[0].id, "/WeaponData/MachineGun");
		SetWeapon(manager.allWeapons[1].id, "/WeaponData/Pistol");
		SetWeapon(manager.allWeapons[2].id, "/WeaponData/RocketLauncher");
		SetWeapon(manager.allWeapons[3].id, "/WeaponData/FlameThrower");
		SetWeapon(manager.allWeapons[4].id, "/WeaponData/BurstRifle");
		SetWeapon(manager.allWeapons[5].id, "/WeaponData/Shotgun");
		SetWeapon(manager.allWeapons[6].id, "/WeaponData/MissileLauncher");
		SetWeapon(manager.allWeapons[7].id, "/WeaponData/LightningBlaster");*/
	}
	
	public void Reset(){
		for(int i=0; i<manager.allWeapons.Count; i++){
			SetWeapon(manager.allWeapons[i].id, "/BaseValueData/WeaponData/"+manager.allWeapons[i].name.Replace(" " , ""));
		}
	}
	
	void SetWeapon(int i, string path){
		if(weapons[i]){
			firstNode = doc.SelectSingleNode(path);
			weapons[i].range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
			weapons[i].fireRate = float.Parse(firstNode.Attributes.GetNamedItem("fireRate").Value);
			weapons[i].force = float.Parse(firstNode.Attributes.GetNamedItem("force").Value);
			weapons[i].bulletsPerClip = int.Parse(firstNode.Attributes.GetNamedItem("bulletsPerClip").Value);
			weapons[i].maxClips = int.Parse(firstNode.Attributes.GetNamedItem("clips").Value);
			weapons[i].reloadSpeed = float.Parse(firstNode.Attributes.GetNamedItem("reloadSpeed").Value);
			weapons[i].damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
			weapons[i].coneAngle = float.Parse(firstNode.Attributes.GetNamedItem("coneAngle").Value);
			weapons[i].costPerBullet = int.Parse(firstNode.Attributes.GetNamedItem("costPerBullet").Value);
			weapons[i].Replenish();
		}
	}
	
	public void SetFortification(string fort){
		string path = "/BaseValueData/FortificationData/" + fort.Replace(" " , "");
		if(fortData){
			firstNode = doc.SelectSingleNode(path);
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
	
	public void SetFortData(){
		fortData = GameController.Instance.current.GetComponent<FortificationData>();
	}
}
