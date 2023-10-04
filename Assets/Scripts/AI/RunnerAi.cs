using System;
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
	public float targetDistance => (targetPoint.position - transform.position).magnitude;

	/// <summary>
	/// Punto secundario usado para suavizar el movimiento
	/// </summary>
	public Transform secondaryPoint;


	/// <summary>
	/// Proximidad a targetPoint a partir de la que se aplica el suavizado
	/// </summary>
	public float smoothingDist = 1f;

	/// <summary>
	/// Calculated target using the smoothing process
	/// </summary>
	public Vector3? effectiveTarget {
		get {
			if (targetPoint == null || secondaryPoint == null){
				return null;
			}
			//var lerpFactor = MathF.Pow(2,-(targetDistance -smoothingDist)* smoothingFactor) ;
			var lerpFactor = (smoothingDist -targetDistance) / (targetPoint.position - secondaryPoint.position).magnitude;
			return Vector3.Lerp(targetPoint.position, secondaryPoint.position, lerpFactor);
		}
	}
	

	/// <summary>
	/// Índice del waypoint objetivo
	/// </summary>
	public int targetIndex;

	public float stepSize = 4f;

	/// <summary>
	/// Factor de atracción del punto objetivo
	/// </summary>
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
		if (effectiveTarget == null)
			return;
		var diff = (Vector3)effectiveTarget - transform.position;
		var step = Vector3.ClampMagnitude(diff.normalized * stepSize, diff.magnitude);
		agent.destination = transform.position + step;
		//agent.destination = targetPoint.position;
	}

	/// <summary>
	/// Cambia de target point si ya se ha llegado al actual
	/// </summary>
	public void updateTargetIfNeeded(){
		if ( targetPoint !=null && ((Vector3)effectiveTarget - transform.position).magnitude > 4)
			return;
		
		(targetPoint, targetIndex) = Waypoints.singleton.getNextPoint(targetIndex,1);
		(secondaryPoint,_) = Waypoints.singleton.getNextPoint(targetIndex,1);
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		if(effectiveTarget != null)
			Gizmos.DrawCube((Vector3)effectiveTarget, Vector3.one);
	}
}
