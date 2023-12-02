using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Basado en código de https://github.com/Firnox/Billboarding/blob/main/Billboard.cs

/// <summary>
/// Se encarga de gestionar los bocadillos de diálogo de los personajes
/// </summary>
[ExecuteAlways] //Para facilitar depuración
public class SpeechBubble : MonoBehaviour {
	[SerializeField] private BillboardType billboardType;

	[Header("Lock Rotation")]
	[SerializeField] private bool lockX;
	[SerializeField] private bool lockY;
	[SerializeField] private bool lockZ;


	[Header("Elements")]
	public Image background;
	public Text text;
	public Image image;

	private Vector3 originalRotation;

	
	public enum BillboardType { LookAtCamera, CameraForward };

	public bool needsUpdate;

	
	private void Awake() {
		originalRotation = transform.rotation.eulerAngles;
	}


	void Start()
	{
		if(Application.isPlaying)
			clearBubble();
	}

	
	

	void OnRenderObject(){

		if(needsUpdate){
			needsUpdate = false;
			background.enabled = image.enabled = image.sprite != null;
		}

		var camera = SceneView.currentDrawingSceneView?.camera;
		camera ??= Application.isPlaying? Camera.main : null;
		if (camera == null)
			return;
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


	
	/// <summary>
	/// Resetea el bocadillo
	/// </summary>
	public void clearBubble(){
		image.sprite = null;
		needsUpdate = true;
		Debug.Log("message Clear");
	}

	public void say(Sprite emoji){
		
		image.sprite = emoji;
		needsUpdate = true;
		Debug.Log("message Changed");
	}
}

