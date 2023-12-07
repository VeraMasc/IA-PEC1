using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;



namespace BBUnity.Actions
{
    [Action("AccessBlackboard")]
    [Help("Deja de hablar")]
    public class AccessMyBlackboard : GOAction
    {

        

        [InParam("fieldName")]
        public string fieldName;

        [InParam("fieldOut")]
        public object fieldOut;
        MyBlackboard _blackboard;

        MyBlackboard blackboard {
            get =>_blackboard ??= gameObject.GetComponent<MyBlackboard>();
            
        }

        public override void OnStart()
        {
            blackboard.GetType().GetField(fieldName).GetValue(blackboard);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;    
        }
    }
}