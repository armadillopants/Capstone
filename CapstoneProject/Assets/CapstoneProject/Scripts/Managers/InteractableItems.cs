using UnityEngine;
using System.Collections.Generic;

public class InteractableItems : MonoBehaviour {

	public List<Vector3> itemsPos = new List<Vector3>();
	
	void Start(){
		GameObject[] item = GameObject.FindGameObjectsWithTag(Globals.INTERACTABLE_ITEM);
		for(int i=0; i<item.Length; i++){
			itemsPos.Add(item[i].transform.position);
		}
	}
}
