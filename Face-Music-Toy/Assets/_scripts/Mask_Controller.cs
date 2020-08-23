using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask_Controller : MonoBehaviour
{
    //General mask and mask movement;
    [Header("Mask Parent")]
    public GameObject maskParent;
    public Vector3 maskParent_targetPos;
    public float scaleMaskMovement = 0.01f;
    public float maskMoveSpeed = 0.01f;

    [Header("Mouth height settings")]
    public GameObject maskMouthBot;
    public Vector3 maskMouthBot_startingPos;
    public Vector3 maskMouthBot_targetPos;
    public float maskMouthBot_maxMovement = 1f;
    public float maskMouthBot_moveSpeed = 0.01f;

    public GameObject maskMouthTop;
    public Vector3 maskMouthTop_startingPos;
    public Vector3 maskMouthTop_targetPos;
    public float maskMouthTop_maxMovement = 1f;
    public float maskMouthTop_moveSpeed = 0.01f;

    public float minMouthValue = 0.2f;
    public float mouthTriggerSensitivity = 0.5f;
    public float mouthTriggerValue = 0f;
    public bool mouthTriggerBool = false;

    [Header("Mouth width settings")]
    public GameObject maskMouthSide_parent;

    public GameObject maskMouthSide_L;
    public Vector3 maskMouthSide_L_startingPos;
    public Vector3 maskMouthSide_L_targetPos;

    public GameObject maskMouthSide_R;
    public Vector3 maskMouthSide_R_startingPos;
    public Vector3 maskMouthSide_R_targetPos;

    public float maskMouthSide_maxMovement = 1f;
    public float maskMouthSide_moveSpeed = 0.01f;

    public float minMouthWidthValue = 0.2f;
    public float mouthWidthTriggerSensitivity = 0f;
    public float mouthWidthTriggerValue = 0f;
    public bool mouthWidthTriggerBool = false;

    [Header("Eyebrow L height settings")]
    public GameObject maskEyebrow_L;
    public Vector3 maskEyebrow_L_startingPos;
    public Vector3 maskEyebrow_L_targetPos;
    public float maskEyebrow_L_maxMovement = 1f;
    public float maskEyebrow_L_moveSpeed = 0.01f;

    public float minEyebrowLValue = 0.2f;
    public float eyebrowTriggerSensitivity_L = 0.5f;
    public float eyebrowLTriggerValue = 0f;
    public bool eyebrowLTriggerBool = false;

    [Header("Eyebrow R height settings")]
    public GameObject maskEyebrow_R;
    public Vector3 maskEyebrow_R_startingPos;
    public Vector3 maskEyebrow_R_targetPos;
    public float maskEyebrow_R_maxMovement = 1f;
    public float maskEyebrow_R_moveSpeed = 0.01f;

    public float minEyebrowRValue = 0.2f;
    public float eyebrowTriggerSensitivity_R = 0.5f;
    public float eyebrowRTriggerValue = 0f;
    public bool eyebrowRTriggerBool = false;

    [Header("Face Sounds")]
    public AudioClip bassDrum_1;
    public AudioClip highHat_1;
    public AudioClip snare_1;

    public AudioClip synth_1;


    // Start is called before the first frame update
    void Start()
    {
        MaskElements_SetStartPositions();
    }

    // Update is called once per frame
    void Update()
    {
        MaskElements_UpdatePositions();
        CheckTriggers();
    }

    private void MaskElements_SetStartPositions() {
        maskMouthBot_startingPos = maskMouthBot.transform.localPosition;
        maskMouthTop_startingPos = maskMouthTop.transform.localPosition;

        maskEyebrow_L_startingPos = maskEyebrow_L.transform.localPosition;
        maskEyebrow_R_startingPos = maskEyebrow_R.transform.localPosition;

        maskMouthSide_L_startingPos = maskMouthSide_L.transform.localPosition;
        maskMouthSide_R_startingPos = maskMouthSide_R.transform.localPosition;

    }

    private void MaskElements_UpdatePositions()
    {
        Debug.Log("MaskElements_UpdatePositions:" + gameObject.name);

        maskEyebrow_L.transform.localPosition = Vector3.MoveTowards(maskEyebrow_L.transform.localPosition, maskEyebrow_L_targetPos, maskEyebrow_L_moveSpeed * Time.deltaTime);
        maskEyebrow_R.transform.localPosition = Vector3.MoveTowards(maskEyebrow_R.transform.localPosition, maskEyebrow_R_targetPos, maskEyebrow_R_moveSpeed * Time.deltaTime);
        maskMouthBot.transform.localPosition = Vector3.MoveTowards(maskMouthBot.transform.localPosition, maskMouthBot_targetPos, maskMouthBot_moveSpeed * Time.deltaTime);
        maskMouthTop.transform.localPosition = Vector3.MoveTowards(maskMouthTop.transform.localPosition, maskMouthTop_targetPos, maskMouthTop_moveSpeed * Time.deltaTime);

        maskMouthSide_L.transform.localPosition = Vector3.MoveTowards(maskMouthSide_L.transform.localPosition, maskMouthSide_L_targetPos, maskMouthSide_moveSpeed * Time.deltaTime);
        maskMouthSide_R.transform.localPosition = Vector3.MoveTowards(maskMouthSide_R.transform.localPosition, maskMouthSide_R_targetPos, maskMouthSide_moveSpeed * Time.deltaTime);

        //Center the mouth sides vertically (mouth top and bot can move different amounts)
        maskMouthSide_parent.transform.localPosition = Vector3.Lerp(maskMouthBot.transform.localPosition, maskMouthTop.transform.localPosition, 0.5f);
        maskMouthSide_parent.transform.localScale = new Vector3(maskMouthSide_parent.transform.localScale.x,
                                                                Vector3.Distance(maskMouthBot.transform.localPosition, maskMouthTop.transform.localPosition) + 0.2f,
                                                                maskMouthSide_parent.transform.localScale.z);

    }

    private void CheckTriggers()
    {

        if(HasTriggerChanged(ref mouthTriggerBool, mouthTriggerValue, mouthTriggerSensitivity)) {
            //Debug.Log("Mouth value has changed!");
            //if (mouthTriggerBool) { TUA_AudioManager.Instance.PlayAudioSFX(bassDrum_1); }
            if (mouthTriggerBool) { TUA_AudioManager.Instance.PlayAudioLoop(synth_1); }
            else {
                TUA_AudioManager.Instance.StopAudioLoop();
            }

        }

        /*
        if (HasTriggerChanged(ref eyebrowLTriggerBool, eyebrowLTriggerValue, eyebrowTriggerSensitivity_L))
        {
            Debug.Log("Eyebrow value has changed!");
            //if (mouthTriggerBool) { TUA_AudioManager.Instance.PlayAudioSFX(bassDrum_1); }
            if (eyebrowLTriggerBool) { TUA_AudioManager.Instance.PlayAudioLoop(synth_1); }

        }*/

        TUA_AudioManager.Instance.AudioLoopAdjustVolume(mouthTriggerValue);

        TUA_AudioManager.Instance.AudioLoopAdjustPitch(mouthWidthTriggerValue);

        /*
        if (HasTriggerChanged(ref eyebrowLTriggerBool, eyebrowLTriggerValue, eyebrowTriggerSensitivity_L))
        {
            Debug.Log("EyebrowL value has changed!");
            if (eyebrowLTriggerBool) { TUA_AudioManager.Instance.PlayAudioSFX(highHat_1); }
        }

        if (HasTriggerChanged(ref eyebrowRTriggerBool, eyebrowRTriggerValue, eyebrowTriggerSensitivity_R))
        {
            Debug.Log("EyebrowR value has changed!");
            if (eyebrowRTriggerBool) { TUA_AudioManager.Instance.PlayAudioSFX(snare_1); }
        }
        */
    }

    public bool HasTriggerChanged(ref bool elementTriggerBool, float elementTriggerValue, float elementTriggerSensitivity) {
        bool hasChangedBool = false;

        if (!elementTriggerBool)
        {
            if (elementTriggerValue > elementTriggerSensitivity)
            {
                elementTriggerBool = true;
                hasChangedBool = true;
            }
        }
        else
        {
            if (elementTriggerValue < elementTriggerSensitivity)
            {
                elementTriggerBool = false;
                hasChangedBool = true;
            }
        }


        return hasChangedBool;
    }

    public void EyebrowL_UpdateValue(float value)
    {

        if (value < minEyebrowLValue)
        {
            value = 0f;
        }
        eyebrowLTriggerValue = value;

        //Eyebrow L
        maskEyebrow_L_targetPos = new Vector3(maskEyebrow_L_startingPos.x,
               maskEyebrow_L_startingPos.y + (eyebrowLTriggerValue * maskEyebrow_L_maxMovement),
               maskEyebrow_L_startingPos.z
           );
        //maskEyebrow_L.transform.localPosition = Vector3.MoveTowards(maskEyebrow_L.transform.localPosition, maskEyebrow_L_targetPos, maskEyebrow_L_moveSpeed * Time.deltaTime);
    }


    public void EyebrowR_UpdateValue(float value)
    {

        if (value < minEyebrowRValue)
        {
            value = 0f;
        }
        eyebrowRTriggerValue = value;

        //Eyebrow R
        maskEyebrow_R_targetPos = new Vector3(maskEyebrow_R_startingPos.x,
               maskEyebrow_R_startingPos.y + (eyebrowRTriggerValue * maskEyebrow_R_maxMovement),
               maskEyebrow_R_startingPos.z
           );
        //maskEyebrow_R.transform.localPosition = Vector3.MoveTowards(maskEyebrow_R.transform.localPosition, maskEyebrow_R_targetPos, maskEyebrow_R_moveSpeed * Time.deltaTime);
    }

    public void Mouth_UpdateValue(float value)
    {
        if (value < minMouthValue)
        {
            value = 0f;
        }
        mouthTriggerValue = value;

        //Bot
        maskMouthBot_targetPos = new Vector3(maskMouthBot_startingPos.x,
                   maskMouthBot_startingPos.y + (mouthTriggerValue * maskMouthBot_maxMovement),
                   maskMouthBot_startingPos.z
               );
            //maskMouthBot.transform.localPosition = Vector3.MoveTowards(maskMouthBot.transform.localPosition, maskMouthBot_targetPos, maskMouthBot_moveSpeed * Time.deltaTime);

            //Top
            maskMouthTop_targetPos = new Vector3(maskMouthTop_startingPos.x,
                   maskMouthTop_startingPos.y + (mouthTriggerValue * maskMouthTop_maxMovement),
                   maskMouthTop_startingPos.z
               );
            //maskMouthTop.transform.localPosition = Vector3.MoveTowards(maskMouthTop.transform.localPosition, maskMouthTop_targetPos, maskMouthTop_moveSpeed * Time.deltaTime);
        
    }

    public void MouthWidth_UpdateValue(float value)
    {
        if (value < minMouthWidthValue)
        {
            value = 0f;
        }
        mouthWidthTriggerValue = value;

        //Left 
        maskMouthSide_L_targetPos = new Vector3(maskMouthSide_L_startingPos.x - (mouthWidthTriggerValue * maskMouthSide_maxMovement),
                   maskMouthSide_L_startingPos.y,
                   maskMouthSide_L_startingPos.z
               );
        //maskMouthBot.transform.localPosition = Vector3.MoveTowards(maskMouthBot.transform.localPosition, maskMouthBot_targetPos, maskMouthBot_moveSpeed * Time.deltaTime);

        //Right
        maskMouthSide_R_targetPos = new Vector3(maskMouthSide_R_startingPos.x + (mouthWidthTriggerValue * maskMouthSide_maxMovement),
                  maskMouthSide_R_startingPos.y,
                  maskMouthSide_R_startingPos.z
              );
        //maskMouthTop.transform.localPosition = Vector3.MoveTowards(maskMouthTop.transform.localPosition, maskMouthTop_targetPos, maskMouthTop_moveSpeed * Time.deltaTime);

    }

}
