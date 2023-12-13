using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Script que gestiona a los miembros de una formación
/// </summary>
public class FormationMember : MonoBehaviour
{

    public NavMeshAgent agent;

    /// <summary>
    /// Formación a la que pertenece
    /// </summary>
    public Formation formation;

    /// <summary>
    /// Su posición dentro de la formación (relativa a la del líder)
    /// </summary>
    public Vector3? formPosition;

    /// <summary>
    /// Su rotación dentro de la formación (relativa a la del líder)
    /// </summary>
    public Quaternion? formRotation;

    /// <summary>
    /// Indica si el miembro de la formación está en el lugar que se le ha asignado
    /// </summary>
    public bool isInPlace {
        get {
            if (formPosition == null)
                return true;
            var worldpos = formationPosToWorld((Vector3)formPosition, formation.transform.rotation);
            var dist = Vector3.Scale((Vector3)(agent.nextPosition - worldpos), new Vector3(1,0,1));
            Debug.Log(dist);
            return  dist.magnitude <= agent.stoppingDistance + formation?.tolerance;
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        var formations = GameObject.FindObjectsOfType<Formation>();
        var best = formations.Select(t =>
                    new
                    {
                        t,
                        distance = Vector3.Distance(t.transform.position, transform.position)
                    }).Aggregate(new { t = (Formation)null, distance = Mathf.Infinity }, (i1, i2) => i1.distance < i2.distance ? i1 : i2);

        joinFormation(best.t);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        followFormation();
    }

    /// <summary>
    /// Hace que el miembro se una a una formación
    /// </summary>
    /// <param name="form"></param>
    public void joinFormation(Formation form){
        var num = form.members.Count;
        form.members.Add(this);
        formation = form;
        formation.isDirty = true;
    }


    /// <summary>
    /// Actualiza el destino del navmesh agent para que siga la formación
    /// </summary>
    public void followFormation(){
        if (formation == null || formPosition == null || formRotation == null)
            return;

        var addRot = formation.transform.rotation;
        var worldpos = formationPosToWorld((Vector3)formPosition, addRot);
        if (agent.destination != worldpos){
            agent.destination = worldpos; //actualiza la posición

        } else if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance){
            //Girar el agente en la dirección que toca
            transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion)formRotation * addRot, Time.deltaTime * agent.angularSpeed);
        }
    }

    /// <summary>
    /// Convierte la posición relativa dentro de la formación en un valor de posición absoluto
    /// </summary>
    /// <param name="relativePos"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public Vector3 formationPosToWorld(Vector3 relativePos,Quaternion rotation ){
        return (rotation * relativePos) + formation.transform.position;
    }
}
