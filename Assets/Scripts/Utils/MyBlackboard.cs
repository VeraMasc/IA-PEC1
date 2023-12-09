using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using BBUnity;
using static BBUnity.UnityBlackboard;
using System.Linq;

/// <summary>
/// Sirve como apaño para evitar los bugs que tiene la Blackboard de BB
/// </summary>
public class MyBlackboard : MonoBehaviour
{
    public GameObject detectedPoop;
    public bool isDogPooping;

    public bool isScared;

    public MyBlackboard detectedDog;

    public MyBlackboard detectedCleaner;

}

    