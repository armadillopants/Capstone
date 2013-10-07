using UnityEngine;
using System.Collections;

public class Lives : MonoBehaviour {
	
	public int curLives;
	private int maxLives = 3;
	private int minLives = 0;

	void Start(){
	
	}
	
	void Update(){
	
	}
	
	public int GetMaxLives(){
		return maxLives;
	}
	
	public int GetLives(){
		return curLives;
	}
	
	public void ModifyLives(int amount){
		maxLives = amount;
		curLives = maxLives;
	}
	
	public void AddLives(int howMuch){
		curLives = Mathf.Min(maxLives, curLives+howMuch);
	}
	
	public void TakeLives(int amount){
		curLives = Mathf.Max(minLives, curLives-amount);
	}
}
