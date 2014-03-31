using UnityEngine;

public class AbilityAmmoVendor : MonoBehaviour {

	public GameObject ammoVendor;
	public Texture2D icon;
	public bool isDisplaying = false;
	private DisplayItem ammo;
	private Ability curAbility;
	
	private int ammoToBuy;
	
	public void SetWeapon(Ability ability){
		curAbility = ability;
	}
	
	public void Vendor(){
		if(!isDisplaying){
			ammo = ScriptableObject.CreateInstance<DisplayItem>();
			ammo.item = ammoVendor;
			ammo.sellItem = ammoVendor.GetComponent<SellableItem>();
			
			int curResources = GameController.Instance.GetResources();
			int purchasableAmmo = curAbility.maxAmount - curAbility.amount;
			int ammoToPurchase = 0;
			
			while(ammoToPurchase < purchasableAmmo && curResources-(ammoToPurchase+1)*curAbility.costPerAmount >= 0)
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
		
		if(!sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.cost = 0;
			AbilitiesManager.Instance.AddAmount(curAbility, ammoToBuy);
			sellItem.purchased = true;
			Debug.Log("Purchased: " + sellItem.itemName);
		}
	}
	
	public void Cancel(){
		isDisplaying = false;
	}
}