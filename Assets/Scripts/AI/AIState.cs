using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Clase padre que engloba los aspectos que tienen en común los estados de la IA
/// </summary>
public class AIState : MonoBehaviour
{


	/// <summary>
    /// Cambia de un estado a otro
    /// </summary>
    /// <param name="targetState"></param>
	public void switchState(AIState targetState){
		if(targetState == null)
			return;
		this.enabled = false;
		targetState.enabled = true;
	}

	protected AIController controller;

	private void Awake() {
		controller = GetComponent<AIController>();
	}

	public void OnDeath(){
		Destroy(this);
	}

}
