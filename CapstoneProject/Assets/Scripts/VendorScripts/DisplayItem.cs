using UnityEngine;

public class DisplayItem : ScriptableObject {
	public GameObject item;
	public SellableItem sellItem;
	public bool upgrade;
	public int id;

	// Window Variables
	public bool hasWorldspace;
	public Vector3 worldspaceLocation;
	public Vector2 windowSize;
	public Vector2 pixelOffset;

	// Icon Variables
	public Texture2D icon;
	public int iconSize;
	
	public System.Object invokingObject;
	public System.Type invokingType;
}
