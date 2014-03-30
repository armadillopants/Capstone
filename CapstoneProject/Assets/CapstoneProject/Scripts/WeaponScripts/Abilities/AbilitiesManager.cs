using UnityEngine;
using System.Collections;
using System.Xml;

public class AbilitiesManager : MonoBehaviour {
	
	private GameObject holder;
	public bool beginAbility;
	
	public Ability orbitAbility = new Ability();
	public Ability rockRainAbility = new Ability();
	public Ability strikerAbility = new Ability();
	private XmlDocument doc = new XmlDocument();
	
	#region Singleton
	
	private static AbilitiesManager _instance;
	
	public static AbilitiesManager Instance {
		get { return _instance; }
	}
	
	void Awake(){
		if(AbilitiesManager.Instance != null){
			DestroyImmediate(gameObject);
			return;
		}
		_instance = this;
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("AbilityData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		
		Initialize();
	}
	
	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion
	
	void Initialize(){
		ResetValues(orbitAbility, "/AbilityData/Values/OrbitAbility/Upgrades/Upgrade"+
			GameObject.Find("Vendor").GetComponent<AbilityVendor>().abilityVendor[0].GetComponent<SellableItem>().currentUpgrade);
		
		ResetValues(rockRainAbility, "/AbilityData/Values/RockRainAbility/Upgrades/Upgrade"+
			GameObject.Find("Vendor").GetComponent<AbilityVendor>().abilityVendor[1].GetComponent<SellableItem>().currentUpgrade);
		
		ResetValues(strikerAbility, "/AbilityData/Values/StrikerAbility/Upgrades/Upgrade"+
			GameObject.Find("Vendor").GetComponent<AbilityVendor>().abilityVendor[2].GetComponent<SellableItem>().currentUpgrade);
	}
	
	public void ResetValues(Ability ability, string path){
		XmlNode firstNode = doc.SelectSingleNode(path);
		ability.maxAmount = int.Parse(firstNode.Attributes.GetNamedItem("amount").Value);
		ability.amount = ability.maxAmount;
		ability.damage = float.Parse(firstNode.Attributes.GetNamedItem("damage").Value);
		ability.maxCoolDown = float.Parse(firstNode.Attributes.GetNamedItem("cooldown").Value);
		ability.coolDown = ability.maxCoolDown;
		ability.costPerAmount = int.Parse(firstNode.Attributes.GetNamedItem("costPerAmount").Value);
		ability.cost = int.Parse(firstNode.Attributes.GetNamedItem("cost").Value);
	}
	
	public int GetCurrentAbilityCost(int cost, string itemName, int currentUpgrade){
		string result = itemName.Replace(" " , "");
		XmlNode firstNode = doc.SelectSingleNode("/AbilityData/Values/" + result + "/Upgrades/Upgrade" + currentUpgrade);
		cost = int.Parse(firstNode.Attributes.GetNamedItem("cost").Value);
		return cost;
	}
	
	void Start(){
		holder = GameObject.Find("AbilityHolder");
		beginAbility = false;
	}
	
	void Update(){
		if(GameController.Instance.canShoot){
			if(Input.GetKeyDown(KeyCode.E) && beginAbility && rockRainAbility.amount > 0){
				if(holder.transform.GetComponent<OrbitAbility>() != null){
					holder.SendMessage("BeginAbility", SendMessageOptions.DontRequireReceiver);
					beginAbility = false;
				} else if(holder.transform.GetComponent<RockRainAbility>() != null){
					holder.SendMessage("BeginAbility", SendMessageOptions.DontRequireReceiver);
					beginAbility = false;
				} else if(holder.transform.GetComponent<StrikerAbility>() != null){
					holder.SendMessage("BeginAbility", SendMessageOptions.DontRequireReceiver);
					beginAbility = false;
				}
			}
		}
	}
	
	public void AddAmount(Ability ability, int howMuch){
	 	ability.maxAmount = howMuch;
		ability.amount = ability.maxAmount;
	}
	
	public void SetCoolDown(Ability ability){
		ability.coolDown = ability.maxCoolDown;
	}
}
