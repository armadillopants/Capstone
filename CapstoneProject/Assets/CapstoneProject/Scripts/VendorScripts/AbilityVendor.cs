using UnityEngine;
using System.Collections.Generic;

public class AbilityVendor : MonoBehaviour {
	
	public List<GameObject> abilityVendor = new List<GameObject>();
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.purchased = true;
			
			Debug.Log("Purchased: " + sellItem.itemName);
		} else {
			Debug.Log("Not enough funds to purchase: " + sellItem.itemName);
		}
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && sellItem.currentUpgrade <= 4){
			GameController.Instance.DeleteResources(sellItem.cost);
			
			sellItem.currentUpgrade += 1;
			
			Debug.Log("Purchased upgrade for: " + sellItem.itemName);
		} else if(sellItem.currentUpgrade >= 5){
			Debug.Log("No more upgrades for: " + sellItem.itemName);
		} else {
			Debug.Log("Not enough funds to purchase upgrade for: " + sellItem.itemName);
		}
	}
}
