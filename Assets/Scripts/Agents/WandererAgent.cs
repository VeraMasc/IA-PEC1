using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
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

    public int visionMask = Physics.AllLayers; 

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

    [Header("Reward Metrics")]
    /// <summary>
    /// Mide cuanto está chocando con el entorno
    /// </summary>
    public float crash;

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

    public float travelDecayFactor = 0.9f;

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
        pathCounter = pathRate;
        travelPath = new List<Vector3>(){transform.position};
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Vision
        foreach(var line in visionValues){
            sensor.AddObservation(line);
        }

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
        if (transform.localPosition.y < 0 || crash > 1000)
        {
            calculateReward();
            EndEpisode();
        }

        
    }

    private void calculateReward(){
        SetReward(calculateTravelReward());
        AddReward(calculateCrashPunishment());
    }

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
        
        Debug.Log($"Index {untilLast}: {tReward}");

        if(untilLast< travelPath.Count){
            tReward += calculateTravelReward(untilLast+1);
        }

        
        if(untilLast==1)
            Debug.Log($"TotalReward: {tReward}");

        return tReward;
    }

    private float calculateCrashPunishment(){
        return 0f;
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
            Debug.Log($"Contac {n}: {angle} {contact.impulse.magnitude}");
            crash += contact.impulse.magnitude;
            n++;
        }
        
    }
    

    private void visionRaycast(){
        var lines = new float[]{-visionSpread*2, -visionSpread, 0, visionSpread, visionSpread*2};
        for(var i=0; i<lines.Length; i++){
            var vector = Quaternion.AngleAxis(lines[i],Vector3.up) * transform.forward;
            Physics.Raycast(transform.position, vector, out RaycastHit hitInfo, visionRange, visionMask);
            visionValues[i] = hitInfo.collider? hitInfo.distance : visionRange;
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
    }
}
 