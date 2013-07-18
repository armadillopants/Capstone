using UnityEngine;
using System.Collections;

public class WeaponVendor : MonoBehaviour {
	
	public GameObject[] weaponVendor;
	public Texture2D icon;
	public Vector2[] offset;
	private bool isDisplaying = false;
	private SellableItemDisplayer displayer;

	// Use this for initialization
	void Start(){
		displayer = GameObject.Find("ItemDisplayer").GetComponent<SellableItemDisplayer>();
		weaponVendor = GameObject.FindGameObjectsWithTag("Weapon");
	}
	
	public void Vendor(){
		if(!isDisplaying){
			for(int i=0; i<weaponVendor.Length; i++){
				DisplayItem weapon;
				weapon = ScriptableObject.CreateInstance<DisplayItem>();
				isDisplaying = true;
				weapon.item = weaponVendor[i].gameObject;
				weapon.sellItem = weaponVendor[i].GetComponent<SellableItem>();
				weapon.upgrade = false;
				weapon.hasWorldspace = false;
				weapon.worldspaceLocation = new Vector3(0,1,0);
				weapon.windowSize = new Vector2(200,100);
				weapon.pixelOffset = offset[i];
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
		Debug.Log("purchased: " + sellItem.itemName);
	}
	
	public void Upgrade(GameObject item){
		
	}
	
	public void Cancel(){
		isDisplaying = false;
	}
}