using UnityEngine;

public class EnemyMovement : MonoBehaviour {
	
	public GameObject currentCell;
	private PlayerMovement playerMovement;
	public Transform target;
	public GameObject playerCell;
	public GameObject goalNode;
	public float shortestPathSoFar;
	public int waitToStart = 5;
	public float curMoveSpeed;
	public float minMoveSpeed = 1f;
	public float maxMoveSpeed = 5f;
	private float speedDamage = 0.1f;
	private float speedRecover = 0.1f;
	
	private bool randomizedCourse;
	private Vector3 randomizeCourseVector;
	private bool calculatatedNewRandomizeCourseVector;
	
	void Start(){
		shortestPathSoFar = Mathf.Infinity;
		target = GameObject.FindWithTag("Player").transform;
		playerMovement = target.GetComponent<PlayerMovement>();
		waitToStart = 5;
		randomizeCourseVector = transform.position;
		curMoveSpeed = maxMoveSpeed;
	}
	
	void Update(){
		if(waitToStart <= 0){
			playerCell = playerMovement.currentCell;
			// Check all the nodes associated with the current cell
			foreach(GameObject nodeCheckingNow in currentCell.GetComponent<AIPathCell>().nodes){
				for(int i=0; i<=nodeCheckingNow.GetComponent<AIPathNode>().cells.Length-1; i++){
					if(!nodeCheckingNow.GetComponent<AIPathNode>().cells[i].GetComponent<AIPathCell>().searchedNodes.Contains(nodeCheckingNow)){
						// If any of the cells that exist contain the players cell
						if(nodeCheckingNow.GetComponent<AIPathNode>().cells[i] == playerCell){
							// Check that the distance to the player is possible to traverse
							if(nodeCheckingNow.GetComponent<AIPathNode>().nodesToCells[i] < shortestPathSoFar){
								// If so, set the goal to that node, and set the distance to get to the cell the player is at
								goalNode = nodeCheckingNow;
								shortestPathSoFar = nodeCheckingNow.GetComponent<AIPathNode>().nodesToCells[i];
							}
						}
					}
				}
			}
			shortestPathSoFar = Mathf.Infinity;
		}
		waitToStart -= 1;
		
		/*if(!calculatatedNewRandomizeCourseVector){
			if(currentCell != null){
				randomizeCourseVector = FindRandomSpotInCurrentCell();
				calculatatedNewRandomizeCourseVector = true;
			}
		}*/
		
		if(goalNode){
			// If the node isnt open
			if(!goalNode.GetComponent<AIPathNode>().nodeOpen){
				// Loop through the cells associated with this node
				for(int i=0; i<=goalNode.GetComponent<AIPathNode>().immediateCells.Count-1; i++){
					goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().searchedNodes.Add(goalNode);
					if(currentCell != null){
						// If the goal nodes cell equals the current cell
						if(goalNode.GetComponent<AIPathNode>().immediateCells[i] == currentCell){
							// Remove the goal node from the list of nodes associated with that cell
							GameObject tempNode = goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().nodes[i+1];
							goalNode = tempNode;
							/*if(goalNode.GetComponent<AIPathNode>().immediateCells[i] != playerCell){
								tempNode = goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().nodes[i+1];
								goalNode = tempNode;
							}*/
							//goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().nodes.Remove(goalNode);
						}
					}
				}
				
				//goalNode = null;
			} else {
				// If the goal node is finally open
				if(goalNode.GetComponent<AIPathNode>().nodeOpen){
					// Loop through the cells associated with this node
					for(int i=0; i<=goalNode.GetComponent<AIPathNode>().immediateCells.Count-1; i++){
						if(goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().searchedNodes.Contains(goalNode)){
							goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().searchedNodes.Remove(goalNode);
							if(currentCell != null){
								/*if(goalNode.GetComponent<AIPathNode>().immediateCells[i] == currentCell){
									// If the list of nodes associated with the this goal node doesnt contain it
									if(!goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().nodes.Contains(goalNode)){
										// Then add it back to the list
										goalNode.GetComponent<AIPathNode>().immediateCells[i].GetComponent<AIPathCell>().nodes.Add(goalNode);
									}
								}*/
							}
						}
					}
				}
			}
		}
		
		/*if(currentCell != playerCell || playerCell == null){
			if(randomizedCourse && goalNode){
				transform.position += (goalNode.transform.position-transform.position).normalized*curMoveSpeed*Time.deltaTime;
				transform.LookAt(goalNode.transform);
			} else if(randomizedCourse && goalNode == null){
				randomizedCourse = false;
			} else {
				if(!randomizedCourse){
					transform.position += (randomizeCourseVector-transform.position).normalized*curMoveSpeed*Time.deltaTime;
					transform.LookAt(randomizeCourseVector);
					if(Vector3.Distance(transform.position, randomizeCourseVector) < transform.localScale.x){
						if(goalNode != null){
							randomizedCourse = true;
						} else {
							calculatatedNewRandomizeCourseVector = false;
						}
					}
				}
			}
		}*/
		
		if(goalNode && currentCell != playerCell){
			transform.position += (goalNode.transform.position-transform.position).normalized*curMoveSpeed*Time.deltaTime;
			transform.LookAt(goalNode.transform);
		}
		
		if(playerCell == currentCell){
			transform.position += (target.position-transform.position).normalized*curMoveSpeed*Time.deltaTime;
			transform.LookAt(target);
		}
		
		if(curMoveSpeed < maxMoveSpeed){
			curMoveSpeed += speedRecover * Time.deltaTime;
		}
		
		ClampMoveSpeed();
	}
	
	void ClampMoveSpeed(){
		curMoveSpeed = Mathf.Clamp(curMoveSpeed, minMoveSpeed, maxMoveSpeed);
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "AIPathCell"){
			currentCell = other.gameObject;
			randomizedCourse = false;
			calculatatedNewRandomizeCourseVector = false;
		}
	}
	
	void OnTriggerStay(Collider other){
		if(other.tag == "Enemy" && other.gameObject != gameObject){
			if(curMoveSpeed > minMoveSpeed){
				curMoveSpeed -= speedDamage;
			}
			transform.position += (transform.position-other.transform.position).normalized*0.1f;
		}
	}

	Vector3 FindRandomSpotInCurrentCell(){
		return currentCell.transform.position + (currentCell.transform.rotation * 
			new Vector3(Random.Range(currentCell.transform.localScale.x * -0.5f, currentCell.transform.localScale.x*0.5f),
			1, Random.Range(currentCell.transform.localScale.z*-0.5f, currentCell.transform.localScale.z*0.5f)));
	}
}
