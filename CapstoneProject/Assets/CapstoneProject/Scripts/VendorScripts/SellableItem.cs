using UnityEngine;

public class SellableItem : MonoBehaviour {
	public string itemName;
	public string description;
	public int cost;
	public Texture2D preview;
	public bool purchased;
	public int currentUpgrade;
	public int id;
	public GameObject upgradedItem;
}
