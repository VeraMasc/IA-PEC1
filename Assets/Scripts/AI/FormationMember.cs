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

    public Vector3? formPosition;

    public Quaternion? formRotation;

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

    public void joinFormation(Formation form){
        var num = form.members.Count;
        form.members.Add(this);
        formation = form;
        formation.isDirty = true;
    }


    public void followFormation(){
        if (formation == null || formPosition == null || formRotation == null)
            return;

        var addRot = formation.transform.rotation;

        if (agent.destination != formPosition + formation.transform.position){
            agent.destination = (addRot * (Vector3)formPosition) + formation.transform.position; //actualiza la posición

        } else if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance){
            //Girar el agente en la dirección que toca
            transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion)formRotation * addRot, Time.deltaTime * agent.angularSpeed);
        }
    }
}
