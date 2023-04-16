using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    private CinemachineVirtualCamera vcam;

    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 60f;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();

        vcam.m_Lens.FieldOfView = Mathf.Clamp(vcam.m_Lens.FieldOfView, minZoom, maxZoom);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            vcam.transform.RotateAround(vcam.Follow.transform.position, Vector3.up, mouseX);
            vcam.transform.RotateAround(vcam.Follow.transform.position, vcam.transform.right, -mouseY);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            vcam.m_Lens.FieldOfView--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            vcam.m_Lens.FieldOfView++;
        }
    }
}
