using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Conditions
{

    [Condition("IsFormationDone")]
    [Help("Comprueba si los miembros de la formación están en posición")]
    public class IsFormationDone : GOCondition
    {
        private Formation formation;

        /// <summary>
        /// Checks whether a target is close depending on a given distance,
        /// calculates the magnitude between the gameobject and the target and then compares with the given distance.
        /// </summary>
        /// <returns>True if the magnitude between the gameobject and de target is lower that the given distance.</returns>
        public override bool Check()
		{
            formation ??= gameObject.GetComponent<Formation>();

            return formation.allInPlace;
		}
    }
}