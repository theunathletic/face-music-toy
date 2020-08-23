using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TUA_FaceDetectorManager : MonoBehaviour
{
    public static TUA_FaceDetectorManager Instance { get; private set; } = null;

    public TUA_FaceDetector TUA_fd;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleFaceDetector_CameraTexture(bool t)
    {
        TUA_fd.ToggleShowCameraTexture(t);
        TUA_fd.ToggleShowRenderTexture(t);
    }

    public void ToggleFaceDetector_ProcessTexture(bool t)
    {
        TUA_fd.ToggleShouldProcessTexure(t);
    }

    public void ToggleFaceDetector_FaceLines(bool t)
    {
        TUA_fd.ToggleShowLines(t);
    }

    public void ToggleFaceDetector_FaceSizeGuides(bool t)
    {
        TUA_fd.ToggleFaceSizeGuides(t);
    }
}
