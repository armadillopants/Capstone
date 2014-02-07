using System.Collections.Generic;
using UnityEngine;

public class AbilityVendor : MonoBehaviour {
	
	public List<GameObject> abilityVendor = new List<GameObject>();
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		GameController.Instance.DeleteResources(sellItem.cost);
		sellItem.purchased = true;
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(sellItem.currentUpgrade <= 2){
			GameController.Instance.DeleteResources(sellItem.cost);
			
			sellItem.currentUpgrade += 1;
			
			Debug.Log("Purchased upgrade for: " + sellItem.itemName);
		}
	}
}
