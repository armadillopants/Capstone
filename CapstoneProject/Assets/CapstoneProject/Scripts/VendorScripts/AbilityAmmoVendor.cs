using UnityEngine;

public class AbilityAmmoVendor : MonoBehaviour {

	public GameObject ammoVendor;
	public GameObject abilityLink;
	public Texture2D icon;
	private bool isDisplaying = false;
	private DisplayItem ammo;
	private AbilitiesManager curAbility;
	
	private int ammoToBuy;
	
	public void SetWeapon(GameObject ability){
		abilityLink = ability;
		curAbility = abilityLink.transform.parent.GetComponent<AbilitiesManager>();
	}
	
	public void Vendor(){
		if(!isDisplaying){
			ammo = ScriptableObject.CreateInstance<DisplayItem>();
			ammo.item = ammoVendor;
			ammo.sellItem = ammoVendor.GetComponent<SellableItem>();
			
			int curResources = GameController.Instance.GetResources();
			int purchasableAmmo = curAbility.maxAmount - curAbility.amount;
			int ammoToPurchase = 0;
			
			while(ammoToPurchase < purchasableAmmo && curResources-(ammoToPurchase+curAbility.amount)*curAbility.costPerAmount >= 0)
			{
				ammoToPurchase++;
			}
			
			AddPurchasableAmmo(ammoToPurchase);
			
			ammo.sellItem.purchased = false;
			ammo.sellItem.cost = ammoToPurchase * curAbility.costPerAmount;
			
			Debug.Log(ammo.sellItem.cost);
			isDisplaying = true;
		}
	}
	
	void AddPurchasableAmmo(int amount){
		ammoToBuy = amount;
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && !sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.cost = 0;
			curAbility.AddAmount(ammoToBuy);
			sellItem.purchased = true;
			Debug.Log("Purchased: " + sellItem.itemName);
		} else if(sellItem.purchased){
			Debug.Log("Item: " + sellItem.itemName + " was already purchased");
		} else {
			Debug.Log("Not enough funds to purchase: " + sellItem.itemName);
		}
	}
	
	public void Cancel(){
		isDisplaying = false;
	}
}