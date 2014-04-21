using UnityEngine;

public class AttachMoneyToPlayer : MonoBehaviour {
	
	private float moveSpeed = 15f;
	
	void Update(){
		if(GameController.Instance.GetPlayer()){
			Transform target = GameController.Instance.GetPlayer();
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
			Quaternion rotate = Quaternion.LookRotation(target.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * moveSpeed);
		} else {
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(other.collider.tag == Globals.PLAYER){
			other.collider.GetComponent<Health>().AddHealth(1f);
			Destroy(gameObject);
		}
	}
}
