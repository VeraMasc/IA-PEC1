using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using System.Diagnostics;

namespace BBUnity.Actions
{
    /// <summary>
    /// </summary>
    [Action("Basic/IncreaseFloat")]
    [Help("Sets a value to a float variable")]
    public class IncreaseFloat : BasePrimitiveAction
    {

        [InParam("invar")]
        [Help("input variable")]
        public float invar;

        ///<value>OutPut Float Parameter.</value>
        [OutParam("var")]
        [Help("output variable")]
        public float var;

        ///<value>Input Float Parameter.</value>
        [InParam("value")]
        [Help("Value")]
        public float value;

        /// <summary>Initialization Method of SetFloat</summary>
        /// <remarks>Initializes the Float value.</remarks>
        public override void OnStart()
        {
            UnityEngine.Debug.Log($"{invar}+{value}");
            var = invar + value;
            
        }

        /// <summary>Method of Update of SetFloat.</summary>
        /// <remarks>Complete the task.</remarks>
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}
