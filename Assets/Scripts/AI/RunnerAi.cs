using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
	/// Sentido en el que sigue el recorrido
	/// </summary>
	public int pathDir;



	/// <summary>
	/// Retraso entre el agente y el objeto que le sigue
	/// </summary>
	public float agentDelay=2f;

	/// <summary>
	/// Rango de variación de la velocidad
	/// </summary>
	public float randomSpeed = 1f;

	


	


	public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
		agent.updatePosition = false;

		//Velocidad aleatoria
		var halfRspeed = randomSpeed / 2;
		agent.speed += Random.Range(-halfRspeed, halfRspeed);
		pathDir = Random.value < 0.5f? 1 : -1;

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

	/// <summary>
	/// Actualiza la posición para que siga al agente desde una cierta distancia
	/// </summary>
	public void followAgent(){
		var pos =agent.nextPosition;
		var diff = pos-transform.position;
		
		if(diff.magnitude >=agentDelay){
			transform.position += diff.normalized * (diff.magnitude-agentDelay);
		}
	}

	/// <summary>
	/// Inicializa la posición del agente a cierta distancia para que no haya delay en el movimiento
	/// </summary>
	private void setAgentPos(){
		var diff = targetPoint.position-transform.position;
		var offsetPos = transform.position + diff.normalized * agentDelay;
		agent.nextPosition = offsetPos;
	}

	/// <summary>
	/// Abandona el path previo (si lo hay) y genera uno nuevo
	/// </summary>
	public void regeneratePath(){
		getNextTarget();

		if (targetPoint == null)
			return;

		agent.destination = targetPoint.position;
	}

	/// <summary>
	/// Cambia de target point si ya se ha llegado al actual
	/// </summary>
	public void getNextTarget(){
		if(targetPoint == null){
			(targetPoint, targetIndex) = Waypoints.singleton.getNextPoint(targetIndex, pathDir==1? 0:-1);
			setAgentPos();
			return;
		}

		(targetPoint, targetIndex) = Waypoints.singleton.getNextPoint(targetIndex,pathDir);
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		
		Gizmos.DrawCube((Vector3)agent.nextPosition, Vector3.one);
	}
}
