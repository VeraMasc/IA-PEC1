using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;


namespace BBUnity.Actions
{
    /// <summary>
    /// 
    /// </summary>
    [Action("Speak")]
    [Help("Dice un mensaje mediante un bocadillo")]
    public class Speak : GOAction
    {
        

        [InParam("speechBubble")]
        [Help("")]
        public SpeechBubble speech;

        [InParam("message")]
        [Help("")]
        public Sprite message;

        [InParam("duration")]
        [Help("")]
        public float duration;

        /// <summary></summary>
        public override void OnStart()
        {
            speech.say(message);
        }
        /// <summary>Method of Update of MoveToRandomPosition </summary>
        /// <remarks>Check the status of the task, if it has traveled the road or is close to the goal it is completed
        /// and otherwise it will remain in operation.</remarks>
        public override TaskStatus OnUpdate()
        {
            if (duration<=0){
                
                return TaskStatus.COMPLETED;
            }
                
            duration -= Time.deltaTime;
            return TaskStatus.RUNNING;
        }

        
        /// <summary>Abort method of MoveToRandomPosition </summary>
        public override void OnAbort()
        {
            speech.clearBubble();
        }
        
        public override void OnEnd()
        {
            speech.clearBubble();
        }
        
    }
}
