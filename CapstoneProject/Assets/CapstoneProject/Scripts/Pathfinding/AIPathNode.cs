using UnityEngine;
using System.Collections.Generic;

public class AIPathNode : MonoBehaviour {
	
	public GameObject[] cells;
	public int[] nodesToCells;
	public List<GameObject> immediateCells = new List<GameObject>();
	public bool testForCells = true;
	public int waitToTestCells = 2;
	public int stage = 1;
	public GameObject fortification;
	
	//[HideInInspector]
	public bool nodeOpen = true;
	
	void Start(){
		nodeOpen = true;
		cells = GameObject.FindGameObjectsWithTag("AIPathCell");
		nodesToCells = new int[cells.Length];
		testForCells = true;
		waitToTestCells = 2;
		stage = 1;
	}
	
	void Update(){
		if(fortification){
			nodeOpen = fortification.GetComponent<Health>().IsDead;
		}
		
		if(testForCells && waitToTestCells <= 0){
			foreach(GameObject immediateCell in immediateCells){
				for(int i=0; i<=cells.Length-1; i++){
					if(cells[i] == immediateCell){
						nodesToCells[i] = 1;
					}
				}
			}
			
			for(stage=2; stage<=cells.Length; stage++){
				for(int i=0; i<=cells.Length-1; i++){
					if(nodesToCells[i] == stage-1){
						foreach(GameObject checkNode in cells[i].GetComponent<AIPathCell>().nodes){
							if(checkNode != gameObject){
								foreach(GameObject checkCell in checkNode.GetComponent<AIPathNode>().immediateCells){
									for(int j=0; j<=cells.Length-1; j++){
										if(cells[j] == checkCell && nodesToCells[j] == 0){
											nodesToCells[j] = stage;
										}
									}
								}
							}
						}
					}
				}
			}
			testForCells = false;
		}
		waitToTestCells -= 1;
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "AIPathCell"){
			immediateCells.Add(other.gameObject);
		}
	}
	
	void OnTriggerStay(Collider other){
		if(other.tag == "Fortification"){
			fortification = other.gameObject;
		}
	}
	
	void OnTriggerExit(){
		fortification = null;
		nodeOpen = true;
	}
}
