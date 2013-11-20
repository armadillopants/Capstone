using UnityEngine;
using System.Collections.Generic;

public class ItemVendor : MonoBehaviour {

	public List<GameObject> itemVendor = new List<GameObject>();
	public GameObject upgradeItemVendor;
	
	public Texture2D icon;
	
	private XMLReader reader;
	private XMLVendorReader vendorReader;

	void Start(){
		reader = GameObject.Find("XMLReader").GetComponent<XMLReader>();
		vendorReader = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			GameController.Instance.SetFortificationToSpawn(sellItem.gameObject);
			reader.SetFortData();
			reader.SetFortification(sellItem.name);
			GameController.Instance.DeleteResources(sellItem.cost);
			
			Debug.Log("Purchased: " + sellItem.itemName);
		} else {
			Debug.Log("Not enough funds to purchase: " + sellItem.itemName);
		}
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && sellItem.currentUpgrade <= 4){
			GameController.Instance.DeleteResources(sellItem.cost);
			vendorReader.SetFortData(sellItem.gameObject);
			vendorReader.UpgradeFortificationData(sellItem.id, sellItem.name, sellItem.currentUpgrade);
			if(sellItem.upgradedItem){
				GameObject upgradedItem = (GameObject)Instantiate(sellItem.upgradedItem, sellItem.gameObject.transform.position, sellItem.gameObject.transform.rotation);
				upgradedItem.name = sellItem.upgradedItem.name;
				upgradedItem.GetComponent<Dragable>().enabled = true;
				UIManager.Instance.uiState = UIManager.UIState.NONE;
				Destroy(sellItem.gameObject);
			}
			
			sellItem.currentUpgrade += 1;
			if(sellItem.currentUpgrade <=4){
				sellItem.cost = vendorReader.GetCurrentFortificationCost(sellItem.cost, sellItem.id, sellItem.name, sellItem.currentUpgrade);
			}
			Debug.Log("Purchased upgrade for: " + sellItem.itemName);
		} else if(sellItem.currentUpgrade >= 5){
			Debug.Log("No more upgrades for: " + sellItem.itemName);
		} else {
			Debug.Log("Not enough funds to purchase upgrade for: " + sellItem.itemName);
		}
	}
}

