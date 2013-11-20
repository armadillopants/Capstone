using UnityEngine;

public class WeaponVendor : MonoBehaviour {
	
	private XMLVendorReader reader;
	private WeaponManager manager;

	void Start(){
		reader = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
		manager = GameObject.FindWithTag(Globals.PLAYER).GetComponentInChildren<WeaponManager>();
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && !sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.purchased = true;
			manager.DetermineWeaponType(sellItem);
			Debug.Log("Purchased: " + sellItem.itemName);
		} else if(sellItem.purchased){
			Debug.Log("Item: " + sellItem.itemName + " was already purchased");
		} else {
			Debug.Log("Not enough funds to purchase: " + sellItem.itemName);
		}
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && sellItem.currentUpgrade <= 4){
			GameController.Instance.DeleteResources(sellItem.cost);
			reader.UpgradeWeaponData(sellItem.id, sellItem.name, sellItem.currentUpgrade);
			
			sellItem.currentUpgrade += 1;
			if(sellItem.currentUpgrade <=4){
				sellItem.cost = reader.GetCurrentWeaponCost(sellItem.cost, sellItem.id, sellItem.name, sellItem.currentUpgrade);
			}
			Debug.Log("Purchased upgrade for: " + sellItem.itemName);
		} else if(sellItem.currentUpgrade >= 5){
			Debug.Log("No more upgrades for: " + sellItem.itemName);
		} else {
			Debug.Log("Not enough funds to purchase upgrade for: " + sellItem.itemName);
		}
	}
}