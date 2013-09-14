using UnityEngine;
using System.Collections.Generic;

public class AIPathCell : MonoBehaviour {
	
	public List<GameObject> nodes = new List<GameObject>();
	public List<GameObject> searchedNodes = new List<GameObject>();
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "AIPathNode"){
			nodes.Add(other.gameObject);
		}
	}
}
