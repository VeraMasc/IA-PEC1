using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidManager manager;
    public Vector3 direction;

    public float speed =1f;
    

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 
            manager.rotationSpeed * Time.deltaTime);
        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);

        calculateSpeed();
    }


    Vector3 mantainCohesion(){
        Vector3 cohesion = Vector3.zero;
        int num = 0;
        foreach (GameObject go in manager.allBoids) {
            if (go != this.gameObject) {
                float distance = Vector3.Distance(go.transform.position, 
                                                transform.position);
                if (distance <= manager.neighbourDistance) {
                    cohesion += go.transform.position;
                    num++;
                }
            }
        }
        if (num > 0)
            cohesion = (cohesion / num - transform.position).normalized * speed;
        return cohesion;
    }

    Vector3 alignWithNeighbors(){
        Vector3 align = Vector3.zero;
        int num = 1; //Siempre habrá al menos un alineamiento (el suyo)
        foreach (GameObject go in manager.allBoids) {
            if (go != this.gameObject) {
                float distance = Vector3.Distance(go.transform.position, 
                                                transform.position);
                if (distance <= manager.neighbourDistance) {
                    align += go.GetComponent<Boid>().direction;
                    num++;
                }
            }
            else { align += direction; } //Sigue también su propio alineamiento (evita bugs)
        }


        align /= num;
        align += Random.insideUnitSphere * manager.directionNoise;
        speed = Mathf.Clamp(align.magnitude, manager.minSpeed, manager.maxSpeed);

        return align;
    }

    Vector3 forceSeparation(){
        Vector3 separation = Vector3.zero;
        foreach (GameObject go in manager.allBoids) {
            if (go != this.gameObject) {
                float distance = Vector3.Distance(go.transform.position, 
                                                transform.position);
                if (distance <= manager.neighbourDistance)
                    separation += (transform.position - go.transform.position) / 
                                (distance * distance);
            }
        }
        return separation;
    }

    void calculateSpeed(){
        Vector3 cohesion, align, separation;
        cohesion = align = separation = Vector3.zero; //Para debuggear el resto del código
        cohesion = mantainCohesion();
        align = alignWithNeighbors();
        separation = forceSeparation();
        direction = (cohesion + align + separation).normalized * speed;
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
