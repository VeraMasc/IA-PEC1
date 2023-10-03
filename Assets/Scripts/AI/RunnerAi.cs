using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunnerAi : MonoBehaviour
{
	public Vector2 velocity;

	public float noise;

	public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        if(!agent.hasPath || agent.remainingDistance <= 0.1){
			if (agent.hasPath)
				velocity = Quaternion.FromToRotation(Vector3.up, Vector3.forward) * agent.desiredVelocity;
			velocity = velocity +(Random.insideUnitCircle * noise) ;
			agent.destination =  transform.position + (Quaternion.FromToRotation(Vector3.forward, Vector3.up) * (Vector3)velocity*2);
			agent.speed = velocity.magnitude;
				
		}
    }

	
}
