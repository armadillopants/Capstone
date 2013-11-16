﻿using UnityEngine;

public class Flashlight : MonoBehaviour {
	
	private Light flashLight;
	public float batteryLife;
	private float maxBatteryLife = 3600f;
	public AnimationCurve batteryCurve;
	private bool switchOn = false;
	
	void Start(){
		batteryLife = Random.Range(maxBatteryLife*0.5f, maxBatteryLife);
		
		flashLight = GetComponent<Light>();
		flashLight.range = 15f;
		flashLight.intensity = 1f;
		flashLight.spotAngle = 100f;
		
		TurnOff();
	}
	
	void Update(){
		if(switchOn){
			batteryLife -= Time.deltaTime;
			if(batteryLife <= 0){
				batteryLife = 0;
				TurnOff();
			}
			
			float batteryCurveEval = batteryCurve.Evaluate(1.0f-batteryLife/maxBatteryLife);
			flashLight.intensity -= (batteryCurveEval/maxBatteryLife*0.5f) * Time.deltaTime;
		}
		
		/*if(Input.GetKeyDown(KeyCode.F) && batteryLife > 0){
			TurnOnOff();
		}*/
	}
	
	public void TurnOn(){
		switchOn = true;//!switchOn;
		flashLight.enabled = true;//!flashLight.enabled;
	}
	
	public void TurnOff(){
		switchOn = false;
		flashLight.enabled = false;
	}
}
