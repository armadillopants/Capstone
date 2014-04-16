using UnityEngine;
 
public class ShieldAnim : MonoBehaviour {
	
	public float scrollSpeed;
	private Material shieldMat;
 
	void Start(){
		shieldMat = renderer.material;
	}
 
	void LateUpdate(){
		float offset = Time.deltaTime * scrollSpeed;
		shieldMat.SetFloat("_Offset", Mathf.Repeat(offset, 1.0f));
	}
}