using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using System.Linq;
using System;


namespace BBUnity.Actions
{
    /// <summary>
    /// Encuentra la papelera más cercana
    /// </summary>
    [Action("FindTrashCan")]
    [Help("Encuentra la papelera más cercana")]
    public class FindTrashCan : GOAction
    {

        /// <summary>
        /// Rángo máximo a tener en cuenta (- para infinito)
        /// </summary>
        [InParam("maxRange")]
        [Help("Rángo máximo a tener en cuenta (- para infinito)")]
        public float maxRange;


        /// <summary>
        /// Output con la posición de la papelera más cercana (si la hay)
        /// </summary>
        [OutParam("nearestTrash")]
        [Help("Resultado de buscar la papelera más cercana")]
        public Vector3 nearestTrash;

        public override TaskStatus OnUpdate()
        {
            var thisPos = gameObject.transform.position;
            var layerMask = LayerMask.GetMask("Interactive");
            var results = Physics.OverlapSphere(thisPos, maxRange, layerMask);

            //Obtener papelera más cercana dentro del rango
            var best = results.Select(t =>
                    new {t, distance= Vector3.Distance(t.transform.position, thisPos)
                }).Aggregate(new {  t = (Collider) null , distance = Mathf.Infinity}, (i1, i2) => i1.distance < i2.distance ? i1 : i2)
                .t;

            if(best != null){
                nearestTrash = best.transform.position;
                return TaskStatus.COMPLETED;
            }
                
            
            return TaskStatus.FAILED;
        }
        
        
    }
}
