using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask_Input : MonoBehaviour
{
    public Mask_Controller m_controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_controller.EyebrowL_UpdateValue(TUA_FaceController.Instance.GetNormalizedEyebrowLValue());
        m_controller.EyebrowR_UpdateValue(TUA_FaceController.Instance.GetNormalizedEyebrowRValue());
        m_controller.Mouth_UpdateValue(TUA_FaceController.Instance.GetNormalizedMouthValue());

        m_controller.MouthWidth_UpdateValue(TUA_FaceController.Instance.GetNormalizedMouthWidthValue());

        MaskSelectionController.Instance.SetHeadTurnValue(TUA_FaceController.Instance.GetNormalizedHEadTurnValue());

    }

    public void SetActiveController(Mask_Controller controller) {
        m_controller = controller;
    }
}
