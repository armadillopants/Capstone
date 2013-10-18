using UnityEngine;
using System.Collections;

public class ItemVendor : MonoBehaviour {

	public GameObject[] itemVendor;
	public GameObject upgradeItemVendor;
	public Texture2D icon;
	private bool isDisplaying = false;
	private bool isDisplayingUpgrades = false;
	private SellableItemDisplayer displayer;
	private DisplayItem item;
	private DisplayItem upgrade;
	private XMLVendorReader vendor;
	
	private float rows = 5;
	private float columns = 3;

	void Start(){
		displayer = GameObject.Find("ItemDisplayer").GetComponent<SellableItemDisplayer>();
		vendor = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
	}
	
	public void Vendor(float x, float y){
		if(!isDisplaying){
			int itemIndex = 0;
			for(float r=0; r<rows; r++){
				for(float c=0; c<columns; c++){
					if(itemIndex >= itemVendor.Length){
						break;
					}
					item = ScriptableObject.CreateInstance<DisplayItem>();
					isDisplaying = true;
					item.item = itemVendor[itemIndex].gameObject;
					item.sellItem = itemVendor[itemIndex].GetComponent<SellableItem>();
					item.upgrade = false;
					item.hasWorldspace = false;
					item.worldspaceLocation = new Vector3(0,1,0);
					item.windowSize = new Vector2(200,100);
					Vector2 display = new Vector2(c*160, r*100);
					item.pixelOffset = new Vector2(x, y) + display;
					item.icon = icon;
					item.iconSize = 50;
					item.invokingObject = this;
					item.invokingType = this.GetType();
					displayer.AddToDisplay(item);
					itemIndex++;
				}
			}
		}
	}
	
	public void UpgradeVendor(float x, float y){
		if(!isDisplayingUpgrades){
			upgrade = ScriptableObject.CreateInstance<DisplayItem>();
			isDisplayingUpgrades = true;
			upgrade.item = upgradeItemVendor.gameObject;
			upgrade.sellItem = upgradeItemVendor.GetComponent<SellableItem>();
			upgrade.upgrade = true;
			upgrade.hasWorldspace = false;
			upgrade.worldspaceLocation = new Vector3(0,1,0);
			upgrade.windowSize = new Vector2(200,100);
			upgrade.pixelOffset = new Vector2(x, y);
			upgrade.icon = icon;
			upgrade.iconSize = 50;
			upgrade.invokingObject = this;
			upgrade.invokingType = this.GetType();
			displayer.AddToDisplay(upgrade);
		}
	}
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			Cancel();
			GameController.Instance.SetFortificationToSpawn(sellItem.gameObject);
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
			vendor.SetFortData(sellItem.gameObject);
			vendor.UpgradeFortificationData(sellItem.id, sellItem.itemName, sellItem.currentUpgrade);
			if(sellItem.upgradedItem){
				Instantiate(sellItem.upgradedItem, sellItem.gameObject.transform.position, sellItem.gameObject.transform.rotation);
				Destroy(sellItem.gameObject);
			}
			
			sellItem.currentUpgrade += 1;
			if(sellItem.currentUpgrade <=4){
				sellItem.cost = vendor.GetCurrentFortificationCost(sellItem.cost, sellItem.id, sellItem.itemName, sellItem.currentUpgrade);
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

