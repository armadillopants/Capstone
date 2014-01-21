using UnityEngine;

public class WeaponVendor : MonoBehaviour {
	
	private XMLVendorReader vendorReader;

	void Start(){
		vendorReader = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(!sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.purchased = true;
			GameObject.FindWithTag(Globals.PLAYER).GetComponentInChildren<WeaponManager>().DetermineWeaponType(sellItem);
			Debug.Log("Purchased: " + sellItem.itemName);
			sellItem.cost = vendorReader.GetCurrentWeaponCost(sellItem.cost, sellItem.id, sellItem.name, sellItem.currentUpgrade);
		}
	}
	
	public void Upgrade(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		GameController.Instance.DeleteResources(sellItem.cost);
		vendorReader.UpgradeWeaponData(sellItem.id, sellItem.name, sellItem.currentUpgrade);
			
		sellItem.currentUpgrade += 1;
		if(sellItem.currentUpgrade <= 2){
			sellItem.cost = vendorReader.GetCurrentWeaponCost(sellItem.cost, sellItem.id, sellItem.name, sellItem.currentUpgrade);
		}
		Debug.Log("Purchased upgrade for: " + sellItem.itemName);
	}
}