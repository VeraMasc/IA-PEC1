using System.Linq;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Conditions
{
    /// <summary>

    /// </summary>
    [Condition("Perception/IsObjectNearby")]
    [Help("Hay un perro cerca?")]
    public class IsObjectNearby : GOCondition
    {
        /// <summary>
        /// Rángo máximo a tener en cuenta 
        /// </summary>
        [InParam("maxRange")]
        [Help("Rángo máximo a tener en cuenta ")]
        public float maxRange;

        /// <summary>
        /// Tag del objeto a buscar
        /// </summary>
        [InParam("objectTag")]
        [Help("Tag del objeto a buscar")]
        public string objectTag;


        /// <summary>
        /// Output con el objeto  más cercano (si lo hay)
        /// </summary>
        [OutParam("nearestObj")]
        [Help("Resultado de buscar al perro más cercana")]
        public GameObject nearestObj;

        public override bool Check()
		{
            var thisPos = gameObject.transform.position;
            var layerMask = LayerMask.GetMask("Interactive");
            var results = Physics.OverlapSphere(thisPos, maxRange, layerMask).AsEnumerable();
            
            if(objectTag != "") //Filtrar por tag
                results = results.Where(obj => obj.tag == objectTag);

            //Obtener papelera más cercana dentro del rango
            var best = results.Select(t =>
                    new {t, distance= Vector3.Distance(t.transform.position, thisPos)
                }).Aggregate(new {  t = (Collider) null , distance = Mathf.Infinity}, (i1, i2) => i1.distance < i2.distance ? i1 : i2)
                .t;

            if(best != null){
                nearestObj = best.gameObject;
                return true;
            }

            return false;
		}
    }
}