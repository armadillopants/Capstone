using UnityEngine;
using System.Collections.Generic;

public class WaypointMaster : MonoBehaviour {
	
	public LayerMask layerToIgnore = -1; // Layer to ignore, when raycasting
	public int layerMask;
	
	public bool drawPathLines = false;
	
	class WayPoint {
		public float fScore = 0f; // Sum of g and h
		public float gScore = 0f; // The actual shortest distance traveled from initial node to current node
		public float hScore = 0f; // The estimated distance from current node to goal
		public int id = -1;
		public Vector3 pos = new Vector3();
		public WayPoint parent = null; // Parent of node
		public List<WayPoint> childNodes = new List<WayPoint>(); // List of child nodes
	}
	
	private List<WayPoint> waypointList = new List<WayPoint>(); // List of all waypoints
	private List<WayPoint> openedList = new List<WayPoint>();
	private List<WayPoint> closedList = new List<WayPoint>();
	public List<Vector3> pathList = new List<Vector3>();
	
	void Start(){
		layerMask = ~layerToIgnore.value;
		
		GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		if(waypoints != null){
			for(int i=0; i<waypoints.Length; i++){
				WayPoint waypoint = new WayPoint();
				waypoint.pos = waypoints[i].transform.position;
				waypoint.id = i;
				waypointList.Add(waypoint);
			}
		}
	}
	
	// Find waypoint nodes, that current node can see
	void FindChild(WayPoint waypoint){
		// Clear previous data first
		waypoint.childNodes.Clear();
		
		// Iterate through each node in list
		Vector3 dir;
		foreach(WayPoint node in waypointList){
			if(node.id != waypoint.id){
				dir = node.pos - waypoint.pos;
				dir.Normalize();
				if((Physics.Raycast(waypoint.pos, dir, Vector3.Distance(waypoint.pos, node.pos), layerMask)) == false){
					waypoint.childNodes.Add(node);
				}
			}
		}
	}
	
	// Clear list of child nodes
	void FreeChild(){
		foreach(WayPoint node in waypointList){
			node.childNodes.Clear();
		}
	}
	
	// Find path
	public void FindPath(Vector3 startNode, Vector3 endNode){
		int resultPath = -1;
		WayPoint tempNode = new WayPoint();
		
		// Clear previous data, before path finding
		openedList.Clear();
		closedList.Clear();
		pathList.Clear();
		
		foreach(WayPoint node in waypointList){
			node.parent = null;
		}
		
		// Add the starting location to the open list
		WayPoint startLoc = new WayPoint();
		startLoc.hScore = Vector3.Distance(startNode, endNode);
		startLoc.fScore = startLoc.hScore;
		startLoc.pos = startNode;
		openedList.Add(startLoc);
		
		// Check to see if we can trace ray to the destination node without path finding
		Vector3 dir = endNode - startNode;
		dir.Normalize();
		if((Physics.Raycast(startNode, dir, Vector3.Distance(startNode, endNode), layerMask)) == false){
			// Add start location
			pathList.Add(startNode);
			// Add end location
			pathList.Add(endNode);
			
			// Path is found
			resultPath = 1;
		}
		
		// Repeat until path is found, or it doesnt exist
		if(resultPath == -1){
			// If opened list isnt empty
			while(openedList.Count != 0){
				// Find lowest f score in opened list
				float f = openedList[0].fScore;
				int index = 0;
				
				for(int i=0; i<openedList.Count; i++){
					if(openedList[i].fScore < f){
						f = openedList[i].fScore;
						index = i;
					}
				}
				
				// If current node already has list of children, do nothing
				if(openedList[index].childNodes.Count == 0){
					FindChild(openedList[index]);
				}
				
				// Add current node to closed list
				closedList.Add(openedList[index]);
				// And remove it from the opened list
				openedList.RemoveAt(index);
				
				// Check all child nodes of node we currently added to closed list
				WayPoint lastNode = closedList[closedList.Count-1];
				foreach(WayPoint child in lastNode.childNodes){
					int skip = 0;
					
					// If current node in the closed list skip this loop cycle
					foreach(WayPoint node in closedList){
						if(child.id == node.id){
							skip = 1;
							//break;
						}
					}
					
					// Should we skip this loop cycle?
					if(skip == 0){
						// If this node is already in opened list, check to see if this path to that node is better
						// If the G score for that node is lower if we use the current node to get there
						foreach(WayPoint node in openedList){
							if(child.id == node.id){
								skip = 1;
								float dist = Vector3.Distance(lastNode.pos, child.pos);
								if((lastNode.gScore + dist) < child.gScore){
									// If new G score is better, recalculate F and G scores for this node
									// And change parent of this node
									child.parent = lastNode;
									child.gScore = lastNode.gScore + dist;
									child.hScore = Vector3.Distance(child.pos, endNode);
									child.fScore = child.gScore + child.hScore;
								}
							}
						}
						
						// If current node istn in the opened list, add it there
						if(skip == 0){
							child.parent = lastNode;
							child.gScore = lastNode.gScore + Vector3.Distance(lastNode.pos, child.pos);
							child.hScore = Vector3.Distance(child.pos, endNode);
							child.fScore = child.gScore + child.hScore;
							openedList.Add(child);
						}
						
						// If current node can see target
						dir = endNode - child.pos;
						dir.Normalize();
						if((Physics.Raycast(child.pos, dir, Vector3.Distance(child.pos, endNode), layerMask)) == false){
							//resultPath = 1;
							// Remember this node
							if(tempNode == null){
								tempNode = child;
							} else {
								// If another child node has lower f score
								if(child.fScore < tempNode.fScore){
									tempNode = child;
								}
							}
						}
					}
				}
			}
			
			// If there was any node, which was able to see end node
			if(tempNode != null){
				// Add end node to the path list
				pathList.Add(endNode);
				
				// Start from the end node, go from each node to its
				// Parent node until we reach the start node
				WayPoint nextNode = tempNode;
				while(nextNode != null){//.pos != startNode){
					pathList.Add(nextNode.pos);
					nextNode = nextNode.parent;
				}
				
				//pathList.Add(startNode);
				//pathList.Reverse();
				resultPath = 1;
			} else {
				// Path doesnt exist
				resultPath = 0;
			}
		}
	}
	
	void OnDrawGizmos(){
		if(pathList.Count != 0 && drawPathLines){
			Gizmos.color = Color.green;
			for(int i=0; i<pathList.Count-1; i++){
				Gizmos.DrawLine(pathList[i], pathList[i+1]);
			}
		}
	}
}