using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponVendor : MonoBehaviour {
	
	public List<GameObject> weaponVendor = new List<GameObject>();// GameObject[] weaponVendor;
	public List<GameObject> upgradeVendor = new List<GameObject>();// GameObject[] upgradeVendor;
	public Texture2D icon;
	public Vector2[] offset;
	private bool isDisplaying = false;
	private SellableItemDisplayer displayer;
	private DisplayItem weapon;
	private DisplayItem upgrade;
	private XMLVendorReader vendor;

	void Start(){
		displayer = GameObject.Find("ItemDisplayer").GetComponent<SellableItemDisplayer>();
		vendor = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
		//weaponVendor = GameObject.FindGameObjectsWithTag("Weapon");
	}
	
	public void Vendor(float x, float y){
		if(!isDisplaying){
			for(int i=0; i<weaponVendor.Count; i++){
				//DisplayItem weapon;
				weapon = ScriptableObject.CreateInstance<DisplayItem>();
				isDisplaying = true;
				weapon.item = weaponVendor[i].gameObject;
				weapon.sellItem = weaponVendor[i].GetComponent<SellableItem>();
				weapon.upgrade = false;
				weapon.hasWorldspace = false;
				weapon.worldspaceLocation = new Vector3(0,1,0);
				weapon.windowSize = new Vector2(200,100);
				weapon.pixelOffset = new Vector2(x, y+(i*100));//offset[i];
				weapon.icon = icon;
				weapon.iconSize = 50;
				weapon.invokingObject = this;
				weapon.invokingType = this.GetType();
				displayer.AddToDisplay(weapon);
			}
		}
	}
	
	public void UpgradeVendor(float x, float y){
		if(!isDisplaying){
			for(int i=0; i<upgradeVendor.Count; i++){
				upgrade = ScriptableObject.CreateInstance<DisplayItem>();
				isDisplaying = true;
				upgrade.item = upgradeVendor[i].gameObject;
				upgrade.sellItem = upgradeVendor[i].GetComponent<SellableItem>();
				if(upgrade.sellItem.currentUpgrade <= 4){
					upgrade.sellItem.cost = vendor.GetCurrentCost(upgrade.sellItem.cost, upgrade.sellItem.id, upgrade.sellItem.itemName, upgrade.sellItem.currentUpgrade);
				}
				upgrade.upgrade = true;
				upgrade.hasWorldspace = false;
				upgrade.worldspaceLocation = new Vector3(0,1,0);
				upgrade.windowSize = new Vector2(200,100);
				upgrade.pixelOffset = new Vector2(x, y+(i*100));
				upgrade.icon = icon;
				upgrade.iconSize = 50;
				upgrade.invokingObject = this;
				upgrade.invokingType = this.GetType();
				displayer.AddToDisplay(upgrade);
			}
		}
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && !sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.purchased = true;
			upgradeVendor.Add(sellItem.gameObject);
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
			vendor.UpgradeData(sellItem.id, sellItem.itemName, sellItem.currentUpgrade);
			
			sellItem.currentUpgrade += 1;
			if(sellItem.currentUpgrade <=4){
				sellItem.cost = vendor.GetCurrentCost(sellItem.cost, sellItem.id, sellItem.itemName, sellItem.currentUpgrade);
			}
			Debug.Log("Purchased upgrade for: " + sellItem.itemName);
		} else if(sellItem.currentUpgrade >= 5){
			Debug.Log("No more upgrades for: " + sellItem.itemName);
		} else {
			Debug.Log("Not enough funds to purchase upgrade for: " + sellItem.itemName);
		}
	}
	
	public void Cancel(){
		isDisplaying = false;
		displayer.Purge();
	}
}