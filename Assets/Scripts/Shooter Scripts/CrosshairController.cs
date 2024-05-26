using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Reference to your main camera.
    private RectTransform crosshairRectTransform;

    //Vector3 position to spawn the projectile
    public Vector3 spawnPosition = Vector3.zero;
    private void Start()
    {
        crosshairRectTransform = GetComponent<RectTransform>();
        if (mainCamera == null)
        {
            // If you haven't assigned the main camera in the Inspector, try to find it.
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Get the position of the crosshair in screen space.
        Vector2 screenPosition = crosshairRectTransform.position;

        // Convert the screen position to a world position.
        spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
    }
}
