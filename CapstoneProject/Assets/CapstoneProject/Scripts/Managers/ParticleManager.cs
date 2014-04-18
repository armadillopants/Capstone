using UnityEngine;

public class ParticleManager : MonoBehaviour {
	
	private static ParticleEmitter particles;
	private static PlayerMovement move;
	
	public static void MoveParticlesWithPlayerVelocity(){
		move = GameObject.FindWithTag(Globals.PLAYER).GetComponent<PlayerMovement>();
		particles = move.GetComponentInChildren<ParticleEmitter>();
		particles.worldVelocity = move.moveDirection;
	}
}
