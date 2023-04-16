using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    public Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        Vector3 v = camera.transform.position - transform.position;

        v.x = v.z = 0f;
        transform.LookAt(camera.transform.position - v);

        transform.Rotate(0, 180, 0);
    }
}
