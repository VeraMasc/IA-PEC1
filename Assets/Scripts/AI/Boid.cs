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
        direction = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        calculateSpeed();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 
            manager.rotationSpeed * Time.deltaTime);
        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);
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
        int num = 0;
        foreach (GameObject go in manager.allBoids) {
            if (go != this.gameObject) {
                float distance = Vector3.Distance(go.transform.position, 
                                                transform.position);
                if (distance <= manager.neighbourDistance) {
                    align += go.GetComponent<Boid>().direction;
                    num++;
                }
            }
        }
        if (num > 0) {
            align /= num;
            speed = Mathf.Clamp(align.magnitude, manager.minSpeed, manager.maxSpeed);
        }
        return align;
    }

    void calculateSpeed(){
        Vector3 cohesion = mantainCohesion();
        Vector3 align = direction;
        Vector3 separation = Vector3.zero;
        direction = (cohesion + align + separation).normalized * speed;
    }
}
