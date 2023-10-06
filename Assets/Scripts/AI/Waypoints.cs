using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton que controla los wayponts del recorrido de los runners
/// </summary>
public class Waypoints : MonoBehaviour
{

	public Transform[] waypointsList;

	private static Waypoints _singleton;
	///<summary>GameManager Singleton</summary>
	public static Waypoints singleton
	{
		get 
		{
			if (_singleton == null)
			{
				_singleton = FindObjectOfType<Waypoints>(); //Para cuando el maldito hotreload me pierde la referencia
			}
			return _singleton;
		}
	}

	void Awake()
	{
		if(singleton!=null &&  singleton != this) //Prevent duplicate singleton
		{
            Destroy(this);
            return;
        }

		_singleton=this;
	}
    // Start is called before the first frame update
    void Start()
    {
		waypointsList = getWaypoints().ToArray();
    }

	/// <summary>
	/// Recupera una lista con todos los waypoints hijos
	/// </summary>
	public List<Transform> getWaypoints(){
		var ret = new List<Transform>();
         
         foreach (Transform child in transform) {
             ret.Add(child);

         }
         return ret;
	}

	/// <summary>
	/// Obtiene el siguiente punto en el recorrido de un runner
	/// </summary>
	/// <param name="current">Waypoint actual del runner</param>
	/// <param name="dir">Sentido de movimiento del runner (en la lista de waypoints)</param>
	/// <returns>Tuple of the point's Transform & it's index</returns>
	public (Transform, int) getNextPoint(int current, int dir){
		var index = (current + dir) % waypointsList.Length;
		return (waypointsList[index], index);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
