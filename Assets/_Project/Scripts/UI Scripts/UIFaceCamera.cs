using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    private Camera _cam;

    // Update is called once per frame
    private void Update()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
        }

        transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward,
            _cam.transform.rotation * Vector3.up);
    }
}
