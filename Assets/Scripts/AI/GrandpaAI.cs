using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

/// <summary>
/// Estados posibles de la máquina de estados de los "Abuelos"
/// </summary>
public enum GrandpaState{
	/// <summary>
	/// Deambulando
	/// </summary>
	wander,
	/// <summary>
	/// Yendo a descansar en un banco
	/// </summary>
	goToBench,
	/// <summary>
	/// Descansando en un banco
	/// </summary>
	rest
}

/// <summary>
/// Script encargado de gestionar a los "Abuelos"
/// </summary>
public class GrandpaAI : MonoBehaviour
{
	/// <summary>
	/// Estado actual
	/// </summary>
	public GrandpaState state;

	/// <summary>
	/// Navmesh agent que controla al abuelo
	/// </summary>
	public NavMeshAgent agent;

	/// <summary>
	/// Rango máximo en el que puede deambular cada vez
	/// </summary>
	public float wanderRange = 8f;

	/// <summary>
	/// Rango en el que detectan los bancos
	/// </summary>
	public float detectionRange = 2f;

	/// <summary>
	/// Segundos que pasa descansando en un banco
	/// </summary>
	public float restTime = 3f;

	/// <summary>
	/// Contador del tiempo que queda de descanso
	/// </summary>
	public float restRemaining ;

	/// <summary>
	/// Mask del navmesh area de "banco"
	/// </summary>
	int benchMask;

	int defaultMask;

	int benchLayer;
    // Start is called before the first frame update
    void Start()
    {
		benchMask = 1 << NavMesh.GetAreaFromName("Bench");
		defaultMask = agent.areaMask;
		benchLayer = LayerMask.GetMask("Bench");
    }

    // Update is called once per frame
    void Update()
    {
		executeState();
		
    }

	/// <summary>
	/// Ejecuta el estado actual de la FSM
	/// </summary>
	private void executeState()
	{
		//Cambiar de estado automáticamente al terminar el path actual
		if(stateNeedsUpdate())
			state = getNextState();

		switch(state){
			case GrandpaState.wander:
				wander(); break;

			case GrandpaState.rest:
				rest(); break;
		}

		
	}

	/// <summary>
	/// Ejecuta el estado de deambular
	/// </summary>
	public void wander(){
		if (agent.hasPath)
			return;
		
		var randomVector = Quaternion.FromToRotation(Vector3.forward,Vector3.up) * (Vector3)(Random.insideUnitCircle * wanderRange);
		randomVector = Vector3.Max(randomVector, randomVector.normalized*2); //Minimum 1 unit movement
		agent.destination =  randomVector + transform.position;		
		
		
			
	}

	/// <summary>
	/// Ejecuta el estado de descanso
	/// </summary>
	public void rest(){
		restRemaining -= Time.deltaTime;
	}

	/// <summary>
	/// Calcula el siguiente estado de la Máquina de Estados Finitos
	/// </summary>
	public GrandpaState getNextState(){
		var newState = GrandpaState.wander; //Por defecto volverá a wander

		
		
		if(state == GrandpaState.goToBench){
			agent.SamplePathPosition(benchMask, 0.1f, out NavMeshHit hit);
			if (hit.mask == benchMask)
				newState = GrandpaState.rest;
			
		} else if( state == GrandpaState.wander){
			var hits = Physics.OverlapSphere(transform.position, detectionRange, benchLayer);

			var closest = hits.Select(hit => 
				new { hit, dist=(hit.transform.position - transform.position).magnitude}//Calculate dist
			).DefaultIfEmpty()
			.Aggregate((curMin, x) => curMin == null || x.dist  < curMin.dist ? x : curMin);

			if(closest != null){
				Debug.Log("Bench Hit");
				agent.destination = closest.hit.transform.position;
				newState = GrandpaState.goToBench;
			}

		} 

		if (state != newState)
			initializeState(newState);
		return newState;
	}

	bool stateNeedsUpdate(){
		if (!agent.hasPath && state != GrandpaState.rest)
			return true;

		if(state == GrandpaState.goToBench && agent.remainingDistance <1f){
			return true;
		}

		if(state == GrandpaState.rest && restRemaining <0){
			return true;
		}

		return false;
	}

	void initializeState(GrandpaState newState){
		if (newState == GrandpaState.rest){
			Debug.Log("BenchMask");
			agent.areaMask = benchMask; //Force to stay in bench
			restRemaining = restTime;
			agent.ResetPath();
		}else if(state == GrandpaState.rest){
			agent.areaMask = defaultMask; //Allow to leave bench
		}
	}

}
