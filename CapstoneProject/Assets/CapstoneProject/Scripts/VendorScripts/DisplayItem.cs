using UnityEngine;

public class DisplayItem : ScriptableObject {
	public GameObject item;
	public SellableItem sellItem;
	public bool upgrade;

	// Window Variables
	public bool hasWorldspace;
	public Vector3 worldspaceLocation;
	public Vector2 windowSize;
	public Vector2 pixelOffset;

	// Icon Variables
	public Texture2D icon;
	public int iconSize;
	
	public Texture2D weaponState;
	
	public Texture2D equippedNormal;
	public Texture2D equippedHover;
	public Texture2D equippedActive;
	
	public Texture2D buyNormal;
	public Texture2D buyHover;
	public Texture2D buyActive;
	
	public Texture2D upgradeNormal;
	public Texture2D upgradeHover;
	public Texture2D upgradeActive;
	
	public System.Object invokingObject;
	public System.Type invokingType;
}
