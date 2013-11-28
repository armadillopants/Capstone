using UnityEngine;

public class Flashlight : MonoBehaviour {
	
	private Light flashLight;
	public float batteryLife;
	private float maxBatteryLife = 3600f;
	public AnimationCurve batteryCurve;
	private bool switchOn = false;
	
	void Start(){
		batteryLife = Random.Range(maxBatteryLife*0.5f, maxBatteryLife);
		
		flashLight = GetComponent<Light>();
		flashLight.range = 20f;
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
	}
	
	public void TurnOn(){
		switchOn = true;
		flashLight.enabled = true;
	}
	
	public void TurnOff(){
		switchOn = false;
		flashLight.enabled = false;
	}
}
