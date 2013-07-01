using UnityEngine;

public class AbilitiesManager : MonoBehaviour {
	
	private GameObject holder;
	public bool beginAbility = false;
	public int amount = 3;
	
	void Start(){
		holder = GameObject.Find("AbilityHolder");
		beginAbility = true;
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.E) && beginAbility && amount > 0){
			holder.SendMessage("BeginAbility", SendMessageOptions.DontRequireReceiver);
			beginAbility = false;
			amount--;
		}
	}
}
