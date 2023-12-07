using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;

namespace BBCore.Conditions
{
    /// <summary>
    /// It is a basic condition to check if Booleans have the same value.
    /// </summary>
    [Condition("Basic/CheckIfNull")]
    [Help("Checks if gameobject is null")]
    public class CheckIfNull : ConditionBase
    {
    
        [InParam("value")]
        [Help("Value to check")]
        public GameObject value;

        
        [InParam("invert")]
        [Help("Invert output")]
        public bool invert;

        /// <summary>
        /// Checks whether two booleans have the same value.
        /// </summary>
        /// <returns>the value of compare first boolean with the second boolean.</returns>
		public override bool Check()
		{
            return (value == null) ^ invert;
		}
    }
}