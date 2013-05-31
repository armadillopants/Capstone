using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {
	
	private int waveNumber = 1;
	private const int WAVES_BETWEEN_FORTIFICATION = 5;
	private bool beginWave = false;
	public bool endWave = false;
	private WaveController controller;
	private float waitTime = 5.0f;

	void Start(){
	}
	
	public void StartWave(WaveController wave, int waveNum) {
		// Spawn amount based on wave number
		/*spawn_amountW = Mathf.FloorToInt(wave_Num * 0.5f) + 6;
		if(wave_Num>=3){
			spawn_amountR = Mathf.FloorToInt(wave_Num * 0.5f) + 3;
		}
		if(wave_Num>=5){
			spawn_amountB = Mathf.FloorToInt(wave_Num * 0.5f) + 1;
		}*/
		
		controller = wave;
		waveNumber = waveNum;
		beginWave = false;
    	StartCoroutine("WaveHandling");
  	}
	
	private IEnumerator WaveHandling(){
		if(waveNumber != 0 && waveNumber % WAVES_BETWEEN_FORTIFICATION == 0){
			Fortification fort = gameObject.AddComponent<Fortification>();
			fort.StartFortifying(this);
		} else {
			BeginWave();
		}
		
		while(!beginWave){
      		yield return new WaitForSeconds(Time.deltaTime);
    	}
	}
	
	public void BeginWave(){
		StartCoroutine("BeginNewWave");
	}
	
	// Never call this function directly as it is to be called through BeginWave
	IEnumerator BeginNewWave() {
		yield return new WaitForSeconds(waitTime);
		waitTime = 0;
		
		beginWave = true;
	}
	
	public void StopWave(){
		beginWave = false;
	}
	
	void Update(){
		// Check to see if the waveNumber has been set (StartWave has been called)
		// and if so, and the current wave is neither the first nor a wave that
		// has a fortification phase to it (which would set the beginWave flag itself)
		// then start the wave.
		if(waveNumber != 0 && waveNumber % WAVES_BETWEEN_FORTIFICATION != 0){
			BeginWave();
		}
		// If the wave is over, then start the next wave and destroy this instance
		// before any other unnecessary calculations are done
		if(endWave){
      		controller.StartNextWave();
			Destroy(this);
    	}
	}
}
