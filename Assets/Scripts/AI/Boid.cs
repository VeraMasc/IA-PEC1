using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidManager manager;

    public float updateTimer;
    public Vector3 direction;

    public float speed =1f;

    /// <summary>
    /// Array de boids cercanos que pueden ser vecinos
    /// </summary>
    public Boid[] boidsNearby;
    

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.forward * speed;
        boidsNearby = manager.allBoids;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 
            manager.rotationSpeed * Time.deltaTime);
        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);

        updateTimer += Time.deltaTime * speed;
        if(updateTimer >= manager.updateRate){
            calculateSpeed();
            updateTimer = 0;
        }
        
    }


    void mantainCohesion(Boid go, ref Vector3 cohesion, float distance){
        cohesion += go.transform.position;
    }

    void alignWithNeighbors(Boid go, ref Vector3 align, float distance){
        align += go.direction;
    }

    void forceSeparation(Boid go, ref Vector3  separation, float distance){
                
        separation += (transform.position - go.transform.position) / 
                                (distance * distance);
    }

    void calculateSpeed(){
        Vector3 cohesion, align, separation;
        cohesion = align = separation = Vector3.zero; //Para debuggear el resto del código

        int num = 0;
        foreach (Boid go in boidsNearby)
        {
            float distance = Vector3.Distance(go.transform.position, transform.position);
            if (go != this) { //Ignore self
                if (distance <= manager.neighbourDistance)
                {
                    mantainCohesion(go, ref cohesion, distance);
                    forceSeparation(go, ref separation, distance);
                    alignWithNeighbors(go, ref align, distance);
                    num++;
                }
                
            }
            
            
            
        }
        align += direction; //Sigue también su propio alineamiento (evita bugs)
        //Divide by boids
        align /= num+1;
        //align += Random.insideUnitSphere * manager.directionNoise;
        speed = Mathf.Clamp(align.magnitude, manager.minSpeed, manager.maxSpeed);

        
        if (num > 0)
            cohesion = (cohesion / num - transform.position).normalized * speed;

        direction = (cohesion + align + separation).normalized * speed
            + Random.insideUnitSphere * manager.directionNoise;
        stayWithinBounds();
    }

    void stayWithinBounds(){
        var nextpos = direction + transform.position;
        if (manager.boundsBox.Contains(nextpos))
            return;

        nextpos = manager.boundsBox.ClosestPoint(nextpos);
        direction = nextpos - transform.position;
    }
}
