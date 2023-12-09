using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Poop : MonoBehaviour
{
    // public event System.EventHandler OnCleaned;
    void OnEnable()
    {
        Controller.singleton.poops.Add(this);

        if(NavMesh.SamplePosition(transform.position, out var hit, 1f, NavMesh.AllAreas))
        {
            Debug.Log(hit.position);
            transform.position = hit.position + Vector3.up * 0.2f;
        }
    }

    void OnDisable()
    {
        Controller.singleton?.poops?.Remove(this);
        // OnCleaned(this, System.EventArgs.Empty);
    }

    public void CleanUp(){
        Destroy(gameObject);
    }

    
}
