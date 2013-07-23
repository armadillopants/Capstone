using UnityEngine;
using System.Collections;

public class ItemVendor : MonoBehaviour {

	public GameObject[] itemVendor;
	public Texture2D icon;
	public Vector2[] offset;
	private bool isDisplaying = false;
	private SellableItemDisplayer displayer;

	void Start(){
		displayer = GameObject.Find("ItemDisplayer").GetComponent<SellableItemDisplayer>();
		//weaponVendor = GameObject.FindGameObjectsWithTag("Weapon");
	}
	
	public void Vendor(float x, float y){
		if(!isDisplaying){
			for(int i=0; i<itemVendor.Length; i++){
				DisplayItem weapon;
				weapon = ScriptableObject.CreateInstance<DisplayItem>();
				isDisplaying = true;
				weapon.item = itemVendor[i].gameObject;
				weapon.sellItem = itemVendor[i].GetComponent<SellableItem>();
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
	
	public void Purchase(GameObject item){
		SellableItem sellItem = item.GetComponent<SellableItem>();
		
		if(GameController.Instance.GetResources() >= sellItem.cost){
			GameController.Instance.canDisplay = false;
			Instantiate(sellItem.gameObject, new Vector3(0, 0.5f, 0), Quaternion.identity);
			GameController.Instance.DeleteResources(sellItem.cost);
			
			Debug.Log("Purchased: " + sellItem.itemName);
		}
	}
	
	public void Upgrade(GameObject item){
		
	}
	
	public void Cancel(){
		isDisplaying = false;
		displayer.Purge();
	}
}

