using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Estados posibles de la máquina de estados de los "Abuelos"
/// </summary>
public enum GrandpaState{
	/// <summary>
	/// Deambulando
	/// </summary>
	wander,
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
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		switch(state){
			case GrandpaState.wander:
				wander(); break;

			case GrandpaState.rest:
				rest(); break;
		}

		state = getNextState();
    }


	public void wander(){
		if(!agent.hasPath){
			
			var randomVector = Quaternion.FromToRotation(Vector3.forward,Vector3.up) * (Vector3)(Random.insideUnitCircle * wanderRange);
			agent.destination =  randomVector + transform.position;		
			
		}
			
	}
	public void rest(){

	}

	/// <summary>
	/// Calcula el siguiente estado de la Máquina de Estados Finitos
	/// </summary>
	public GrandpaState getNextState(){
		return GrandpaState.wander;
	}
}
