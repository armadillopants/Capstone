using UnityEngine;
using System.Collections;

public class AnimFrames : MonoBehaviour {
	
	public Texture2D[] frames;
	public float framesPerSecond = 10f;
	
	void Update(){
		int index = (int)(Time.time * framesPerSecond);
		if(index < frames.Length){
			//index %= frames.Length;
			renderer.material.mainTexture = frames[index];
		}
	}
}
