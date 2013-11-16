using UnityEngine;
using System.Collections;

public class AbilitiesManager : MonoBehaviour {
	
	private GameObject holder;
	public bool beginAbility = false;
	private int amount = 3;
	private float coolDown;
	private float maxCoolDown = 30f;
	
	private string ability = "";
	
	#region Singleton
	
	private static AbilitiesManager _instance;
	
	public static AbilitiesManager Instance {
		get { return _instance; }
	}
	
	void Awake(){
		if(AbilitiesManager.Instance != null){
			DestroyImmediate(gameObject);
			return;
		}
		_instance = this;
	}
	
	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion
	
	void Start(){
		holder = GameObject.Find("AbilityHolder");
		beginAbility = true;
	}
	
	public void SetAbility(string newAbility){
		ability = newAbility;
	}
	
	void Update(){
		if(coolDown > 0){
			coolDown -= 1f*Time.deltaTime;
			
			if(coolDown <= 0){
				beginAbility = true;
				coolDown = 0;
			}
		}
		
		if(holder.transform.GetComponent(ability)){
			if(holder.transform.GetComponent(ability).ToString().Contains(ability)){
				if(GameController.Instance.canShoot){
					if(Input.GetKeyDown(KeyCode.E) && beginAbility && amount > 0){
						holder.SendMessage("BeginAbility", SendMessageOptions.DontRequireReceiver);
						beginAbility = false;
						amount--;
					}
				}
			}
		}
	}
	
	public void AddAmount(int howMuch){
		amount = howMuch;
	}
	
	public void SetCoolDown(){
		coolDown = maxCoolDown;
	}
}
