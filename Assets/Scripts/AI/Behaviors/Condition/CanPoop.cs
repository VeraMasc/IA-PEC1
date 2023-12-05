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
        
        public override bool Check()
		{
            return true;
		}
    }
}