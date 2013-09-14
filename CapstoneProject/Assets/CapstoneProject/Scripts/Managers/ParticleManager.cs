using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {
	
	private static ParticleEmitter particles;
	private static PlayerMovement move;
	
	public static void MoveParticlesWithPlayerVelocity(){
		move = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
		particles = move.GetComponentInChildren<ParticleEmitter>();
		particles.worldVelocity = move.moveDirection;
	}
}
