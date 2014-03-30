using System.Collections.Generic;
using UnityEngine;

public class AbilityVendor : MonoBehaviour {
	
	public List<GameObject> abilityVendor = new List<GameObject>();
	
	void Start(){
		abilityVendor[0].GetComponent<SellableItem>().cost = AbilitiesManager.Instance.orbitAbility.cost;
		abilityVendor[1].GetComponent<SellableItem>().cost = AbilitiesManager.Instance.rockRainAbility.cost;
		abilityVendor[2].GetComponent<SellableItem>().cost = AbilitiesManager.Instance.strikerAbility.cost;
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		GameController.Instance.DeleteResources(sellItem.cost);
		sellItem.purchased = true;
		sellItem.currentUpgrade += 1;
		sellItem.cost = AbilitiesManager.Instance.GetCurrentAbilityCost(sellItem.cost, sellItem.itemName, sellItem.currentUpgrade);
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(sellItem.currentUpgrade <= 2){
			GameController.Instance.DeleteResources(sellItem.cost);
			
			switch(sellItem.itemName){
			case "Orbit Ability":
				AbilitiesManager.Instance.ResetValues(AbilitiesManager.Instance.orbitAbility, "/AbilityData/Values/OrbitAbility/Upgrades/Upgrade"+
					abilityVendor[0].GetComponent<SellableItem>().currentUpgrade);
				break;
			case "RockRain Ability":
				AbilitiesManager.Instance.ResetValues(AbilitiesManager.Instance.rockRainAbility, "/AbilityData/Values/RockRainAbility/Upgrades/Upgrade"+
					abilityVendor[1].GetComponent<SellableItem>().currentUpgrade);
				break;
			case "StrikerAbility":
				AbilitiesManager.Instance.ResetValues(AbilitiesManager.Instance.strikerAbility, "/AbilityData/Values/StrikerAbility/Upgrades/Upgrade"+
					abilityVendor[2].GetComponent<SellableItem>().currentUpgrade);
				break;
			}
			
			sellItem.currentUpgrade += 1;
			if(sellItem.currentUpgrade <= 2){
				sellItem.cost = AbilitiesManager.Instance.GetCurrentAbilityCost(sellItem.cost, sellItem.itemName, sellItem.currentUpgrade);
			}
			
			Debug.Log("Purchased upgrade for: " + sellItem.itemName);
		}
	}
}
