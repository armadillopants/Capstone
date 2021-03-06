﻿using UnityEngine;
using System.Collections;

public class FlyToShip : MonoBehaviour {
	
	private Transform ship;
	private Transform player;
	private float step;
	private float offset;
	private Vector3 shipPos;
	
	private float timer = 120.00f;
	private Transform trans;
	
	void Start(){
		trans = transform;
		ship = GameController.Instance.GetShip();
		player = GameController.Instance.GetPlayer();
		
		offset = 8f;
		
		shipPos = new Vector3(ship.position.x, ship.position.y+5f, ship.position.z);
	}
	
	void Update(){
		trans.position = Vector3.Lerp(trans.position, shipPos, 1*Time.deltaTime);
		Quaternion rotateShip = Quaternion.LookRotation(ship.forward, trans.up);
		trans.rotation = Quaternion.Slerp(trans.rotation, rotateShip, 1*Time.deltaTime);
		
		step += 0.01f;
	
		if(step > 999999f){
			step = 1f;
		}
		
		Hover();
		
		if(GameController.Instance.GetWaveController().GetComponent<Wave>() != null){
			if(GameController.Instance.GetWaveController().GetComponent<Wave>().beginWave && !GameController.Instance.GetPlayerHealth().IsDead){
				timer -= Time.deltaTime;
			}
		}
		
		if(timer <= 0){
			timer = 0;
			Destroy(GameController.Instance.GetWaveController().GetComponent<Wave>());
			Destroy(GameObject.FindWithTag(Globals.PLAYER).GetComponent<LocalInput>());
			Destroy(GameObject.FindWithTag(Globals.PLAYER).GetComponent<PlayerMovement>());
			UIManager.Instance.uiState = UIManager.UIState.GAMEWON;
			player.position = Vector3.Lerp(player.position, ship.position, 1*Time.deltaTime);
			Quaternion rotatePlayer = Quaternion.LookRotation(ship.forward, player.up);
			player.rotation = Quaternion.Slerp(player.rotation, rotatePlayer, 1*Time.deltaTime);
			player.parent = ship;
			ship.transform.parent = trans;
			StartCoroutine(BlastOff());
		}
	}
	
	private IEnumerator BlastOff(){
		yield return new WaitForSeconds(5f);
		trans.position = Vector3.Lerp(trans.position, new Vector3(50, 60, 80), 1*Time.deltaTime);
	}
	
	void Hover(){
		Vector3 pos = trans.position;
		pos.y = Mathf.Sin(step)+offset;
		trans.position = pos;
	}
	
	public float GetTimer(){
		return timer;
	}
}
