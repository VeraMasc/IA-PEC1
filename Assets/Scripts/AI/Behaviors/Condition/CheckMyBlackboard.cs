using Pada1.BBCore.Framework;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine.Assertions;
using System;
using BBUnity.Conditions;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;




namespace BBCore.Conditions
{
    
    /// <summary>
    /// 
    /// </summary>
    [Condition("Basic/CheckMyBlackboard")]
    [Help("Compares the values of two floats")]
    public class CheckMyBlackboard : GOCondition
    {


        [InParam("fieldName")]
        public string fieldName;

        ///<value>Input Second Boolean Parameter.</value>
        [InParam("floatB")]
        [Help("Second value to be compared")]
        public float floatB;

        [InParam("objB")]
        public GameObject objB;

        [InParam("stringB")]
        public string stringB;

        [InParam("boolB")]
        public bool boolB;

        [InParam("comparison")]
        [Help("Type of comparison")]
        public CompareType comparison;

        [InParam("invert")]
        [Help("Invert output")]
        public bool invert;


        MyBlackboard _blackboard;

        MyBlackboard blackboard {
            get =>_blackboard ??= gameObject.GetComponent<MyBlackboard>();
            
        }

        private FieldInfo field;

        private MethodInfo method;

        /// <summary>
        /// Checks whether two booleans have the same value.
        /// </summary>
        /// <returns>the value of compare first boolean with the second boolean.</returns>
        public override bool Check()
		{

            field ??= typeof(MyBlackboard).GetField(fieldName);
            if (field == null)
                throw new Exception($"MyBlackboard field with name {fieldName} not found");
            method ??= typeof(MyBlackboard).GetMethod("compare");

            var type = field.FieldType;
            var valueA = field.GetValue(blackboard);

            object valueB = getRightInput(type);


            var generic = method.MakeGenericMethod(type);

            return (bool) generic.Invoke(this, new object[] { valueA, valueB });
		}

        public object getRightInput(Type type){
            object ret;
            if( typeof(string).IsAssignableFrom(type)){
                ret = stringB;

            }else if( typeof(float).IsAssignableFrom(type)){
                ret = floatB;

            }else if( typeof(bool).IsAssignableFrom(type)){
                ret = boolB;
            }else if( typeof(GameObject).IsAssignableFrom(type)){
                ret = objB;
            }else{
                throw new ApplicationException($"Invalid field type: {type.Name}");
            }
            return ret;
        }

        public bool compare<T>(T valueA, T valueB){
            bool ret;
            switch(comparison){
                case CompareType.equal:
                    ret = EqualityComparer<T>.Default.Equals(valueA, valueB);
                    break;
                
                case CompareType.greaterOrEqual:
                    ret = Comparer<T>.Default.Compare(valueA, valueB) >= 0;
                    break;

                case CompareType.lesserOrEqual:
                    ret = Comparer<T>.Default.Compare(valueA, valueB) <= 0;
                    break;

                case CompareType.notEqual:
                    ret = !EqualityComparer<T>.Default.Equals(valueA, valueB);
                    break;

                default:
                    throw new ApplicationException($"Invalid comparison value: {comparison}");
            }

            return ret ^ invert;
        }
    }
}