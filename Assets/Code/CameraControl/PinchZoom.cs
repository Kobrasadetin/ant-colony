using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.1f;        // The rate of change of the orthographic size in orthographic mode.
	public float scrollSensitivity = 1f;
	private float currentSpeed = 0f;
	private float damping = 0.67f;

	private Camera mainCamera;
	

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			float oldsize = mainCamera.orthographicSize;
			mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * mainCamera.orthographicSize;
			currentSpeed =  mainCamera.orthographicSize - oldsize;
		} else {
			currentSpeed *= damping;
			mainCamera.orthographicSize += currentSpeed;
		}

		// If there are two touches on the device...
		if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (mainCamera.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                mainCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                mainCamera.orthographicSize = Mathf.Max(mainCamera.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                mainCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 0.1f, 179.9f);
            }
        }
		mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, 0.1f, 50f);
	}
}