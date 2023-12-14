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
    /// <summary>
    /// elemento encargado de renderizar el fondo (el "bocadillo") del mensaje
    /// </summary>
	public Image background;

    /// <summary>
    /// Texto del mensaje. No se usa, está para hacer pruebas
    /// </summary>
	public Text text;

    /// <summary>
    /// Imagen que se muestra en el mensaje
    /// </summary>
	public Image image;

    /// <summary>
    /// Rotacion inicial del bocadillo. Se usa durante el proceso de "billboarding"
    /// </summary>
	private Vector3 originalRotation;

	/// <summary>
    /// Modos posibles de bilboarding
    /// </summary>
	public enum BillboardType { LookAtCamera, CameraForward };

    /// <summary>
    /// Indica si hay que actualizar el mensaje que se muestra
    /// </summary>
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


        //Hacer que siempre mire hacia la cámara
        #if DEBUG
		var camera = SceneView.currentDrawingSceneView?.camera;
		camera ??= Application.isPlaying? Camera.main : null;
        #else
        var camera =  Application.isPlaying? Camera.main : null;
        #endif
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

	}

    /// <summary>
    /// Rellena el bocadillo con una imagen
    /// </summary>
    /// <param name="emoji">Sprite de la imagen o null para borrar la imagen actual</param>
	public void say(Sprite emoji){
		
		image.sprite = emoji;
		needsUpdate = true;

	}
}

