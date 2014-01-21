using UnityEngine;

public class DayNightCycle : MonoBehaviour {
	
	public enum DayPhase { NIGHT, DAWN, DAY, DUSK };
	
    public float dayLength;
    public float curTime;
	
    public DayPhase currentPhase;
 
    public Color fullLight;
    public Color fullDark;
 
    private float dawnTime;
    private float dayTime;
    private float duskTime;
    private float nightTime;
 
    private float quarterDay;
    private float lightIntensity;
	
    public void Initialize(){
		curTime = 0;
		dayLength = 500.0f;
        quarterDay = dayLength * 0.25f;
        dawnTime = 0.0f;
        dayTime = dawnTime + quarterDay;
        duskTime = dayTime + quarterDay;
        nightTime = duskTime + quarterDay;
        if(light != null){ 
			lightIntensity = light.intensity;
		}
		SetDawn();
		UpdateDaylight();
    }
 
    void Start(){
        Initialize();
    }
 
    void Update(){
		if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
			if(curTime > dayTime && currentPhase == DayPhase.DAWN){
				SetDay();
			} else if(curTime > duskTime && currentPhase == DayPhase.DAY){
				SetDusk();
			} else if(curTime > nightTime && currentPhase == DayPhase.DUSK){
				SetNight();
			} else if(curTime > dawnTime && curTime < dayTime && currentPhase == DayPhase.NIGHT){
				SetDawn();
			}
	
	        UpdateDaylight();
	 
	        curTime += Time.deltaTime;
	        curTime %= dayLength;
		}
    }
 
    void SetDawn(){
        if(light != null){ 
			light.enabled = true; 
		}
        currentPhase = DayPhase.DAWN;
    }
	
    void SetDay(){
        RenderSettings.ambientLight = fullLight;
        if(light != null){ 
			light.intensity = lightIntensity; 
		}
        currentPhase = DayPhase.DAY;
    }
 
    void SetDusk(){
        currentPhase = DayPhase.DUSK;
    }

    void SetNight(){
        RenderSettings.ambientLight = fullDark;
        if(light != null){ 
			light.enabled = false; 
		}
        currentPhase = DayPhase.NIGHT;
    }
 
   	void UpdateDaylight(){
        if(currentPhase == DayPhase.DAWN){
            float relativeTime = curTime - dawnTime;
            RenderSettings.ambientLight = Color.Lerp(fullDark, fullLight, relativeTime / quarterDay);
            if(light != null){
				light.intensity = lightIntensity * (relativeTime / quarterDay);
			}
        } else if(currentPhase == DayPhase.DUSK){
            float relativeTime = curTime - duskTime;
            RenderSettings.ambientLight = Color.Lerp(fullLight, fullDark, relativeTime / quarterDay);
            if(light != null){
				light.intensity = lightIntensity * ((quarterDay - relativeTime) / quarterDay);
			}
        }
		
        transform.Rotate(Vector3.up * ((Time.deltaTime / dayLength) * 360.0f), Space.Self);
   }
}