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

        [InParam("hasDuration")]
        [Help("Hace que el mensaje dure un tiempo determinado")]
        public bool hasDuration;

        [InParam("isEndless")]
        [Help("Se ejecuta eternamente hasta que algo la obligue a abortar")]
        public bool isEndless;

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
            if(isEndless){
                return TaskStatus.SUSPENDED;
            }

            if (!hasDuration || duration<=0){
                
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
            if(hasDuration || isEndless)
                speech.clearBubble();
        }

        public override void OnFailedEnd()
        {
            speech.clearBubble();
        }
        
        
    }
}
