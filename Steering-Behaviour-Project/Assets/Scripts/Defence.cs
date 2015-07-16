using UnityEngine;
using System.Collections;

public class Defence : MonoBehaviour {

	public Vector3 force; //can be applied in any direction
	public Vector3 velocity; //direction 
	public float mass = 1f; 
	public float maxSpeed = 5f; // Top speed
	public GameObject target; //Targert
	public bool pathFollowEnabled, seekEnabled, pursueEnabled, arriveEnabled; //toggle which steering behaviour you want
	public Vector3 offsetPursuitOffset;
	public Transform[] waypoints;//For measuring the distance between the defender and the attackers and also the finish
	Transform attacker;
	Transform finish;
	public float distanceAttacker = 10;
	public float distanceFinish = 10;
	//For using the path script
	Path path = new Path();
	void Start () {
		//
		path.CreatePath();
		//
		attacker = GameObject.FindWithTag("Crow").transform;
		finish = GameObject.FindWithTag("Finish").transform;
	}
	
	void Update () {
		//
		distanceAttacker = Vector3.Distance(attacker.transform.position, transform.position);
		distanceFinish = Vector3.Distance(finish.transform.position, transform.position);
		//
		pathFollowEnabled = true;
		//
		if (distanceAttacker <= 10f)
		{
			target = GameObject.FindWithTag("Crow");
			pathFollowEnabled = false;
			pursueEnabled = true;
		}
		
		if (distanceFinish >= 10f || distanceAttacker >= 10f)
		{
			target = GameObject.FindWithTag("Finish");
			pursueEnabled = false;
			pathFollowEnabled = true;
		}
		//calls forceintegrator
		ForceIntegrator();
	}
	
	void ForceIntegrator()
	{
		Vector3 accel = force / mass; //
		velocity = velocity + accel * Time.deltaTime; //
		transform.position = transform.position + velocity * Time.deltaTime; //
		force = Vector3.zero; //
		if(velocity.magnitude > float.Epsilon) //
		{
			transform.forward = Vector3.Normalize(velocity); 
		}
		velocity *= 0.99f; 
		if(arriveEnabled)
		{
			force += Arrive (target.transform.position);
		}
		if(pursueEnabled)
		{
			force += Pursue(target); 
		}
		if(seekEnabled)
		{
			force += Seek(target.transform.position); 
		}
		if(pathFollowEnabled)
		{
			force += PathFollow ();
		}
	}
	
	Vector3 Arrive(Vector3 targetPos) 
	{
		Vector3 toTarget = targetPos - transform.position;
		float distance = toTarget.magnitude;
		if(distance <= 1f)
		{
			return Vector3.zero;
		}
		float slowingDistance = 8.0f; 
		float decelerateTweaker = maxSpeed / 10f; 
		float rampedSpeed = maxSpeed * (distance / slowingDistance * decelerateTweaker);  
		float newSpeed = Mathf.Min (rampedSpeed, maxSpeed); 
		Vector3 desiredVel = newSpeed * toTarget.normalized; 
		return desiredVel - velocity; 
	}
	
	Vector3 Pursue(GameObject target) 
	{
		Vector3 desiredVel = target.transform.position - transform.position; 
		float distance = desiredVel.magnitude; 
		float lookAhead = distance / maxSpeed; 
		Vector3 desPos = target.transform.position+(lookAhead * target.GetComponent<Attack>().velocity);  
		return Seek (desPos); 
	}
	Vector3 Seek(Vector3 target) 
	{
		Vector3 desiredVel = target - transform.position; 
		desiredVel.Normalize(); 
		desiredVel *= maxSpeed; 
		return desiredVel - velocity; 
	}
	Vector3 PathFollow()
	{
		float distance = (transform.position - path.NextWaypoint()).magnitude;
		if(distance < 0.5f)
		{
			path.AdvanceWaypoint();
		}
		if(!path.looped && path.IsLastCheckpoint())
		{
			return Arrive (path.NextWaypoint());
		}
		else
		{
			return Seek (path.NextWaypoint());
		}
	}
}
