using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum FormationShape{
    none,
    circle,
    square,
    randomSpread
}


/// <summary>
/// Gestiona los movimientos en formación
/// </summary>
public class Formation : MonoBehaviour
{

    public List<FormationMember> members = new List<FormationMember>();

    public NavMeshAgent agent;

    public float spread = 2f;

    public float maxSpread = 8f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
