
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;

//Sacado de https://forum.unity.com/threads/navmesh-modifier-or-navmesh-modifier-volume-at-run-time.487632/

[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    public bool useCollider;
    public static List<MeshFilter> Meshes = new List<MeshFilter>();

    public static List<Collider> Shapes = new List<Collider>();
    public static List<NavMeshModifierVolume> VolumeModifiers = new List<NavMeshModifierVolume>();
    public static int AgentTypeId;
 
    //----------------------------------------------------------------------------------------
    private void OnEnable()
    {
        var volumes = GetComponents<NavMeshModifierVolume>();
        if(volumes != null)
            VolumeModifiers.AddRange(volumes);
 
        var modifier = GetComponent<NavMeshModifier>();
        if ((modifier != null) && (!modifier.AffectsAgentType(AgentTypeId) || (modifier.ignoreFromBuild) && modifier.AffectsAgentType(AgentTypeId)))
            return;
        
        var meshes = GetComponentsInChildren<MeshFilter>();
        if (meshes != null && meshes.Length > 0)
            Meshes.AddRange(meshes);
        
        if(useCollider){
            var colliders = GetComponentsInChildren<Collider>();
            if (colliders != null && colliders.Length > 0){
                Shapes.AddRange(colliders);
            }
                
        }
    }
 
    //----------------------------------------------------------------------------------------
    private void OnDisable()
    {
        var volumes = GetComponents<NavMeshModifierVolume>();
        if (volumes != null)
        {
            for (int index = 0; index < volumes.Length; index++)
                VolumeModifiers.Remove(volumes[index]);
        }
 
        var modifier = GetComponent<NavMeshModifier>();
        if((modifier != null) && (modifier.ignoreFromBuild))
            return;
 
        var mesh = GetComponent<MeshFilter>();
        if(mesh != null)
            Meshes.Remove(mesh);

        if(useCollider){
            var colliders = GetComponentsInChildren<Collider>();
            if (colliders != null ){
                for (int index = 0; index < volumes.Length; index++)
                    Shapes.Remove(colliders[index]);
            }
                
        }
    }
 
    //----------------------------------------------------------------------------------------
    public static void CollectMeshes(ref List<NavMeshBuildSource> _sources)
    {
        _sources.Clear();
        for (var i = 0; i < Meshes.Count; ++i)
        {
            var mf = Meshes[i];

            if (mf == null)
                continue;
 
            var m = mf.sharedMesh;
            if (m == null)
                continue;
 
            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            var modifier = mf.GetComponent<NavMeshModifier>();
    
            s.area = modifier && modifier.overrideArea ? modifier.area : 0;
            _sources.Add(s);
        }
    }

    //----- Custom
    public static void CollectShapes(ref List<NavMeshBuildSource> _sources)
    {
        for (var i = 0; i < Shapes.Count; ++i)
        {
            var col = Shapes[i];

            if (col == null)
                continue;
 
            
 
            var s = new NavMeshBuildSource();
            if(col.GetType() == typeof(SphereCollider)){
                s.shape = NavMeshBuildSourceShape.Sphere;
                s.size = (col as SphereCollider).radius * Vector3.one;
                s.transform = col.transform.localToWorldMatrix;

                var modifier = col.GetComponent<NavMeshModifier>();
                s.area = modifier && modifier.overrideArea ? modifier.area : 0;
                Debug.Log(modifier.area);
                _sources.Add(s);
            }
            

            
        }
    }
 
    //----------------------------------------------------------------------------------------
    public static void CollectModifierVolumes(int _layerMask, ref List<NavMeshBuildSource> _sources)
    {
        foreach (var m in VolumeModifiers)
        {
            if ((_layerMask & (1 << m.gameObject.layer)) == 0)
                continue;
            if (!m.AffectsAgentType(AgentTypeId))
                continue;
 
            var mcenter = m.transform.TransformPoint(m.center);
            var scale = m.transform.lossyScale;
            var msize = new Vector3(m.size.x * Mathf.Abs(scale.x), m.size.y * Mathf.Abs(scale.y), m.size.z * Mathf.Abs(scale.z));
 
            var src = new NavMeshBuildSource();
            src.shape = NavMeshBuildSourceShape.ModifierBox;
            src.transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one);
            src.size = msize;
            src.area = m.area;
            _sources.Add(src);
        }
    }
}