using UnityEngine;
using System.Collections;

public class BeginWaveCountdown : MonoBehaviour {
	
	private Animation anim;
	private int amountOfWavesLeft = 10;
	
	private GameObject waveController;

	void Start(){
		anim = GetComponent<Animation>();
		
		waveController = GameObject.Find("WaveController");
		
		if(anim){
			anim.wrapMode = WrapMode.Loop;
			
			anim.Play("Swirl");
		}
		
		GameController.Instance.SetCurWave(waveController.GetComponent<WaveController>().GetWaveNumber());
		GameController.Instance.SetEndWave(amountOfWavesLeft);
	}
	
	void Update(){
		if(waveController.GetComponent<Wave>() != null){
			if(waveController.GetComponent<Wave>().endWave){
				amountOfWavesLeft--;
			}
		}
	}
	
	void OnGUI(){
		GUI.Box(new Rect(0, 350, 250, 22), "Waves until Rescue Ship's arrival: " + amountOfWavesLeft);
	}
}
