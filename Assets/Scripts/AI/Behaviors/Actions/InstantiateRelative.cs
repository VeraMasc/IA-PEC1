using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    /// <summary>
    ///It is an action to clone a GameObject.
    /// </summary>
    [Action("GameObject/InstantiateRelative")]
    [Help("Clones the object original and returns the clone")]
    public class InstantiateRelative : GOAction
    {

        ///<value>Input Object to be cloned Parameter.</value>
        [InParam("original")]
        [Help("Object to be cloned")]
        public GameObject original;

        ///<value>Input position for the clone Parameter.</value>
        [InParam("position")]
        [Help("position for the clone")]
        public Vector3 position;

        [InParam("isPosRelative")]
        [Help("is position relative to the object")]
        public bool isPosRelative;

        [InParam("isRotRelative")]
        [Help("is rotation relative to the object")]
        public bool isRotRelative;

        [InParam("isOffsetRotated")]
        [Help("is the relative pos rotated with the parent object")]
        public bool isOffsetRotated;

        ///<value>OutPut instantiated game object Parameter.</value>
        [OutParam("instantiated")]
        [Help("Returned game object")]
        public GameObject instantiated;


        /// <summary>Initialization Method of Instantiate.</summary>
        /// <remarks>Installed a GameObject in the position and type dice.</remarks>
        public override void OnStart()
        {
            var targetPos = position;
            if(isPosRelative){
                if(isOffsetRotated){
                    targetPos = gameObject.transform.rotation * targetPos;
                }
                targetPos += gameObject.transform.position;
            }
            var targetRot = original.transform.rotation;
            if(isRotRelative){
                targetRot = gameObject.transform.rotation * targetRot;
            }
            original = GameObject.Instantiate(original,targetPos, targetRot) as GameObject;
        }

        /// <summary>Method of Update of Instantiate.</summary>
        /// <remarks>Complete the task.</remarks>
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}
