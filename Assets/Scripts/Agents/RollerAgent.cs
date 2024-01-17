using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RollerAgent : Agent
{
    Rigidbody body;

    public Transform Target;

    public float forceMultiplier = 10;
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
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(body.velocity.x);
        sensor.AddObservation(body.velocity.z);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        body.AddForce(controlSignal * forceMultiplier);

        
        // Fell off platform
        if (transform.localPosition.y < 0)
        {
            EndEpisode();
        }

        
    }
    private void OnCollisionEnter(Collision other) {
        if(other.collider.transform == Target){
            Debug.Log("Target Reached");
            SetReward(1.0f);
            EndEpisode();
        }
    }
}
