using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

public class TUA_PPSEffectsManager : MonoBehaviour
{
    public static TUA_PPSEffectsManager Instance { get; private set; } = null;


    //  public PostProcessingProfile ppProfile;
    //  private ColorGradingModel.Settings colorGradingSettings;

    public PostProcessVolume p_Volume;



    private void Awake()
    {
        Instance = this;
        //      ppProfile = Camera.main.GetComponent<PostProcessingBehaviour>().profile;
    }

    // Start is called before the first frame update
    void Start()
    {
       // ToggleGreyscale(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleGreyscale(bool isOn) {
  //      colorGradingSettings = ppProfile.colorGrading.settings;
        float targetValue;
        if (isOn) {
            targetValue = 1f;
        }
        else {
            targetValue = 0f;
        }
      //  settings.basic.saturation = 0f;

       // ppProfile.colorGrading.settings = settings;

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", p_Volume.weight,
            "to", targetValue,
            "time", 2f,
            "onupdatetarget", gameObject,
            "onupdate", "UpdateSaturationTween",
            "easetype", iTween.EaseType.easeOutQuad
            )
        );
    }

    void UpdateSaturationTween(float newValue) {
        /*
        Debug.Log("UpdateSaturationTween: " + newValue);
        colorGradingSettings.basic.saturation = newValue;
        ppProfile.colorGrading.settings = colorGradingSettings;
        */
        p_Volume.weight = newValue;
    }
}
