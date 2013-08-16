#pragma strict

var Player : GameObject;
var Follower : GameObject;
var waypMaster : GameObject;

private var playerActor : WaypointActor;
private var followerActor : WaypointActor;
private var master : WaypointMaster; 

private var heightPoint : float;

function Start()
{
	if(!Player)
		Debug.Log("Player is not defined");
	if(!Follower)
		Debug.Log("Follower is not defined");
	if(!waypMaster)
		Debug.Log("WaypointMaster is not defined");
		
	if(Player && Follower && waypMaster)
	{
		playerActor = Player.GetComponent(WaypointActor);
		followerActor = Follower.GetComponent(WaypointActor);
		master = waypMaster.GetComponent(WaypointMaster);
		
		var contr : CharacterController = Player.GetComponent(CharacterController);
		heightPoint = contr.height / 2;
	}
}

function Update () 
{
	if(Input.GetMouseButtonDown(0))
	{
		// Find clicked point in 3d space
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		var hit : RaycastHit; 
		if (Physics.Raycast (ray, hit, 100)) 
		{
			var goTo = hit.point;
			goTo = goTo + hit.normal * heightPoint;
			playerActor.GoTo(goTo);
		} 
	}
}