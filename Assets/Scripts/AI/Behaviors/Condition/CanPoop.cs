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
        public override bool Check()
		{
            if (Controller.singleton.poops.Count >= poopLimit)
                return false;

            if (IsPoopNearby.isPoopNearPos(gameObject.transform.position, poopRange, out _))
                return false;
            
            return true;
		}
    }
}