using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using System.Linq;


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
        /// Output con la transform de la papelera más cercana (si la hay)
        /// </summary>
        [OutParam("NearestTrash")]
        [Help("Resultado de buscar la papelera más cercana")]
        Transform trashOut;

        public override TaskStatus OnUpdate()
        {
            var thisPos = gameObject.transform.position;
            var layerMask = LayerMask.GetMask("Interactive");
            var results = Physics.OverlapSphere(thisPos, maxRange, layerMask);

            var best = results.Select(t =>
                    new {t, distance= Vector3.Distance(t.transform.position, thisPos)
                }).Aggregate((i1, i2) => i1.distance > i2.distance ? i1 : i2)
                .t;

            if(best != null){
                trashOut = best.transform;
                return TaskStatus.COMPLETED;
            }
                
            
            return TaskStatus.FAILED;
        }
        
        
    }
}
