using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float minDist=5;
	public float maxDist=10;

	public CinemachineCameraOffset offset;

	void Start()
	{
		offset = GetComponent<CinemachineCameraOffset>();
        Cursor.visible = false; 
	}


	void Update()
	{
		if(Input.mouseScrollDelta.y !=0){
			offset.m_Offset.z += Input.mouseScrollDelta.y;
			offset.m_Offset.z = Mathf.Clamp(offset.m_Offset.z, minDist, maxDist);
		}
	}
}
