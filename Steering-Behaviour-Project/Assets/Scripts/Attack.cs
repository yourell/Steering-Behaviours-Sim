using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
	
	public Vector3 force; //applied in any direction
	public Vector3 velocity; //direction 
	public float mass = 1f; //Objects weight
	public float maxSpeed = 5f; //Top speed
	public GameObject target; //Target
	public bool seekEnabled, fleeEnabled;
	public float distanceDefender;
	public float distanceFinish;
	Transform defender;
	Transform finish;
	
	void Start()
	{
		
		defender = GameObject.FindWithTag("Defender").transform;
		finish = GameObject.FindWithTag("Finish").transform;
	}
	
	void Update () 
	{
		//Works out the distance between the scarecrow and the birds or crop
		distanceDefender = Vector3.Distance(defender.transform.position, transform.position);
		distanceFinish = Vector3.Distance(finish.transform.position, transform.position);
		//if defender gets too close changes seeking finsh to flee from defender
		if (distanceDefender <= 5f)
		{
			target = GameObject.FindWithTag("Defender");
			seekEnabled = false;
			fleeEnabled = true;
		}
		//otherwise if the scarecrow is far enough away stops flee and changes to seek on finish
		else if (distanceDefender >= 20f)
		{
			target = GameObject.FindWithTag("Finish");
			fleeEnabled = false;
			seekEnabled = true;
		}
		ForceIntegrator();
	}
	
	void ForceIntegrator()
	{
		Vector3 accel = force / mass; 
		velocity = velocity + accel * Time.deltaTime; 
		transform.position = transform.position + velocity * Time.deltaTime; 
		force = Vector3.zero;
		if(velocity.magnitude > float.Epsilon) 
		{
			transform.forward = Vector3.Normalize(velocity); 
		}
		velocity *= 1f;  
		if(seekEnabled)
		{
			force += Seek(target.transform.position); 
		}
		if(fleeEnabled)
		{
			force += Flee (target.transform.position);
		}
	}
	
	Vector3 Seek(Vector3 target) //This searches for a target
	{
		Vector3 desiredVel = target - transform.position; 
		desiredVel.Normalize(); 
		desiredVel *= maxSpeed; 
		return desiredVel - velocity; 
	}
	
	Vector3 Flee(Vector3 target) 
	{
		Vector3 desiredVel = target - transform.position; 
		float distance = desiredVel.magnitude; 
		if(distance < 12f)
		{
			desiredVel.Normalize();
			desiredVel *= maxSpeed;
			return velocity - desiredVel; 
		}
		else 
		{
			return Vector3.zero; 
		}
	}
	
	
}