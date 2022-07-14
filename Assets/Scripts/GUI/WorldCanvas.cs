using UnityEngine;

// Move this object, expected to be a GUI panel on an 
// overlay canvas, to the 3d parent of the canvas
public class WorldCanvas : MonoBehaviour
{
	private Camera mainCamera;
	private Transform parent3d;

	public void Start()
	{
		mainCamera = Camera.main;
        parent3d = transform.parent.parent;
	}
	public void LateUpdate()
	{
		Vector3 locationPosition = parent3d.position;
		transform.position = mainCamera.WorldToScreenPoint(locationPosition);
	}
}
