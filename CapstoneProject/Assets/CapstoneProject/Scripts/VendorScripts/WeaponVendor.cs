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
		
		if(!sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.purchased = true;
			manager.DetermineWeaponType(sellItem);
			Debug.Log("Purchased: " + sellItem.itemName);
		}
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		GameController.Instance.DeleteResources(sellItem.cost);
		reader.UpgradeWeaponData(sellItem.id, sellItem.name, sellItem.currentUpgrade);
			
		sellItem.currentUpgrade += 1;
		if(sellItem.currentUpgrade < 2){
			sellItem.cost = reader.GetCurrentWeaponCost(sellItem.cost, sellItem.id, sellItem.name, sellItem.currentUpgrade);
		}
		Debug.Log("Purchased upgrade for: " + sellItem.itemName);
	}
}