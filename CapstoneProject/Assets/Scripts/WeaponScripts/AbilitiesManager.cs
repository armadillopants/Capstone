using UnityEngine;
using System.Collections;

public class AbilitiesManager : MonoBehaviour {
	
	private GameObject holder;
	
	void Start(){
		holder = GameObject.Find("AbilityHolder");
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.E)){
			holder.SendMessage("BeginAbility", SendMessageOptions.DontRequireReceiver);
		}
	}
}
