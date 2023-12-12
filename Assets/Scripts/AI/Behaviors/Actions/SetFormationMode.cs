using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    
    [Action("SetFormationMode")]
    [Help("Cambia el modo de la formación")]
    public class SetFormationMode : GOAction
    {

        [InParam("mode")]
        [Help("Nuevo modo de la formación")]
        public FormationShape mode;

        private Formation formation;

        /// <summary>Initialization Method of MoveToPosition.</summary>
        /// <remarks>Check if there is a NavMeshAgent to assign a default one and assign the destination to the NavMeshAgent the given position.</remarks>
        public override void OnStart()
        {
            formation = gameObject.GetComponent<Formation>();

            formation.shape = mode;
            formation.applyChanges();

            
        }

        /// <summary>Method of Update of MoveToPosition </summary>
        /// <remarks>Check the status of the task, if it has traveled the road or is close to the goal it is completed
        /// and otherwise it will remain in operation.</remarks>
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }

        /// <summary>Abort method of MoveToPosition.</summary>
        /// <remarks>When the task is aborted, it stops the navAgentMesh.</remarks>
        public override void OnAbort()
        {


        }
    }
}
