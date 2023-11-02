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
	rest,
	/// <summary>
	/// Yendo a descansar en un banco
	/// </summary>
	leaveBench,
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
	/// Veces que se añade la velocidad actual al cálculo de la posición futura.
	/// </summary>
	public float wanderInertia = 2f;
	/// <summary>
	/// Rango mínimo en el que puede deambular cada vez
	/// </summary>
	public float minWanderRange = 5f;
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

	/// <summary>
	/// Navmesh mask por defecto del agente
	/// </summary>
	int defaultMask;

	/// <summary>
	/// Layer en la que se encuentra el bench (se usa para detectarlos)
	/// </summary>
	int benchLayer;

	public RandomBetweenInt avoidance = new RandomBetweenInt(0, 50);
    // Start is called before the first frame update
    void Start()
    {
		benchMask = 1 << NavMesh.GetAreaFromName("Bench");
		defaultMask = agent.areaMask;
		benchLayer = LayerMask.GetMask("Bench");
		agent.avoidancePriority = avoidance.value;
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
			case GrandpaState.leaveBench:
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
		if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
			return;
		
		var randomVector = Quaternion.FromToRotation(Vector3.forward,Vector3.up) * (Vector3)(Random.insideUnitCircle * wanderRange) + agent.velocity * wanderInertia;
		var minVector = randomVector.normalized * minWanderRange;
		if(minVector.magnitude > randomVector.magnitude) //Minimum movement
			randomVector = minVector; 
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

			var closest = benchNearby(detectionRange);

			if(closest != null){
				agent.destination = closest.hit.transform.position;
				newState = GrandpaState.goToBench;
			}

		} else if(state == GrandpaState.rest){
			newState = GrandpaState.leaveBench;

		} else if (state == GrandpaState.leaveBench){
			var closest = benchNearby(detectionRange/2);
			if (closest != null)
				newState = GrandpaState.leaveBench;
		}

		if (state != newState)
			initializeState(newState);
		return newState;
	}

	bool stateNeedsUpdate(){
		if ((!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance) && state != GrandpaState.rest)
			return true;

		if(state == GrandpaState.goToBench && agent.remainingDistance <1f){
			return true;
		}

		if(state == GrandpaState.rest && restRemaining <0){
			return true;
		}

		return false;
	}

	public ClosestBench benchNearby(float range){
		var hits = Physics.OverlapSphere(transform.position, range, benchLayer);

		var closest = hits.Select(hit => 
			new ClosestBench{ hit=hit, dist=(hit.transform.position - transform.position).magnitude}//Calculate dist
		).DefaultIfEmpty()
		.Aggregate((curMin, x) => curMin == null || x.dist  < curMin.dist ? x : curMin);

		if(closest != null){
			Debug.Log("Bench Hit");
		}

		return closest;
	}

	void initializeState(GrandpaState newState){
		if (newState == GrandpaState.rest){
			Debug.Log("BenchMask");
			agent.areaMask = benchMask; //Force to stay in bench
			restRemaining = restTime;
			agent.ResetPath();
		}else{

			if(newState == GrandpaState.leaveBench){
				agent.areaMask = defaultMask; //Allow to leave bench
					
			}

		}
		
	}

}

public class ClosestBench{
	public Collider hit;
	public float dist;
}