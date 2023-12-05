using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine.Assertions;
using System;




namespace BBCore.Conditions
{
    public enum CompareType{
        equal,
        greaterOrEqual,
        lesserOrEqual,
        notEqual
    }
    /// <summary>
    /// 
    /// </summary>
    [Condition("Basic/CheckFloat")]
    [Help("Compares the values of two floats")]
    public class CheckFloat : ConditionBase
    {
        ///<value>Input First Boolean Parameter.</value>
        [InParam("valueA")]
        [Help("First value to be compared")]
        public float valueA;

        ///<value>Input Second Boolean Parameter.</value>
        [InParam("valueB")]
        [Help("Second value to be compared")]
        public float valueB;

        [InParam("comparison")]
        [Help("Type of comparison")]
        public CompareType comparison;

        /// <summary>
        /// Checks whether two booleans have the same value.
        /// </summary>
        /// <returns>the value of compare first boolean with the second boolean.</returns>
		public override bool Check()
		{
            switch(comparison){
                case CompareType.equal:
                    return valueA == valueB;
                
                case CompareType.greaterOrEqual:
                    return valueA >= valueB;

                case CompareType.lesserOrEqual:
                    return valueA <= valueB;

                case CompareType.notEqual:
                    return valueA != valueB;

                default:
                    throw new ApplicationException($"Invalid comparison value: {comparison}");
            }
			
		}
    }
}