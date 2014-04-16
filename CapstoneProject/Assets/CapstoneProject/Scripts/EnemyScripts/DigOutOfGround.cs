using UnityEngine;

public class DigOutOfGround : MonoBehaviour {
	
	void Start(){
		Instantiate(Spawner.spawner.explosion, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
		Instantiate(Spawner.spawner.hole, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
	}
	
	void Update(){
		if(transform.position.y < -0.1f){
			collider.isTrigger = true;
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0, transform.position.z), 0.5f*Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.5f*Time.deltaTime);
		} else {
			collider.isTrigger = false;
			GetComponent<Enemy>().isUnderground = false;
			Destroy(this);
		}
	}
	
	void OnTriggerEnter(Collider hit){
		if(hit.tag == Globals.FORTIFICATION){
			GameController.Instance.UpdateGraphOnDestroyedObject(hit.gameObject.collider, hit.gameObject);
		}
	}
}
