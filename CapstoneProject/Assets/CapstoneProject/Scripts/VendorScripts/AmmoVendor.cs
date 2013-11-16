using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmmoVendor : MonoBehaviour {

	public GameObject ammoVendor;
	public GameObject weaponLink;
	public Texture2D icon;
	private bool isDisplaying = false;
	private SellableItemDisplayer displayer;
	private DisplayItem ammo;
	private BaseWeapon curWeapon;

	void Start(){
		displayer = GameObject.Find(Globals.ITEM_DISPLAYER).GetComponent<SellableItemDisplayer>();
	}
	
	public void SetWeapon(GameObject weapon){
		weaponLink = weapon;
		curWeapon = weaponLink.GetComponent<BaseWeapon>();
	}
	
	public void Vendor(float x, float y){
		if(!isDisplaying){
			ammo = ScriptableObject.CreateInstance<DisplayItem>();
			isDisplaying = true;
			ammo.item = ammoVendor;
			ammo.sellItem = ammoVendor.GetComponent<SellableItem>();
			int finalCost = (int)(((curWeapon.bulletsPerClip-curWeapon.bulletsLeft)*curWeapon.costPerBullet)+((curWeapon.maxClips-curWeapon.clips)*(curWeapon.bulletsPerClip*curWeapon.costPerBullet)));
			ammo.sellItem.purchased = false;
			ammo.sellItem.cost = finalCost;
			ammo.upgrade = false;
			ammo.hasWorldspace = false;
			ammo.worldspaceLocation = new Vector3(0,1,0);
			ammo.windowSize = new Vector2(200,100);
			ammo.pixelOffset = new Vector2(x, y);
			ammo.icon = icon;
			ammo.iconSize = 50;
			ammo.invokingObject = this;
			ammo.invokingType = this.GetType();
			displayer.AddToDisplay(ammo);
		}
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && !sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.cost = 0;
			curWeapon.Replenish();
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
		displayer.Purge();
	}
}
