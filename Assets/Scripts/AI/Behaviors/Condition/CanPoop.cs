using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Conditions
{
    /// <summary>
    /// Comprueba si la ubicación y situación actuales son aceptables
    /// para que el perro haga caca
    /// </summary>
    [Condition("Perception/CanPoop")]
    [Help("Comprueba si el perro puede hacer caca")]
    public class CanPoop : GOCondition
    {
        ///<value>Número máximo de cacas.</value>
        [InParam("poopLimit")]
        [Help("Max number of poops")]
        public int poopLimit=5;

        [InParam("poopRange")]
        [Help("Distancia mínima entre cacas")]
        public float poopRange=10;

        MyBlackboard _blackboard;

        MyBlackboard blackboard {
            get =>_blackboard ??= gameObject.GetComponent<MyBlackboard>();
            
        }

        public override bool Check()
		{
            if (blackboard.isDogPooping)
                return true;

            if (Controller.singleton.poops.Count >= poopLimit)
                return false;
            
            if(blackboard.poopDelay>0){
                blackboard.poopDelay -= Random.Range(0.5f, 1.5f) * Time.deltaTime; //Tiempo aleatorio entre cacas
                return false;
            }

            if (IsPoopNearby.isPoopNearPos(gameObject.transform.position, poopRange, out _))
                return false;
            
            return true;
		}
    }
}