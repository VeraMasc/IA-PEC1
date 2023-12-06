using System.Linq;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;

namespace BBUnity.Conditions
{
    /// <summary>

    /// </summary>
    [Condition("Perception/IsPoopNearby")]
    [Help("Hay un perro cerca?")]
    public class IsPoopNearby : GOCondition
    {
        /// <summary>
        /// Rángo máximo a tener en cuenta 
        /// </summary>
        [InParam("maxRange")]
        [Help("Rángo máximo a tener en cuenta ")]
        public float maxRange;

        

        /// <summary>
        /// Output con el objeto  más cercano (si lo hay)
        /// </summary>
        
        [OutParam("nearestObj")]
        [Help("Resultado de buscar al perro más cercana")]
        public GameObject nearestObj;

        GameObject _nearestObj;

        MyBlackboard _blackboard;

        MyBlackboard blackboard {
            get =>_blackboard ??= gameObject.GetComponent<MyBlackboard>();
            
        }

        /// <summary>
        /// invoked by the execution engine when the condition was previously true
        /// </summary>
        /// <returns>Failed si la condición se ha vuelto falsa</returns>
        public override TaskStatus MonitorFailWhenFalse()
        {
            
            if (blackboard.detectedPoop != null || Check()) // if (Check())
                return TaskStatus.RUNNING;
            else
                return TaskStatus.FAILED;
        }
        
        /// <summary>
        /// Invoked by the execution engine when the condition was previously false
        /// </summary>
        /// <returns>Complete si la condición se ha vuelto cierta</returns>
        public override TaskStatus MonitorCompleteWhenTrue()
        {
            if (!Check())
                return TaskStatus.RUNNING;
            else
                return TaskStatus.COMPLETED;
        }

        public override void OnAbort()
        {
            
            base.OnAbort();
        }

        public override bool Check()
		{
            var thisPos = gameObject.transform.position;
            var results = Controller.singleton.poops.AsEnumerable();//Physics.OverlapSphere(thisPos, maxRange, layerMask).AsEnumerable();
            
        

            //Obtener papelera más cercana dentro del rango
            var best = results.Select(t =>
                    new {t, distance= Vector3.Distance(t.transform.position, thisPos)
                }).Aggregate(new {  t = (Poop) null , distance = Mathf.Infinity}, (i1, i2) => i1.distance < i2.distance ? i1 : i2)
                ;

            
            if(best.t != null && best.distance <= maxRange){
                blackboard.detectedPoop= best.t.gameObject;
                return true;
            }

            return false;
		}
    }
}