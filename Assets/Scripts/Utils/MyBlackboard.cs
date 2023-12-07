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

}

namespace Pada1.BBCore{
    /// <summary>
    /// Extender la Blackboard para facilitar trabajar con ella
    /// </summary>
    public static class BlackboardExtension
    {
        /// <summary>
        /// Obtiene el valor de un parámetro de la blackboard
        /// </summary>
        /// <param name="name">Nombre del parámetro</param>
        /// <returns></returns>
        public static T getParam<T>(this UnityBlackboard blackboard, string name)
        {
            // var hasValue = blackboard.behaviorParams.values.TryGetValue("IsPooping", out var value);
            // Debug.Log($"hasvalue: {value.constValue ?? (object) "null" }");
            // if(!hasValue)
            //     return default(T);
            
            // return (T)value.constValue;
            
        
            var typelists = blackboard.allParamLists.Where(list => typeof(T).IsAssignableFrom(list.t));

            foreach(var typelist in typelists){
                var index = typelist.paramNames.IndexOf(name);

                if (index != -1){
                    return (T) typelist.get(index);
                }
            }
            
            

            return default(T);
            
        }
    }
}

    