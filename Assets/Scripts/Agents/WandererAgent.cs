using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using Mathf = UnityEngine.Mathf;

/// <summary>
/// Agente que se comporta como un cohete, segunda versión
/// </summary>
public class WandererAgent : Agent
{
    Rigidbody body;

    public float visionRange = 5f;

    public float visionSpread=20f;

    public Transform Target;

    public float forceMultiplier = 10;

    public float rotateMultiplier = 10;

    public float dragFactor =0.05f;

    public float lateralDrag =0.1f;

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

    private float closest;

    void Start () {
        body = GetComponent<Rigidbody>();
    }


    public override void OnEpisodeBegin()
    {
       // If the Agent fell, zero its momentum
        if (transform.localPosition.y < 0 || rotateCount>30)
        {
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
            transform.localPosition = new Vector3( 0, 0.5f, 0);
        }
        rotateCount = 0;
        closest = Mathf.Infinity;
        // Move the target to a new spot
        Target.localPosition = new Vector3((Random.value * 8 - 4) * spawnRange,
                                           0.5f,
                                           (Random.value * 8 - 4) * spawnRange);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(body.velocity.x);
        sensor.AddObservation(body.velocity.z);

        //Agent
        sensor.AddObservation(body.angularVelocity.y);
        //Agent direction
        sensor.AddObservation(new Vector2(transform.forward.x,transform.forward.z));
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

        //Artificial Lateral Drag
        var rejected = body.velocity - projected;
        body.velocity -= rejected * lateralDrag;
        
        //Clamp speed
        var newSpd = projected.magnitude * sign + accel * forceMultiplier;
        newSpd = Mathf.Clamp(newSpd, minSpeed, maxSpeed);
        var changeSpd =  newSpd - projected.magnitude * sign;
        changeSpd = Mathf.Clamp(changeSpd,  -forceMultiplier, forceMultiplier);
        body.velocity += changeSpd * transform.up;

        //Set rotation drag
        body.angularDrag = newSpd * dragFactor;

        

        //Track rotation
        rotateCount += Mathf.Abs(body.angularVelocity.y * Time.fixedDeltaTime);

        //Track distance
        closest = Mathf.Min(closest, Vector3.Distance(transform.position,Target.position));

        //Fail agent for spinning constantly
        if(rotateCount>30){ 
            AddReward(-1f);
            rewardDistance();
            EndEpisode();
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        
        rotate = actionBuffers.ContinuousActions[0];
        accel = actionBuffers.ContinuousActions[1];
        

    

        // Fell off platform
        if (transform.localPosition.y < 0)
        {
            AddReward(-0.5f);
            rewardDistance();
            EndEpisode();
        }

        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.transform == Target){
            rotateCount = Mathf.Max(rotateCount/2f,1);
            float reward = 0.5f;
            reward /= rotateCount;
            var logSteps = Mathf.Log(Mathf.Max(StepCount/4f,1))*0.1f+1;
            reward /= logSteps;
            SetReward(reward +0.5f);
            EndEpisode();
        }
    }

    private void rewardDistance(){
        var distReward = Mathf.Log(Mathf.Max(closest,1))+1; //Reducir la recompensa logaritmicamente con la distancia
        AddReward(0.5f/distReward);
    }



    private void OnDrawGizmos() {
        //Draw shight lines
        Gizmos.color = Color.green;
        var sightVector = transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + sightVector);
    }
}
 