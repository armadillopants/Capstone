using System.Collections.Generic;
using UnityEngine;

public class FilmCameraScript : MonoBehaviour {
	
	private Camera cam;
	private bool displayLocations = false;
	public List<GameObject> filmLocations = new List<GameObject>();
	private Vector2 scrollPos = new Vector2();
	
	private float maxFOV = 20f;
	private float minFOV = 60f;
	private DayNightCycle cycle;

	void Start(){
		cam = GetComponent<Camera>().camera;
		cam.enabled = false;
		
		foreach(GameObject loc in GameObject.FindGameObjectsWithTag("Film")){
			filmLocations.Add(loc);
		}
		
		cycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
	}
	
	/*void Update(){
		if(Input.GetKeyDown(KeyCode.Space)){
			EnableCamera();
		}
		
		if(Input.GetKeyDown(KeyCode.Tab)){
			displayLocations = !displayLocations;
		}
		
		if(Input.GetKey(KeyCode.C)){
			cycle.curTime += 50f*Time.deltaTime;
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha4)){
			UIManager.Instance.displayUI = !UIManager.Instance.displayUI;
			UIManager.Instance.displayOtherStoof = !UIManager.Instance.displayOtherStoof;
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			cam.fieldOfView = minFOV;
		} else if(Input.GetKeyDown(KeyCode.Alpha2)){
			cam.fieldOfView = minFOV-maxFOV;
		} else if(Input.GetKeyDown(KeyCode.Alpha3)){
			cam.fieldOfView = maxFOV;
		}
		
		if(Input.GetKey(KeyCode.Z)){
			if(cam.fieldOfView > 0){
				cam.fieldOfView -= 5f*Time.deltaTime;
			}
		}
		
		if(Input.GetKey(KeyCode.X)){
			if(cam.fieldOfView < 60){
				cam.fieldOfView += 5f*Time.deltaTime;
			}
		}
	}*/
	
	void EnableCamera(){
		cam.enabled = !cam.enabled;
	}
	
	void OnGUI(){
		if(displayLocations){
			scrollPos = GUI.BeginScrollView(new Rect(10, 10, 150, 100), scrollPos, new Rect(0, 0, 70, 300));
			for(int i=0; i<filmLocations.Count; i++){
				if(GUILayout.Button(filmLocations[i].name)){
					cam.transform.position = filmLocations[i].transform.position;
					cam.transform.rotation = filmLocations[i].transform.rotation;
					cam.transform.parent = filmLocations[i].transform;
				}
			}
			GUI.EndScrollView();
		}
	}
}
