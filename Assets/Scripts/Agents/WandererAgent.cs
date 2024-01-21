using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BBUnity.Actions;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Mathf = UnityEngine.Mathf;
using Random = UnityEngine.Random;

/// <summary>
/// Agente que se comporta como un cohete, segunda versión
/// </summary>
public class WandererAgent : Agent
{
    Rigidbody body;

    public float visionRange = 5f;

    public float visionSpread=20f;

    public float[] visionValues; 

    public List<int> visionObstacles = new List<int>();
    private int visionArea = Physics.AllLayers;

    public List<string> visionLayers = new List<string>();
    private int visionMask = Physics.AllLayers; 

    public float forceMultiplier = 10;

    public float rotateMultiplier = 10;



    /// <summary>
    /// Velocidad mínima a la que puede ir el cohete
    /// </summary>
    public float minSpeed = 1;

    /// <summary>
    /// Velocidad máxima a la que puede ir el cohete
    /// </summary>
    public float maxSpeed = 5;
    private float rotate;
    private float walk;

    public bool relativeInputs;

    [Header("Reward Metrics")]
    /// <summary>
    /// Mide cuanto está chocando con el entorno
    /// </summary>
    public float crash;

    public float maxCrash = 1000;

    /// <summary>
    /// Mide lo bien que ha viajado el agente
    /// </summary>
    public float travel;

    /// <summary>
    /// Cada cuanto actualizar el path
    /// </summary>
    public float pathRate= 10f;

    /// <summary>
    /// Contador de actualización del path
    /// </summary>
    private float pathCounter;

    public float travelDecayFactor = 0.6f;

    public float travelRecFactor = 0.6f;

    /// <summary>
    /// Número máximo de elementos en el path
    /// </summary>
    public int maxPath= 5;
    public List<Vector3> travelPath;

    void Start () {
        body = GetComponent<Rigidbody>();
        visionValues= new float[5];
    }


    public override void OnEpisodeBegin()
    {
        

        //Reset position and velocity if needed
        if(transform.localPosition.y < 0 || crash > maxCrash){
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.Move(NavmeshSpawner.singleton.getSpawnPos(), 
                Quaternion.AngleAxis(Random.Range(0,360), Vector3.up));

            pathCounter = pathRate;
            //Save start point
            travelPath = new List<Vector3>(){transform.position};
        }
        

        //Reset metrics
        crash =0;
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Vision
        foreach(var line in visionValues){
            sensor.AddObservation(line);
        }

        //Agent velocity
        sensor.AddObservation(body.angularVelocity.y);

        var velocity2D = new Vector2(body.velocity.x, body.velocity.z);
        if(relativeInputs){
            var rotation = Quaternion.FromToRotation(Vector3.forward, transform.forward);
            velocity2D = rotation* velocity2D;
        }
        sensor.AddObservation(velocity2D);
        //Agent direction
        sensor.AddObservation(new Vector2(transform.forward.x,transform.forward.z));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void Update() {
        visionRaycast();

        pathCounter -= Time.deltaTime;
        if(pathCounter<=0){
            pathCounter = pathRate;
            travelPath.Add(transform.position);
            
            if(travelPath.Count > maxPath){ //Limpiar exceso del path
                travelPath.RemoveAt(0);
            }
            calculateReward();
        }
    }

    private void FixedUpdate() {
        body.angularVelocity = Vector3.up * rotate * rotateMultiplier;

        var projVel = Vector3.Project(body.velocity,transform.forward);
        body.velocity -= projVel;
        body.velocity += transform.forward * walk * forceMultiplier;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        
        rotate = actionBuffers.ContinuousActions[0];
        walk = actionBuffers.ContinuousActions[1];
        

    

        // Fell off platform
        if (transform.localPosition.y < 0 || crash > maxCrash)
        {
            calculateReward();
            AddReward(-0.5f);
            EndEpisode();
            // Debug.Log($"TotalReward: {GetCumulativeReward()}");
        }

        
    }

    private void calculateReward(){
        SetReward(calculateTravelReward());
        AddReward(calculateCrashPunishment());
        
    }

    /// <summary>
    /// Calcula la recompensa dada por lo bien que está deambulando
    /// </summary>
    /// <param name="untilLast">Indice máximo a tener en cuenta (ignorar, usado para recursividad)</param>
    /// <returns>Recompensa</returns>
    private float calculateTravelReward(int untilLast=1){
        float tReward = 0f;
        var startIndex = travelPath.Count-untilLast-1;
        if(startIndex <0)
            return tReward;
        
        var last = travelPath.Last();

        var center =travelPath.GetRange(startIndex, untilLast)
            .Aggregate(new Vector3(0,0,0), (s,v) => s + v) 
            / untilLast;

        tReward += Vector3.Distance(center,last);
        
        // Debug.Log($"Index {untilLast}: {tReward}");
        var ret = 1- 1/(travelDecayFactor * tReward +1);

        if(untilLast< travelPath.Count){
            var prevReward = calculateTravelReward(untilLast+1);
            ret = (ret * (1-travelRecFactor) + prevReward*travelRecFactor)/2; //Ponderar
        }
        if(ret >1 || ret<0)
            Debug.Log($"Travel Reward out of bounds:{ret} Raw:{tReward} Step:{untilLast}");
        
        return Math.Clamp(ret,0f,1f);
    }

    /// <summary>
    /// Calcula el castigo derivado de chocar con el entorno
    /// </summary>
    /// <returns>Entre -0.5 y 0 según cuanto se choque y cuan a menudo</returns>
    private float calculateCrashPunishment(){
        var powSteps = Mathf.Pow(StepCount,0.9f);
        var rate = Mathf.Log((crash+1) * maxCrash * powSteps,10)
            /Mathf.Log(maxCrash *powSteps,10)
            -1 ; //Tiende a 1 en maxsteps
        //Debug.Log(rate);
        return Mathf.Clamp(-0.5f * rate,-0.5f,0f);
    }

    private void OnCollisionStay(Collision other) {
        ContactPoint[] contacts = new ContactPoint[other.contactCount];
        other.GetContacts(contacts);
        int n =1;
        foreach(var contact in contacts){
            if (Vector3.Angle(contact.normal,Vector3.up) <= 30) //Ignorar colisiones con el suelo
                continue;
            
            if(contact.impulse.magnitude ==0) //Ignorar colisiones sin empuje
                continue;
            var angle = Vector3.Angle(contact.normal, -transform.forward);
            //Debug.Log($"Contac {n}: {angle} {contact.impulse.magnitude}");
            crash += contact.impulse.magnitude;
            n++;
        }
        
    }
    
    /// <summary>
    /// Realiza los raycasts que se usan como inputs de la visión
    /// </summary>
    private void visionRaycast(){
        var lines = new float[]{-visionSpread*2, -visionSpread, 0, visionSpread, visionSpread*2};
        for(var i=0; i<lines.Length; i++){
            var pos = transform.position + Vector3.down*0.5f;
            var vector = Quaternion.AngleAxis(lines[i],Vector3.up) * transform.forward;
            
            //Raycasts (se necesitan ambos para detectar todo)
            Physics.Raycast(transform.position, vector, out RaycastHit rayInfo, visionRange, visionMask);
            NavMesh.Raycast(pos, pos+vector*visionRange,out var hitInfo, visionArea);
            
            var dist =  hitInfo.distance;
            if(rayInfo.collider && rayInfo.distance < dist) //Keep minimum
                dist = rayInfo.distance;

            //Keep results within bounds
            dist = Mathf.Min(dist, visionRange);
            visionValues[i] = dist ;
        }
        
    }


    private void OnDrawGizmos() {
        if(!Application.isPlaying) //Hace que solo recalcule en el editor
            visionRaycast(); 

        //Draw shight lines
        var lines = new float[]{-visionSpread*2, -visionSpread, 0, visionSpread, visionSpread*2};

        for(var i=0; i<lines.Length; i++){
            var vector = Quaternion.AngleAxis(lines[i],Vector3.up) * transform.forward * visionValues[i];
            //Dibujar con color
            Gizmos.color = visionValues[i] == visionRange? Color.green: Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + vector);
        }
         
    }

    private void OnValidate() {
        if (visionValues.Length<5){
            visionValues = new float[5];
        }

        //Calcular máscaras
        visionArea = NavMesh.AllAreas;
        foreach(var obstacle in visionObstacles){
            visionArea ^= 1 << obstacle;
        }

        visionMask = LayerMask.GetMask(visionLayers.ToArray());
    }
}
 