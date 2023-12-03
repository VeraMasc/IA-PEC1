using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;


namespace BBUnity.Actions
{
    /// <summary>
    /// 
    /// </summary>
    [Action("ShutUp")]
    [Help("Deja de hablar")]
    public class ShutUp: GOAction
    {
        
        [InParam("speechBubble")]
        [Help("")]
        public SpeechBubble speech;
        

        /// <summary></summary>
        public override void OnStart()
        {
            speech.clearBubble();
        }
        /// <summary>Method of Update of MoveToRandomPosition </summary>
        /// <remarks>Check the status of the task, if it has traveled the road or is close to the goal it is completed
        /// and otherwise it will remain in operation.</remarks>
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;    
        }

        
        
        
    }
}
