using UnityEngine;
using System.Collections;

public class AnimFrames : MonoBehaviour {
	
	public Texture2D[] frames;
	public float framesPerSecond = 10f;
	public bool resetFrames = false;
	
	void Update(){
		if(resetFrames){
			int index = (int)(Time.time * framesPerSecond);
			index %= frames.Length;
			renderer.material.mainTexture = frames[index];
		} else {
			int index = (int)(Time.time * framesPerSecond);
			if(index < frames.Length){
				renderer.material.mainTexture = frames[index];
			}
		}
	}
}
