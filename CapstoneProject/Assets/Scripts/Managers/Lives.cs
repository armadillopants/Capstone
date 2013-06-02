using UnityEngine;
using System.Collections;

public class Lives : MonoBehaviour {
	
	public int curLives;
	private int maxLives = 3;

	void Start(){
	
	}
	
	void Update(){
	
	}
	
	public void ModifyLives(int amount){
		maxLives = amount;
		curLives = maxLives;
	}
	
	public void AddLives(int howMuch){
		curLives = Mathf.Min(maxLives, curLives+howMuch);
	}
}
