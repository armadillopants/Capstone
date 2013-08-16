using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponVendor : MonoBehaviour {
	
	public List<GameObject> weaponVendor = new List<GameObject>();
	public List<GameObject> upgradeVendor = new List<GameObject>();
	public Texture2D icon;
	private bool isDisplaying = false;
	private bool isDisplayingUpgrades = false;
	private SellableItemDisplayer displayer;
	private DisplayItem weapon;
	private DisplayItem upgrade;
	private XMLVendorReader vendor;
	private WeaponManager manager;
	
	private float rows = 5;
	private float columns = 3;

	void Start(){
		displayer = GameObject.Find("ItemDisplayer").GetComponent<SellableItemDisplayer>();
		vendor = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
		manager = GameObject.FindWithTag("Player").GetComponentInChildren<WeaponManager>();
	}
	
	public void Vendor(float x, float y){
		if(!isDisplaying){
			int weaponIndex = 0;
			for(float r=0; r<rows; r++){
				for(float c=0; c<columns; c++){
					if(weaponIndex >= weaponVendor.Count){
						break;
					}
					weapon = ScriptableObject.CreateInstance<DisplayItem>();
					isDisplaying = true;
					weapon.item = weaponVendor[weaponIndex].gameObject;
					weapon.sellItem = weaponVendor[weaponIndex].GetComponent<SellableItem>();
					weapon.upgrade = false;
					weapon.hasWorldspace = false;
					weapon.worldspaceLocation = new Vector3(0,1,0);
					weapon.windowSize = new Vector2(200,100);
					Vector2 display = new Vector2(c*160, r*100);
					weapon.pixelOffset = new Vector2(x, y) + display;
					weapon.icon = icon;
					weapon.iconSize = 50;
					weapon.invokingObject = this;
					weapon.invokingType = this.GetType();
					displayer.AddToDisplay(weapon);
					weaponIndex++;
				}
			}
		}
	}
	
	public void UpgradeVendor(float x, float y){
		if(!isDisplayingUpgrades){
			int upgradeIndex = 0;
			for(float r=0; r<rows; r++){
				for(float c=0; c<columns; c++){
					if(upgradeIndex >= upgradeVendor.Count){
						break;
					}
					upgrade = ScriptableObject.CreateInstance<DisplayItem>();
					isDisplayingUpgrades = true;
					upgrade.item = upgradeVendor[upgradeIndex].gameObject;
					upgrade.sellItem = upgradeVendor[upgradeIndex].GetComponent<SellableItem>();
					if(upgrade.sellItem.currentUpgrade <= 4){
						upgrade.sellItem.cost = vendor.GetCurrentCost(upgrade.sellItem.cost, upgrade.sellItem.id, upgrade.sellItem.itemName, upgrade.sellItem.currentUpgrade);
					}
					upgrade.upgrade = true;
					upgrade.hasWorldspace = false;
					upgrade.worldspaceLocation = new Vector3(0,1,0);
					upgrade.windowSize = new Vector2(200,100);
					Vector2 display = new Vector2(c*160, r*100);
					upgrade.pixelOffset = new Vector2(x, y) + display;
					upgrade.icon = icon;
					upgrade.iconSize = 50;
					upgrade.invokingObject = this;
					upgrade.invokingType = this.GetType();
					displayer.AddToDisplay(upgrade);
					upgradeIndex++;
				}
			}
		}
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost && !sellItem.purchased){
			GameController.Instance.DeleteResources(sellItem.cost);
			sellItem.purchased = true;
			upgradeVendor.Add(sellItem.gameObject);
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
	
	public void CancelUpgrades(){
		isDisplayingUpgrades = false;
		displayer.Purge();
	}
}