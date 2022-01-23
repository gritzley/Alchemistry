using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class CameraPreviewSelector : MonoBehaviour
{
    public List<Camera> cameras;
    private void OnEnable() => UpdateCameras();
    private void UpdateCameras() => cameras = GetComponentsInChildren<Camera>().ToList();
}
