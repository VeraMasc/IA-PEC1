using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton para contener la información de uso habitual en el juego
/// </summary>
[ExecuteInEditMode]
public class Controller : MonoBehaviour
{

	private static Controller _singleton;
	///<summary>Controller Singleton</summary>
	public static Controller singleton
	{
		get 
		{
			if (_singleton == null)
			{
				_singleton = FindObjectOfType<Controller>(); //Para cuando el maldito hotreload me pierde la referencia
			}
			return _singleton;
		}
	}

    public List<BehaviorExecutor> dogs;

    public List<Poop> poops;

	void Awake()
	{
		if(singleton!=null &&  singleton != this) //Prevent duplicate singleton
		{
            Destroy(this);
            return;
        }

		_singleton=this;
	}

    void Update()
    {
        if (Application.isPlaying)
            return;

        //Busca automáticamente todos los perros cada vez que cambias la escena
        dogs = GameObject.FindGameObjectsWithTag("Dog")
            ?.Select(o => o.GetComponentInParent<BehaviorExecutor>())
            .ToList();
    }
}
