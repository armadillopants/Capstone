using UnityEngine;

public class Flashlight : MonoBehaviour {
	
	private Light flashLight;
	
	void Start(){
		flashLight = GetComponent<Light>();
		flashLight.range = 15f;
		flashLight.intensity = 1f;
		flashLight.spotAngle = 100f;
		flashLight.enabled = false;
	}
	
	void Update(){
		if(GameController.Instance.timeOfDay == GameController.TimeOfDay.DAYTIME){
			flashLight.enabled = false;
		} else {
			flashLight.enabled = true;
		}
	}
}
