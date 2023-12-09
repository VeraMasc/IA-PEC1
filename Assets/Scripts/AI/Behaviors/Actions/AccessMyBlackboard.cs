using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using System;
using Unity.VisualScripting;



namespace BBUnity.Actions
{
    [Action("AccessBlackboard")]
    [Help("Deja de hablar")]
    public class AccessMyBlackboard : GOAction
    {

        

        [InParam("fieldName")]
        public string fieldName;



        [OutParam("objOut")]
        public GameObject objOut;

        [OutParam("floatOut")]
        public float floatOut;

        [OutParam("stringOut")]
        public string stringOut;

        [OutParam("boolOut")]
        public bool boolOut;

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

            if( typeof(string).IsAssignableFrom(type)){
                stringOut = field.GetValue(blackboard) as string;
            }

            if( typeof(float).IsAssignableFrom(type)){
                floatOut = (float)(field.GetValue(blackboard));
            }

            if( typeof(bool).IsAssignableFrom(type)){
                boolOut = (bool)(field.GetValue(blackboard));
            }

            if( typeof(GameObject).IsAssignableFrom(type)){
                
                objOut = (field.GetValue(blackboard) as GameObject).gameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;    
        }
    }
}