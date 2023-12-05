using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poop : MonoBehaviour
{
    void OnEnable()
    {
        Controller.singleton.poops.Add(this);
    }

    void OnDisable()
    {
        Controller.singleton.poops.Remove(this);
    }
}
