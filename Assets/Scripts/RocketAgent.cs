using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using Mathf = UnityEngine.Mathf;

/// <summary>
/// Agente que se comporta como un cohete
/// </summary>
public class RocketAgent : Agent
{
    Rigidbody body;

    public Transform Target;

    public float forceMultiplier = 10;

    public float rotateMultiplier = 10;

    public float rotateCount ;

    public float spawnRange =1;

    /// <summary>
    /// Velocidad mínima a la que puede ir el cohete
    /// </summary>
    public float minSpeed = 1;

    /// <summary>
    /// Velocidad máxima a la que puede ir el cohete
    /// </summary>
    public float maxSpeed = 5;
    private float rotate;
    private float accel;

    void Start () {
        body = GetComponent<Rigidbody>();
    }


    public override void OnEpisodeBegin()
    {
       // If the Agent fell, zero its momentum
        if (transform.localPosition.y < 0)
        {
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
            transform.localPosition = new Vector3( 0, 0.5f, 0);
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4 * spawnRange,
                                           0.5f,
                                           Random.value * 8 - 4 * spawnRange);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(body.velocity.x);
        sensor.AddObservation(body.velocity.z);

        //Agent direction
        sensor.AddObservation(transform.forward);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void FixedUpdate() {

        // body.AddRelativeForce(accel * new Vector3(0, forceMultiplier, 0));
        body.AddTorque(0,rotate * rotateMultiplier,0);

        //Forward velocity component
        var projected = Vector3.Project(body.velocity, transform.up);
        var sign = Mathf.Sign(Vector3.Dot(projected, transform.up));
        
        //Clamp speed
        var newSpd = projected.magnitude * sign + accel * forceMultiplier;
        var changeSpd = Mathf.Clamp(newSpd, minSpeed, maxSpeed) - projected.magnitude * sign;
        changeSpd = Mathf.Clamp(changeSpd,  -forceMultiplier, forceMultiplier);
        body.velocity += changeSpd * transform.up;

        //Track rotation
        rotateCount = body.angularVelocity.y * Time.fixedDeltaTime;
    
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        
        rotate = actionBuffers.ContinuousActions[0];
        accel = actionBuffers.ContinuousActions[1];
        

    

        // Fell off platform
        if (transform.localPosition.y < 0)
        {
            EndEpisode();
        }

        
    }
     private void OnTriggerEnter(Collider other) {
        if(other.transform == Target){
            Debug.Log("Target Reached");
            rotateCount = Mathf.Max(rotateCount,1);
            float reward = 1.0f;
            reward /= rotateCount;
            var logSteps = Mathf.Log(StepCount)*0.1f+1;
            reward /= logSteps;
            SetReward(reward);
            EndEpisode();
        }
    }
}
