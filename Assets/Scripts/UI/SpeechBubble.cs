using UnityEditor;
using UnityEngine;

// Basado en código de https://github.com/Firnox/Billboarding/blob/main/Billboard.cs

[ExecuteAlways] //Para facilitar depuración
public class SpeechBubble : MonoBehaviour {
	[SerializeField] private BillboardType billboardType;

	[Header("Lock Rotation")]
	[SerializeField] private bool lockX;
	[SerializeField] private bool lockY;
	[SerializeField] private bool lockZ;

	private Vector3 originalRotation;

	
	public enum BillboardType { LookAtCamera, CameraForward };

	
	private void Awake() {
		originalRotation = transform.rotation.eulerAngles;
	}


	

	void OnRenderObject(){
		var camera = SceneView.currentDrawingSceneView?.camera;
		camera ??= Camera.main;
		// There are two ways people billboard things.
		switch (billboardType) {
			case BillboardType.LookAtCamera:
			transform.LookAt(camera.transform.position, Vector3.up);
			break;
			case BillboardType.CameraForward:
			transform.forward = camera.transform.forward;
			break;
			default:
			break;
	}

	// Modify the rotation in Euler space to lock certain dimensions.
	Vector3 rotation = transform.rotation.eulerAngles;
	if (lockX) { rotation.x = originalRotation.x; }
	if (lockY) { rotation.y = originalRotation.y; }
	if (lockZ) { rotation.z = originalRotation.z; }
	transform.rotation = Quaternion.Euler(rotation);
  }
}

