using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TUA_DebugManager : MonoBehaviour
{
    //-------------------------------------------------------------------------------------------------------------------------
    public GameObject screen_debugWindow;
    [SerializeField]
    private bool toggle_debugWindow = false;

    public GameObject btn_toggleDebugWindow;
    public Toggle ui_toggle_debugBtn;
    [SerializeField]
    private bool toggle_debugBtn = true;

    //-------------------------------------------------------------------------------------------------------------------------
    public GameObject fps_counter;
    public Toggle ui_toggle_fps;
    [SerializeField]
    private bool toggle_fps = false;

    //-------------------------------------------------------------------------------------------------------------------------
    public Toggle ui_toggle_camera;
    [SerializeField]
    private bool toggle_camera = false;

    //-------------------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Application Version : " + Application.version);

        screen_debugWindow.SetActive(toggle_debugWindow);
        InitialiseDebugBtn();
        InitialiseFPS();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Debugger")){
            ToggleDebugWindow();
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    void ToggleDebugWindow()
    {
        toggle_debugWindow = !toggle_debugWindow;
        screen_debugWindow.SetActive(toggle_debugWindow);
    }

    public void UI_ToggleDebugWindow()
    {
        ToggleDebugWindow();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    void InitialiseDebugBtn() {
        ui_toggle_debugBtn.isOn = toggle_debugBtn;
    }

    public void UI_ToggleDebugBtn(bool toggle)
    {
        toggle_debugBtn = toggle;
        btn_toggleDebugWindow.SetActive(toggle_debugBtn);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    void InitialiseFPS()
    {
        ui_toggle_fps.isOn = toggle_fps;
    }

    public void UI_ToggleFPS(bool toggle) {
        fps_counter.SetActive(toggle);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public void UI_ToggleCamera(bool toggle)
    {
        FaceTracker.Instance.ToggleFaceDetector_CameraTexture(toggle);
    }

    public void UI_ToggleProcessTexture(bool toggle)
    {
        FaceTracker.Instance.ToggleFaceDetector_ProcessTexture(toggle);
    }

    public void UI_ToggleFaceLines(bool toggle)
    {
        FaceTracker.Instance.ToggleFaceDetector_FaceLines(toggle);
    }


}
