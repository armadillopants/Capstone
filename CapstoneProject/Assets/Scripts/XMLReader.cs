using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLReader : MonoBehaviour {
	
	private BaseWeapon[] weapons;
	
	void Awake(){
		GameObject player = GameObject.Find("Player");
		weapons = player.GetComponentsInChildren<BaseWeapon>();
	}

	void Start(){
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WeaponData", typeof(TextAsset));
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(asset.text);
		XmlNode firstNode = doc.FirstChild;

		if(weapons[0].name == "Machine Gun"){
			//weapons[0].range = float.Parse(firstNode.Attributes.GetNamedItem("range").Value);
		}
		if(weapons[1].name == "Pistol"){
			
		}
		if(weapons[2].name == "Rocket Launcher"){
			
		}
		if(weapons[3].name == "Flame Thrower"){
			
		}
	}
	
	void Update(){
	
	}
}
