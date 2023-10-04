using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script encargado de gestionar a los runners
/// </summary>
public class RunnerAi : MonoBehaviour
{
	/// <summary>
	/// Punto objetivo hacia el que ha de ir
	/// </summary>
	public Transform targetPoint;

	/// <summary>
	/// Punto secundario usado para suavizar el movimiento
	/// </summary>
	public Transform secondaryPoint;

	public Vector3 effectiveTarget => targetPoint.position;
	

	/// <summary>
	/// Índice del waypoint objetivo
	/// </summary>
	public int targetIndex;

	//Factor de atracción del punto objetivo
	public float targetAttraction;
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
        if(!agent.hasPath || agent.remainingDistance <= 1  ){
			regeneratePath();
		}
    }

	/// <summary>
	/// Abandona el path previo (si lo hay) y genera uno nuevo
	/// </summary>
	public void regeneratePath(){
		updateTargetIfNeeded();
		// if (agent.hasPath)
		// 		velocity = Quaternion.FromToRotation(Vector3.up, Vector3.forward) * agent.desiredVelocity;
		// Vector2 targetDir = Quaternion.FromToRotation(Vector3.up, Vector3.forward)* (targetPoint.position - transform.position).normalized;
		// velocity = velocity+ (targetDir * targetAttraction) +(Random.insideUnitCircle * noise) ;
		// agent.destination =  transform.position + (Quaternion.FromToRotation(Vector3.forward, Vector3.up) * (Vector3)velocity*2);
		// agent.speed = velocity.magnitude;
		agent.destination = targetPoint.position;
	}

	/// <summary>
	/// Cambia de target point si ya se ha llegado al actual
	/// </summary>
	public void updateTargetIfNeeded(){
		if ((targetPoint.position - transform.position).magnitude > 2)
			return;

		(targetPoint, targetIndex) = Waypoints.singleton.getNextPoint(targetIndex,1);
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(effectiveTarget, Vector3.one);
	}
}
