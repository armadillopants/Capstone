
var thePath : GameObject;	// Waypoint master GameObject 
var moveSpeed : float;
var turnSpeed : float;
var gravity : float;
var MinDistance : float = 0.0;
var RayRate : float;

private var master : WaypointMaster; 		// waypoint master component
private var PathToFollow = new Array();
private var contr : CharacterController;
private var corotine : boolean = true;
private var seeDest : boolean = true;

private var goTo : Vector3;	// Go to position
var followObj : GameObject = null;	// Game object to follow for
private var nearDest : boolean = false; // If actor near followObj
private var lastPos : Vector3;	// Check if actor isn't stuck
private var moveDirection : Vector3;

function Start()
{
	//followObj = GameObject.FindWithTag("Player");
	//thePath = GameObject.Find("WaypointMaster");
	master = thePath.GetComponent(WaypointMaster);
	contr = GetComponent(CharacterController);
	
	if(moveSpeed == 0)
		moveSpeed = 1;
	if(turnSpeed == 0)
		turnSpeed = 1;
	if(gravity == 0)
		gravity = 9.8;
	
	if(!master)
		Debug.Log("WaypointMaster is not defined");
	if(!contr)
		Debug.Log("CharacterController is not found");
}

function Update () 
{
	if(master)
	{
		// Call functions only fixed times per second
		if(corotine)
		{
			
			StartCoroutine(LookForDest());
			StartCoroutine(FindPath());
			StartCoroutine(checkLastPos());
		}
		
		// Move actor along path
		if(PathToFollow.length != 0 && !nearDest)
		{
			if(followObj)
				PathToFollow[PathToFollow.length-1] = followObj.transform.position;
			else
				PathToFollow[PathToFollow.length-1] = goTo;
				
			var pathPos : Vector3 = PathToFollow[0];			
			var dir = pathPos - transform.position;
			var rot = Quaternion.LookRotation(dir);
			transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime * turnSpeed); 
			transform.rotation.x=0;
			transform.rotation.z=0;
			dir.Normalize();
			
			if(contr.isGrounded)
				moveDirection = transform.forward * (moveSpeed / 100);
		
			if(Vector3.Distance(transform.position, PathToFollow[0]) < MinDistance)
				PathToFollow.RemoveAt(0);
		
		}
		
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		// Move the controller			
		contr.Move(moveDirection);
	}		
}

// Move to goTo position
function GoTo(dest : Vector3)
{
	goTo = dest;
	
	// Try to find path
	PathToFollow.clear();
	master.FindPath(transform.position, dest);
	if(master.pathList.length != 0)
	{
		print("Path is found");
		for (var node : Vector3 in master.pathList)
			PathToFollow.Push(node);
	}
}

private function FindPath()
{
	corotine = false;
	
	if(!seeDest)
	{
		if(followObj)
			goTo = followObj.transform.position;
		print("FindPath called");
		PathToFollow.clear();
		master.FindPath(transform.position, goTo);
		if(master.pathList.length != 0)
		{
			for (var node : Vector3 in master.pathList)
				PathToFollow.Push(node);
		}
	}
	
	yield WaitForSeconds (RayRate);
	corotine = true;
}

// Check to see if actor can see destination
private function LookForDest()
{
	corotine = false;
	
	if(followObj)
	{
		goTo = followObj.transform.position;
		
		var followCollider : Collider = followObj.GetComponent(Collider);
		nearDest = false;
		if(followCollider)
		{
			var boundBox = Vector3.Distance(followCollider.bounds.max, followCollider.bounds.center);
			if(Vector3.Distance(transform.position, goTo) <= (contr.radius+boundBox+ MinDistance*2))
				nearDest = true;
		}
	}
	
	if(PathToFollow.length != 0 || (followObj && !nearDest))
	{
		var tempGameObj = new GameObject();
		seeDest = false;
			
		if(PathToFollow.length > 1)
			tempGameObj.transform.position = PathToFollow[PathToFollow.length-2];
		else
			tempGameObj.transform.position = transform.position;
		
		tempGameObj.transform.LookAt(goTo);
		var dist = Vector3.Distance(tempGameObj.transform.position, goTo);
		if((Physics.Raycast(tempGameObj.transform.TransformPoint(contr.radius, 0, 0), tempGameObj.transform.forward, dist, master.layerMask)) == false)
		{
			if((Physics.Raycast(tempGameObj.transform.TransformPoint(-contr.radius, 0, 0), tempGameObj.transform.forward, dist, master.layerMask)) == false)
				seeDest = true;
		}
		
		// Chect to see if actor can see shorter path to the next node of PathToFollow list
		if(PathToFollow.length > 1)
		{
			tempGameObj.transform.position = transform.position;
			tempGameObj.transform.LookAt(PathToFollow[1]);
			dist = Vector3.Distance(transform.position, PathToFollow[1]);
			if((Physics.Raycast(tempGameObj.transform.TransformPoint(contr.radius, 0, 0), tempGameObj.transform.forward, dist, master.layerMask)) == false)
			{
				if((Physics.Raycast(tempGameObj.transform.TransformPoint(-contr.radius, 0, 0), tempGameObj.transform.forward, dist, master.layerMask)) == false)
				{
					PathToFollow.RemoveAt(0);
				}
			}
		}
		
		Destroy(tempGameObj);
	}
	
	yield WaitForSeconds (RayRate);
	corotine = true;
}

// Check if actor is not stuck somewhere
private function checkLastPos()
{
	corotine = false;
	
	if((Vector3.Distance(lastPos, transform.position) < 0.6) && !nearDest && PathToFollow.length != 0)
	{
		seeDest = false;
		FindPath();
		print("checkLastPos() called");
	}
		
	lastPos = transform.position;
	
	yield WaitForSeconds (RayRate);
	corotine = true;
}

@script RequireComponent(CharacterController)