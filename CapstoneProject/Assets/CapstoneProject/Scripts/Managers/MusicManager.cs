using UnityEngine;

public class MusicManager : MonoBehaviour {
	
	public AudioClip dayTime;
	public AudioClip nightTime;
	public AudioClip buildPhase;
	public AudioClip mainMenu;
	
	private DayNightCycle cycle;
	private bool switchingClips = false;
	
	private int secondsToFade = 5;
	
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
						CrossFadeInto(dayTime);
					}
				} else {
					if(audio.clip == dayTime){
						CrossFadeInto(buildPhase);
					}
				}
			}
			
			if(cycle.currentPhase == DayNightCycle.DayPhase.NIGHT || cycle.currentPhase == DayNightCycle.DayPhase.DUSK){
				if(GameController.Instance.GetWaveController().GetComponent<Fortification>() == null){
					if(audio.clip != nightTime){
						CrossFadeInto(nightTime);
					}
				} else {
					if(audio.clip == nightTime){
						CrossFadeInto(buildPhase);
					}
				}
			}
			
			if(!switchingClips){
				if(audio.volume < 1){
					audio.volume += (Time.deltaTime / (secondsToFade + 1));
				}
			}
		}
	}
	
	void CrossFadeInto(AudioClip clip){
		if(audio.volume > 0){
			switchingClips = true;
			audio.volume -= (Time.deltaTime / (secondsToFade + 1));
		} else {
			switchingClips = false;
			audio.clip = clip;
			audio.Play();
		}
	}
}
