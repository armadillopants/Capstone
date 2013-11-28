using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	
	Vector3 oldPos = Vector3.zero;
	private bool activeObject = true;

	void Start(){
		oldPos = transform.position;
		renderer.material.color = Color.white;
	}
	
	void Update(){
		transform.position = Vector3.Lerp(transform.position, oldPos, 1*Time.deltaTime);
	}
	
	void OnMouseDown(){
		activeObject = false;
		transform.position = Vector3.Lerp(transform.position, transform.position+new Vector3(0,0,0.2f), 5*Time.deltaTime);
	}
	
	void OnMouseUp(){
		if(!activeObject){
			transform.GetChild(0).SendMessage("Execute", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void OnMouseOver(){
		if(activeObject){
			transform.position = Vector3.Lerp(transform.position, transform.position+new Vector3(0,0,-0.1f), 1*Time.deltaTime);
		}
	}
	
	void OnMouseEnter(){
		renderer.material.color = Color.blue;
	}
	
	void OnMouseExit(){
		activeObject = true;
		renderer.material.color = Color.white;
	}
}
