using UnityEngine;
using UnityEngine.AI;
using System;

/// <summary>
/// Estructura base de un controlador de una IA
/// </summary>
public class AIController: MonoBehaviour {
	/// <summary>
    /// Gameobject que actua como "cámara" del enemigo
    /// </summary>
	public Transform viewpoint;

	/// <summary>
    /// Layermask de los objetos que la IA considera opacos
    /// </summary>
	public LayerMask wallMask;

	/// <summary>
    /// Agente de Navmesh del NPC
    /// </summary>
	public NavMeshAgent agent;

	public bool pathEndReached {
		get => !agent.hasPath;
	}


	/// <summary>
    /// Ejecuta la muerte del enemigo
    /// </summary>
	public virtual void OnDeath(){
		Destroy(gameObject);
	}

	
	public virtual void OnHurt(){
		
	}

	public virtual void tryAttack(Transform target){
		
	}

	[NonSerialized]
	public Animator animator;

	
}
