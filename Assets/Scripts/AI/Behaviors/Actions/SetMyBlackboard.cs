using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using System;



namespace BBUnity.Actions
{
    [Action("SetMyBlackboard")]
    [Help("Deja de hablar")]
    public class SetMyBlackboard : GOAction
    {

        

        [InParam("fieldName")]
        public string fieldName;



        [InParam("objIn")]
        public GameObject objIn;

        [InParam("floatIn")]
        public float floatIn;

        [InParam("stringIn")]
        public string stringIn;

        [InParam("boolIn")]
        public bool boolIn;

        MyBlackboard _blackboard;

        MyBlackboard blackboard {
            get =>_blackboard ??= gameObject.GetComponent<MyBlackboard>();
            
        }

        public override void OnStart()
        {
            var field = blackboard.GetType().GetField(fieldName);

            if (field == null)
                throw new Exception($"MyBlackboard field with name {fieldName} not found");
            var type = field.FieldType;

            Debug.Log(type);

            if( type.IsAssignableFrom(typeof(string))){
                field.SetValue(blackboard, stringIn);
            }

            if( type.IsAssignableFrom(typeof(float))){
                field.SetValue(blackboard,floatIn);
            }

            if( type.IsAssignableFrom(typeof(bool))){
                field.SetValue(blackboard,boolIn);
            }

            if( type.IsAssignableFrom(typeof(GameObject))){
                field.SetValue(blackboard,objIn);
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;    
        }
    }
}