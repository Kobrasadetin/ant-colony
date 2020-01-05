using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 dragOrigin; //Where are we moving?
    private Vector3 clickOrigin = Vector3.zero; //Where are we starting?
    private Vector3 basePos = Vector3.zero; //Where should the camera be initially?
	private Camera cam;
	public bool DragEnabled;

	private void Start()
	{
		cam = Camera.main;
	}

	void Update()
    {
		if (DragEnabled && Input.touchCount <= 1)
		{
			if (Input.GetMouseButton(0) || Input.GetMouseButton(2))
			{
				if (clickOrigin == Vector3.zero)
				{
					clickOrigin = Input.mousePosition;
					basePos = transform.position;
				}
				dragOrigin = Input.mousePosition;
			}

			if (!Input.GetMouseButton(0) && !Input.GetMouseButton(2))
			{
				clickOrigin = Vector3.zero;
				return;
			}
			Vector3 newPosition = new Vector3(basePos.x + ((clickOrigin.x - dragOrigin.x) * .008f * cam.orthographicSize), basePos.y + ((clickOrigin.y - dragOrigin.y) * .008f * cam.orthographicSize), cam.transform.position.z);
			newPosition = new Vector3(Mathf.Clamp(newPosition.x, -20, 20), Mathf.Clamp(newPosition.y, -20, 20), newPosition.z);
			transform.position = newPosition;
		}
    }
}