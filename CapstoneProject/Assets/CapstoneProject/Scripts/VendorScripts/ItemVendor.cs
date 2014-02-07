using UnityEngine;
using System.Collections.Generic;

public class ItemVendor : MonoBehaviour {

	public List<GameObject> itemVendor = new List<GameObject>();
	public GameObject upgradeItemVendor;
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(sellItem.GetComponent<Dragable>()){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			GameController.Instance.SetFortificationToSpawn(sellItem.gameObject, 0);
		} else {
			GameObject.FindWithTag(Globals.SHIP).AddComponent<BeginWaveCountdown>();
			GameController.Instance.DeleteResources(sellItem.cost);
		}
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		XMLVendorReader vendorReader = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
		
		if(sellItem.currentUpgrade < 2){
			
			if(sellItem.upgradedItem){
				GameObject upgradedItem = (GameObject)Instantiate(sellItem.upgradedItem, sellItem.gameObject.transform.position, sellItem.gameObject.transform.rotation);
				upgradedItem.name = sellItem.upgradedItem.name;
				upgradedItem.GetComponent<Dragable>().enabled = true;
				upgradedItem.GetComponent<Dragable>().canUpdate = true;
				SellableItem itemToUpgrade = upgradedItem.GetComponent<SellableItem>();
				itemToUpgrade.cost = vendorReader.GetCurrentFortificationCost(itemToUpgrade.cost, itemToUpgrade.itemName, itemToUpgrade.currentUpgrade);
				vendorReader.SetFortData(itemToUpgrade.gameObject);
				vendorReader.UpgradeFortificationData(itemToUpgrade.itemName, itemToUpgrade.currentUpgrade);
				GameController.Instance.DeleteResources(itemToUpgrade.cost);
				UIManager.Instance.uiState = UIManager.UIState.NONE;
				Destroy(sellItem.gameObject);
			} else {
				GameController.Instance.DeleteResources(sellItem.cost);
				vendorReader.SetFortData(sellItem.gameObject);
				vendorReader.UpgradeFortificationData(sellItem.itemName, sellItem.currentUpgrade);
				sellItem.currentUpgrade += 1;
				if(sellItem.currentUpgrade < 2){
					sellItem.cost = vendorReader.GetCurrentFortificationCost(sellItem.cost, sellItem.itemName, sellItem.currentUpgrade);
				}
			}
		}
	}
}

