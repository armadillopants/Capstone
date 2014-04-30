using UnityEngine;

public class MusicManager : MonoBehaviour {
	
	public AudioClip dayTime;
	public AudioClip nightTime;
	public AudioClip buildPhase;
	public AudioClip mainMenu;
	
	private DayNightCycle cycle;
	
	void Start(){
		audio.clip = mainMenu;
		audio.loop = true;
		audio.playOnAwake = true;
		audio.Play();
		
		cycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
	}
	
	void FixedUpdate(){
		if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
			if(cycle.currentPhase == DayNightCycle.DayPhase.DAY || cycle.currentPhase == DayNightCycle.DayPhase.DAWN){
				if(GameController.Instance.GetWaveController().GetComponent<Fortification>() == null){
					if(audio.clip != dayTime){
						iTween.AudioTo(gameObject, iTween.Hash("volume", 0f, "time", 5f, "oncomplete", "SwitchClips", "oncompleteparams", dayTime));
					}
				} else {
					if(audio.clip == dayTime){
						iTween.AudioTo(gameObject, iTween.Hash("volume", 0f, "time", 5f, "oncomplete", "SwitchClips", "oncompleteparams", buildPhase));
					}
				}
			}
			
			if(cycle.currentPhase == DayNightCycle.DayPhase.NIGHT || cycle.currentPhase == DayNightCycle.DayPhase.DUSK){
				if(GameController.Instance.GetWaveController().GetComponent<Fortification>() == null){
					if(audio.clip != nightTime){
						iTween.AudioTo(gameObject, iTween.Hash("volume", 0f, "time", 5f, "oncomplete", "SwitchClips", "oncompleteparams", nightTime));
					}
				} else {
					if(audio.clip == nightTime){
						iTween.AudioTo(gameObject, iTween.Hash("volume", 0f, "time", 5f, "oncomplete", "SwitchClips", "oncompleteparams", buildPhase));
					}
				}
			}
		} else {
			if(audio.clip != mainMenu){
				iTween.AudioTo(gameObject, iTween.Hash("volume", 0f, "time", 5f, "oncomplete", "SwitchClips", "oncompleteparams", mainMenu));
			}
		}
		
		if(audio.volume <= 0){
			iTween.AudioTo(gameObject, iTween.Hash("volume", 0.3f, "time", 5f));
		}
	}
	
	void SwitchClips(AudioClip clip){
		audio.clip = clip;
		audio.Play();
	}
}
