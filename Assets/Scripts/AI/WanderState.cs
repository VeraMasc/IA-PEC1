using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Se encarga de gestionar el estado en el que la IA deambula
/// </summary>
/// 

public class WanderState : AIState
{

	/// <summary>
    /// Tags de los tipos a detectar
    /// </summary>
	public string[] targetTags;
	/// <summary>
	/// Estado al que cambiará al detectar a entidades del tipo a buscar
	/// </summary>
	public AIState OnDetectTarget;

	

	/// <summary>
    /// Radio en el que el enemigo deambula
    /// </summary>
	public float wanderRange = 2f;

	/// <summary>
    /// Tiempo que espera tras llegar a su destino antes de cambiarlo de nuevo
    /// </summary>
	public RandomBetween wanderWait = new RandomBetween(1f,2f);


	/// <summary>
    /// Tiempo de espera restante antes de cambiar de destino
    /// </summary>
	public float wanderTimer;
	
	private void OnEnable() {
		wanderAway();

	}
	private void Update() {
		wanderAway();

		// //detectPlayer();
		// var detected = senses.hearEntityTagged(controller.viewpoint, targetTags).Length > 0
		// 	|| senses.seeEntityTagged(controller.viewpoint, controller.wallMask, targetTags).Length > 0;
		// if(detected)
		// 	switchState(OnDetectTarget);

	}
	

	public virtual void wanderAway(){
		if(controller.pathEndReached){
			if(wanderTimer <= 0){
				var randomVector = Quaternion.FromToRotation(Vector3.forward,Vector3.up) * (Vector3)(Random.insideUnitCircle * wanderRange);
				controller.agent.destination =  randomVector + transform.position;

				wanderTimer = wanderWait;
			}
			else{
				wanderTimer -= Time.deltaTime;
			}
			
		}
			
	}
	
	/// <summary>
    /// Se activa cuando el enemigo recibe daño
    /// </summary>
	public void OnHurt(){
		if(enabled)
			switchState(OnDetectTarget);
	}
}
