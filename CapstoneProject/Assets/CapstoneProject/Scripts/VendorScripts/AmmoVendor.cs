using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmmoVendor : MonoBehaviour {

	public GameObject ammoVendor;
	public GameObject weaponLink;
	public Texture2D icon;
	private bool isDisplaying = false;
	private DisplayItem ammo;
	private BaseWeapon curWeapon;
	
	private int clipsToBuy;
	private int bulletsToBuy;
	
	public void SetWeapon(GameObject weapon){
		weaponLink = weapon;
		curWeapon = weaponLink.GetComponent<BaseWeapon>();
	}
	
	public void Vendor(){
		if(!isDisplaying){
			ammo = ScriptableObject.CreateInstance<DisplayItem>();
			ammo.item = ammoVendor;
			ammo.sellItem = ammoVendor.GetComponent<SellableItem>();
			
			int curResources = GameController.Instance.GetResources();
			int purchasableClips = curWeapon.maxClips - curWeapon.clips;
			int purchasableBullets = curWeapon.bulletsPerClip - curWeapon.bulletsLeft;
			int bulletsToPurchase = 0;
			int clipsToPurchase = 0;
			
			while(bulletsToPurchase < purchasableBullets && curResources-(bulletsToPurchase+1)*curWeapon.costPerBullet >= 0)
			{
				bulletsToPurchase++;
			}
			while(clipsToPurchase < purchasableClips && curResources-(clipsToPurchase+curWeapon.bulletsPerClip)*curWeapon.costPerBullet >= 0)
			{
				clipsToPurchase++;
			}
			
			AddPurchasableAmmo(bulletsToPurchase, clipsToPurchase);
			
			ammo.sellItem.purchased = false;
			ammo.sellItem.cost = clipsToPurchase + bulletsToPurchase*curWeapon.costPerBullet;
			
			Debug.Log(ammo.sellItem.cost);
			isDisplaying = true;
		}
	}
	
	void AddPurchasableAmmo(int bullets, int clips){
		bulletsToBuy = bullets;
		clipsToBuy = clips;
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && !sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.cost = 0;
			curWeapon.PurchasedAmmo(bulletsToBuy, clipsToBuy);
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
