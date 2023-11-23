using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{

    [Header("General")]
    /// <summary>
    /// Lista de todos los boids
    /// </summary>
    public GameObject[] allBoids;

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

    public float neighbourDistance;
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
    
    }

    void initBoids(){
        allBoids = new GameObject[numBoids];
        for (int i = 0; i < numBoids; ++i) {
            Vector3 pos = this.transform.position + Random.insideUnitSphere * spawnRange; // random position
            Vector3 randomize = Random.insideUnitSphere.normalized; // random vector direction
            allBoids[i] = (GameObject)Instantiate(boidPrefab, pos, Quaternion.LookRotation(randomize));
            allBoids[i].GetComponent<Boid>().manager = this;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan; 
        Gizmos.DrawWireCube(transform.position, bounds);
    }
}
