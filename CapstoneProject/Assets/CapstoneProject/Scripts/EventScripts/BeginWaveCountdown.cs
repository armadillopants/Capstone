using UnityEngine;
using System.Collections;

public class BeginWaveCountdown : MonoBehaviour {
	
	private int amountOfWavesLeft = 10;

	void Start(){
		GameController.Instance.SetCurWave(GameController.Instance.GetWaveController().GetWaveNumber());
		GameController.Instance.SetEndWave(amountOfWavesLeft);
	}
	
	void Update(){
		if(GameController.Instance.GetWaveController().GetComponent<Wave>() != null){
			if(GameController.Instance.GetWaveController().GetComponent<Wave>().endWave){
				amountOfWavesLeft -= 1;
			}
		}
	}
	
	public int GetWavesLeft(){
		return amountOfWavesLeft;
	}
}
