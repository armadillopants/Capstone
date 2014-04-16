using UnityEngine;

public class Flashlight : MonoBehaviour {
	
	private Light flashLight;
	
	void Start(){
		flashLight = GetComponent<Light>();
		flashLight.range = 25f;
		flashLight.intensity = 1f;
		flashLight.spotAngle = 100f;
		
		TurnOff();
	}
	
	public void TurnOn(){
		flashLight.enabled = true;
	}
	
	public void TurnOff(){
		flashLight.enabled = false;
	}
}
