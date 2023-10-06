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

	/// <summary>
	/// Índice del waypoint objetivo
	/// </summary>
	public int targetIndex;



	/// <summary>
	/// Retraso entre el agente y el objeto que le sigue
	/// </summary>
	public float agentDelay=2f;


	


	


	public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
		agent.updatePosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!agent.hasPath || agent.remainingDistance <= 1  ){
			regeneratePath();
		}
		else{
			followAgent();
		}
    }

	public void followAgent(){
		var pos =agent.nextPosition;
		var diff = pos-transform.position;
		
		if(diff.magnitude >=agentDelay){
			transform.position += diff.normalized * (diff.magnitude-agentDelay);
		}
	}

	/// <summary>
	/// Abandona el path previo (si lo hay) y genera uno nuevo
	/// </summary>
	public void regeneratePath(){
		updateTargetIfNeeded();

		if (targetPoint == null)
			return;

		agent.destination = targetPoint.position;
	}

	/// <summary>
	/// Cambia de target point si ya se ha llegado al actual
	/// </summary>
	public void updateTargetIfNeeded(){


		(targetPoint, targetIndex) = Waypoints.singleton.getNextPoint(targetIndex,1);
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		
		Gizmos.DrawCube((Vector3)agent.nextPosition, Vector3.one);
	}
}
