using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

    public Vector3 bounds = new Vector3(10, 7, 10);
    [System.NonSerialized]
    public Bounds boundsBox ;

    [Header("Boid Settings")]
    public float rotationSpeed;

    public float minSpeed =0.5f;

    public float maxSpeed = 1.5f;

    public float spawnRange = 6;

    public float updateRate =0.1f;

    public float neighbourDistance;
    public float nearbyDistance;
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
        if (Input.GetKeyDown(KeyCode.C)){
            calculateNearby();
        }
    }

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
        Gizmos.DrawWireCube(transform.position, bounds);
    }
}
