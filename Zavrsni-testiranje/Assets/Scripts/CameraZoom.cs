using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 10f;
    [SerializeField]
    private float minZoom = 60f;
    [SerializeField]
    private float maxZoom = 100f;

    private Camera zoomCamera;

    private void Start()
    {
        zoomCamera = Camera.main;
    }

    private void Update()
    {
        if (zoomCamera.orthographic)
        {
            zoomCamera.orthographicSize = Mathf.Clamp(zoomCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, minZoom, maxZoom);
        }
        else
        {
            zoomCamera.fieldOfView = Mathf.Clamp(zoomCamera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, minZoom, maxZoom);
        }
    }
}
