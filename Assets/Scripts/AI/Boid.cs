using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de cada boid individual
/// </summary>
public class Boid : MonoBehaviour
{
    /// <summary>
    /// Su manager
    /// </summary>
    public BoidManager manager;
    /// <summary>
    /// Cuanto queda para actualizar el movimiento del boid
    /// </summary>
    public float updateTimer;

    /// <summary>
    /// Dirección de movimiento actual del boid
    /// </summary>
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
        boidsNearby = manager.allBoids; ///Por defecto tiene en cuenta a todos los boids
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 
            manager.rotationSpeed * Time.deltaTime);
        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);

        updateTimer += Time.deltaTime ;
        if(updateTimer >= manager.updateRate){ //Actualiza la velocidad cada cierto tiempo
            calculateSpeed();
            updateTimer = 0;
        }
        
    }

    


    /// <summary>
    /// Fórmula del cálculo de la separación
    /// </summary>
    void forceSeparation(Boid go, ref Vector3  separation, float distance){
        
        var dsp = (distance);
        if (distance < manager.tooCloseDistance)
            dsp *= (distance/manager.tooCloseDistance);
        var res = manager.avoidance*(transform.position - go.transform.position) /
                                (dsp);
        separation += res;
    }

    /// <summary>
    /// Calcula la velocidad en base a los vecinos
    /// </summary>
    void calculateSpeed(){
        Vector3 cohesion, align, separation;
        cohesion = align = separation = Vector3.zero; //Para debuggear el resto del código

        int num = 0;
        foreach (Boid go in boidsNearby) //Tener en cuenta solo boids "cercanos"
        {
            float distance = Vector3.Distance(go.transform.position, transform.position);
            if (go != this) { //Ignore self
                if (distance <= manager.neighbourDistance)
                {
                    cohesion += go.transform.position; //Mantener cohesión
                    forceSeparation(go, ref separation, distance); //Mantener separación
                    align += go.direction; //Alinear con vecinos
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

    /// <summary>
    /// Altera la dirección de los boids para que vuelvan al interior de los límites si se salen
    /// </summary>
    void stayWithinBounds(){
        var nextpos = direction + transform.position;
        if (manager.boundsBox.Contains(nextpos))
            return;

        // nextpos = manager.boundsBox.ClosestPoint(nextpos);
        // direction = nextpos - transform.position;
        var strength = manager.boundsBox.SqrDistance(nextpos);
        var goBack = manager.transform.position - transform.position;

        direction += goBack * strength;
    }
}
