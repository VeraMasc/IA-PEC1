using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton para contener la información de uso habitual en el juego
/// </summary>
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

    public List<GameObject> dogs;

    public List<GameObject> poops;

	void Awake()
	{
		if(singleton!=null &&  singleton != this) //Prevent duplicate singleton
		{
            Destroy(this);
            return;
        }

		_singleton=this;
	}
}
