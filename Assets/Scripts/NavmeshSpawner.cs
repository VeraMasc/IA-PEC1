using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using UnityEngine;
using UnityEngine.AI;



/// <summary>
/// Se encarga de encontrar puntos de spawn válidos en la navmesh
/// </summary>
public class NavmeshSpawner : MonoBehaviour
{
    private static NavmeshSpawner _singleton;
	///<summary>NavmeshSpawner Singleton</summary>
	public static NavmeshSpawner singleton
	{
		get 
		{
			if (_singleton == null)
			{
				_singleton = FindObjectOfType<NavmeshSpawner>(); //Para cuando el maldito hotreload me pierde la referencia
			}
			return _singleton;
		}
	}

    public Vector2 spawnArea = Vector2.one *4;

    public List<int> excludeAreas = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Intenta obtener un punto de spawn válido
    /// </summary>
    /// <param name="tryLimit">Límite de intentos</param>
    /// <returns>Coordenadas deln punto de spawn válido</returns>
    /// <exception cref="System.Exception">Si no encuentra ninguno después de N intentos</exception>
    public Vector3 getSpawnPos(int tryLimit =100){
        if(tryLimit <=0)
            throw  new System.Exception("Spawn Point Generation try limit reached");

        var randVector = getRandomPos2D(transform.position, spawnArea);

        int areaMask = NavMesh.AllAreas;
        foreach(var area in excludeAreas){
            areaMask |= 1 << area;
        } 
        var valid = getValidPos(randVector, NavMesh.AllAreas);
        
        return valid ?? getSpawnPos(tryLimit-1);
    }

    /// <summary>
    /// Obtiene unas coordenadas 2D aleatorias dentro de un area
    /// </summary>
    /// <param name="center">Centro del area</param>
    /// <param name="area">Tamaño del area</param>
    /// <returns>Cooordenadas 2D</returns>
    public Vector2 getRandomPos2D(Vector2 center, Vector2 area){
        var rVector = new Vector2(Random.value * area.x, Random.value * area.y);
        rVector -= area/2;
        return center + rVector;
    }

    /// <summary>
    /// Busca una posición 3D válida en la navmesh a partir de unas coordenadas 2D
    /// </summary>
    /// <param name="areaMask">Máscara que indica qué áreas son validas</param>
    /// <returns>Posición 3D válida con las mismas coordenadas 2D (si existe)</returns>
    public Vector3? getValidPos(Vector2 pos2D, int areaMask){
        Vector3 source = new Vector3(pos2D.x,0,pos2D.y);

        var res = NavMesh.SamplePosition(source, out var hit, 1f, areaMask);

        Debug.Log($"Source:{source} {hit.position} > {res}");

        if(hit.hit==false)
            return null;
        
        return hit.position;
    }
}
