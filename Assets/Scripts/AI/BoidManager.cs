using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controlador del conjunto de Boids
/// </summary>
public class BoidManager : MonoBehaviour
{

    [Header("General")]
    /// <summary>
    /// Lista de todos los boids
    /// </summary>
    public Boid[] allBoids;

    /// <summary>
    /// Número de boids a crear
    /// </summary>
    public int numBoids;
    /// <summary>
    /// prefab de los boids
    /// </summary>
    public GameObject boidPrefab;

    /// <summary>
    /// Dimensiones a partir de las que generamos la <see cref="boundsBox"/>
    /// </summary>
    public Vector3 bounds = new Vector3(10, 7, 10);
    [System.NonSerialized]
    /// <summary>
    /// Límites del area en la que se mueven los boids
    /// </summary>
    public Bounds boundsBox ;

    [Header("Boid Settings")]
    /// <summary>
    /// Velocidad a la que los boids ajustan su dirección
    /// </summary>
    public float rotationSpeed;

    /// <summary>
    /// Velocidad mínima de los Boids
    /// </summary>
    public float minSpeed =0.5f;

    /// <summary>
    /// Velocidad máxima de los Boids
    /// </summary>
    public float maxSpeed = 1.5f;

    /// <summary>
    /// Rango en el que spawnear los boids de forma aleatoria
    /// </summary>
    public float spawnRange = 6;

    /// <summary>
    /// Cada cuanto calcular el comportamiento de los boids
    /// </summary>
    public float updateRate =0.1f;

    /// <summary>
    /// Cada cuanto recalcular los voids cercanos que se tienen en cuenta
    /// </summary>
    public float watchNearbyRate =0.5f;

    /// <summary>
    /// Timer de <see cref="watchNearbyRate"/>
    /// </summary>
    public float watchNearbyTimer = 0;

    /// <summary>
    /// Distancia dentro de la cual los boids se consideran "vecinos"
    /// </summary>
    public float neighbourDistance;

    /// <summary>
    /// Distancia a partir de la cual no se comprueba si los voids son vecinos
    /// </summary>
    public float nearbyDistance;

    /// <summary>
    /// Aleatoriedad que añadir al movimiento
    /// </summary>
    public float directionNoise = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        initBoids();
        boundsBox = new Bounds(transform.position, bounds); 
    }

    // Update is called once per frame
    void Update()
    {
        watchNearbyTimer += Time.deltaTime;
        if (watchNearbyTimer >= watchNearbyRate || Input.GetKeyDown(KeyCode.C)){
            calculateNearby();
            watchNearbyTimer = 0;
        }
    }

    /// <summary>
    /// Crea e inicializa los boids
    /// </summary>
    void initBoids(){
        allBoids = new Boid[numBoids];
        for (int i = 0; i < numBoids; ++i) {
            Vector3 pos = this.transform.position + Random.insideUnitSphere * spawnRange; // random position
            Vector3 randomize = Random.insideUnitSphere.normalized; // random vector direction
            var newBoid = (GameObject)Instantiate(boidPrefab, pos, Quaternion.LookRotation(randomize));
            allBoids[i] = newBoid.GetComponent<Boid>();
            allBoids[i].manager = this;
        }
    }

    /// <summary>
    /// Calcula los boids "cercanos" de cada boid 
    /// (los que es posible que sean sus vecinos ahora o en el futuro cercano).
    /// </summary>
    void calculateNearby(){
        var nearbyDict = new Dictionary<Boid, Queue<Boid>>();
        int num = 1;
        foreach (Boid boid in allBoids){
            nearbyDict.Add(boid, new Queue<Boid>());
        }
        foreach (Boid boid in allBoids){ //Itera todos los boids
            for(var i = num; i<allBoids.Length; i++){ //Evita comparar con si mismo y boids ya iterados
                float distance = Vector3.Distance(boid.transform.position, allBoids[i].transform.position);

                if(distance <= nearbyDistance){
                    nearbyDict[boid].Enqueue(allBoids[i]);
                    nearbyDict[allBoids[i]].Enqueue(boid);
                }
            }
            num++;
        }

        foreach (Boid boid in allBoids){
            boid.boidsNearby = nearbyDict.TryGetValue(boid, out var queue)? queue.ToArray() : new Boid[0];
            Debug.Log(queue.ToArray());
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        //Dibujar gizmos de los límites
        Gizmos.DrawWireCube(transform.position, bounds);
    }
}
