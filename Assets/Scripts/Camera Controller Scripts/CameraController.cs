using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    private CinemachineVirtualCamera vcam;

    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 60f;

    [SerializeField] private StarterAssetsInputs _input;

    [SerializeField] private float mouseX, mouseY;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();

        _input = GetComponent<StarterAssetsInputs>();

        vcam.m_Lens.FieldOfView = Mathf.Clamp(vcam.m_Lens.FieldOfView, minZoom, maxZoom);
    }

    private void Update()
    {
        if (_input.shoot)
        {
            mouseX = _input.look.x * rotationSpeed;
            mouseY = _input.look.y * rotationSpeed;

            vcam.transform.RotateAround(vcam.Follow.transform.position, Vector3.up, mouseX);
            vcam.transform.RotateAround(vcam.Follow.transform.position, vcam.transform.right, -mouseY);
        }

        //if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        //{
        //    if(vcam.m_Lens.FieldOfView > minZoom)
        //        vcam.m_Lens.FieldOfView--;
        //}
        //else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        //{
        //    if(vcam.m_Lens.FieldOfView < maxZoom)
        //        vcam.m_Lens.FieldOfView++;
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ResetToInitialRotation();
        //}

        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    HideAnnotations();
        //}
    }

    public void ResetToInitialRotation()
    {
        InfoUIManager.instance.ShowAnnotations();
    }

    public void HideAnnotations()
    {
        InfoUIManager.instance.HideAnnotations();
    }
}
