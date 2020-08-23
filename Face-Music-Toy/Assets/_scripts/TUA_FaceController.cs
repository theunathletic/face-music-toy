using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;

public class TUA_FaceController : MonoBehaviour
{
    public static TUA_FaceController Instance { get; private set; } = null;


    public bool isTrackingFace = false;

    public Vector2 trackingAreaSize;

    //General mask and mask movement;
    public GameObject maskParent;
    public Vector3 maskParent_targetPos;
    public float scaleMaskMovement = 0.01f;
    public float maskMoveSpeed = 0.01f;

    /*
    public GameObject eyebrow_L;
    public GameObject eyebrow_R;
    */  

    //General face positioning and size
    public Vector2 faceRegionSize;
    public Vector2 faceRegionPos;

    public bool shouldSetBaseline = false;

    public Vector2 faceRegionSize_lastRecalculateSize;
    public float faceRegionSize_recalculateThreshold = 40f;

    public float faceRegionSize_minFaceSize = 150f;
    public float faceRegionSize_maxFaceSize = 150f;

    public RectTransform faceSizeGuide_min;
    public RectTransform faceSizeGuide_max;


    public bool isFaceCloseEnough = false;
    public bool isFaceFarEnough = true;

    public enum FaceTrackingStatus
    {
        TooFar,
        TooFarBuffer,
        Good,
        TooCloseBuffer,
        TooClose
    };

    public FaceTrackingStatus faceTrackingStatus = FaceTrackingStatus.TooFar;
    public FaceTrackingStatus nextFaceTrackingStatus;
    public float faceTrackingLossBufferTime = 2f;
    public float faceTrackingLoss_timer = 0f;
    private Coroutine faceTrackingLoss_coroutine;


    //Real face element locations and dist calc.
    public Vector2 eyebrow_L_pos;
    public Vector2 eye_L_pos;
    public float eyeBrowDist_L;
    public float eyeBrowDist_L_min;
    public float eyeBrowDist_L_max;

    public Vector2 eyebrow_R_pos;
    public Vector2 eye_R_pos;
    public float eyeBrowDist_R;
    public float eyeBrowDist_R_min;
    public float eyeBrowDist_R_max;

    [Range(0.0f, 1.0f)]
    public float eyeBrowDist_min_ofFace;

    [Range(0.0f, 1.0f)]
    public float eyeBrowDist_max_ofFace;

    public Vector2 mouth_top;
    public Vector2 mouth_bot;
    public float mouthDist;
    public float mouthDist_min;
    public float mouthDist_max;

    [Range(0.0f, 1.0f)]
    public float mouthDist_min_ofFace;

    [Range(0.0f, 1.0f)]
    public float mouthDist_max_ofFace;

    public Vector2 mouth_left;
    public Vector2 mouth_right;
    public float mouthWidthDist;
    public float mouthWidthDist_min;
    public float mouthWidthDist_max;

    [Range(0.0f, 1.0f)]
    public float mouthWidthDist_min_ofFace;

    [Range(0.0f, 1.0f)]
    public float mouthWidthDist_max_ofFace;

    //Normalized face element values.
    public float eyeBrowL_normalizedDistValue = 0;
    public float eyeBrowR_normalizedDistValue = 0;
    public float mouth_normalizedDistValue = 0;
    public float mouthWidth_normalizedDistValue = 0;


    //Not currently using this.
    public float smoothSpeed = 0f;
    public float eyeBrowL_smoothNormalizedDistValue = 0;
    public float eyeBrowR_smoothNormalizedDistValue = 0;
    public float mouth_smoothNormalizedDistValue = 0;
    public float mouthWidth_smoothNormalizedDistValue = 0;


    //Cused for measuring head rotation.
    public Vector2 nosePos;
    public float headTurnAmount;
    public float headTurnAmount_normalizedValue = 0;


    //Trigger trackers and sensitivities;
    /*
    public float mouthTrigger;
    public float mouthTriggerSensitivity = 0.5f;

    public float eyebrowTrigger_L;
    public float eyebrowTriggerSensitivity_L = 0.5f;

    public float eyebrowTrigger_R;
    public float eyebrowTriggerSensitivity_R = 0.5f;
    */

    //Mask element and movement
    /*
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

    public GameObject maskEyebrow_L;
    public Vector3 maskEyebrow_L_startingPos;
    public Vector3 maskEyebrow_L_targetPos;
    public float maskEyebrow_L_maxMovement = 1f;
    public float maskEyebrow_L_moveSpeed = 0.01f;

    public GameObject maskEyebrow_R;
    public Vector3 maskEyebrow_R_startingPos;
    public Vector3 maskEyebrow_R_targetPos;
    public float maskEyebrow_R_maxMovement = 1f;
    public float maskEyebrow_R_moveSpeed = 0.01f;

    public float eyebrowMoveSpeed = 1f;
    */





    /*
     Jaw = 0,

            LeftEyebrow,
            RightEyebrow,

            NoseBridge,
            Nose,

            LeftEye,
            RightEye,

            OuterLip,
            InnerLip
     * */

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        maskMouthBot_startingPos = maskMouthBot.transform.localPosition;
        maskMouthTop_startingPos = maskMouthTop.transform.localPosition;

        maskEyebrow_L_startingPos = maskEyebrow_L.transform.localPosition;
        maskEyebrow_R_startingPos = maskEyebrow_R.transform.localPosition;
        */
       // TUA_ScreenManager.Instance.OutsideOfScreenFlow_OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_trackingFail"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
        SwitchFaceTrackingStatus(FaceTrackingStatus.TooFarBuffer);

    }

    // Update is called once per frame
    void Update()
    {
        //Smooth values
        eyeBrowL_smoothNormalizedDistValue = Mathf.MoveTowards(eyeBrowL_smoothNormalizedDistValue, eyeBrowL_normalizedDistValue, smoothSpeed);
        eyeBrowR_smoothNormalizedDistValue = Mathf.MoveTowards(eyeBrowR_smoothNormalizedDistValue, eyeBrowR_normalizedDistValue, smoothSpeed);
        mouth_smoothNormalizedDistValue = Mathf.MoveTowards(mouth_smoothNormalizedDistValue, mouth_normalizedDistValue, smoothSpeed);

    }

    public void SetTrackingAreaSize(float w, float h)
    {
        trackingAreaSize = new Vector2(w, h);

        //Debug.Log("SetTrackingAreaSize: " + trackingAreaSize);

        SetMinFaceSize(trackingAreaSize.x);
        SetMaxFaceSize(trackingAreaSize.x);
    }

    public void SetTrackingState(bool isCurrentlyTracking)
    {
        if (isCurrentlyTracking != isTrackingFace)
        {
            isTrackingFace = isCurrentlyTracking;
            if (isTrackingFace)
            {
                TrackingFound();
            }
            else
            {
                TrackingLost();
            }
        }
    }

    private void TrackingFound()
    {
        //Debug.Log("FaceController: TrackingFound.");
        shouldSetBaseline = true;
        CheckFaceRegionSizeChange();
    }

    private void TrackingLost()
    {
      //  SetFaceCloseEnough(false);
        SwitchFaceTrackingStatus(FaceTrackingStatus.TooFarBuffer);
        //Debug.Log("FaceController: TrackingLost.");
    }



    //This might be useful for creating a representation of the face element positions with Unity 3D objects. 
    /*
    public void UpdateFaceElementPositions(DetectedFace face) {
        faceParent_targetPos = new Vector3((face.Region.Center.X - (trackingAreaSize.x / 2.0f)) * scaleFaceMovement,
            -(face.Region.Center.Y - (trackingAreaSize.y / 2.0f)) * scaleFaceMovement,
            faceParent.transform.position.z
        );

        faceRegionSize =  new Vector2(face.Region.Size.Width, face.Region.Size.Height);
        faceRegionPos = new Vector2(face.Region.Center.X, face.Region.Center.Y);

        faceParent.transform.position = Vector3.MoveTowards(faceParent.transform.position, faceParent_targetPos, faceMoveSpeed * Time.deltaTime);

        foreach (DetectedObject sub in face.Elements)
            {
                if (sub.Marks != null)
                {

                if(sub.Name == "Eyebrow_L") {
                    UpdateFaceElement(new Vector2(sub.Marks[0].X, sub.Marks[0].Y),
                           eyebrow_L,
                           false,
                           true,
                           eyebrowMoveSpeed);

                }

                if (sub.Name == "Eye_L")
                {
                    UpdateFaceElement(new Vector2(sub.Marks[0].X, sub.Marks[0].Y),
                        eye_L,
                        false,
                        true,
                        eyebrowMoveSpeed);
                        
                }

                if (sub.Name == "Eye_R")
                {
                    UpdateFaceElement(new Vector2(sub.Marks[0].X, sub.Marks[0].Y),
                        eye_R,
                        false,
                        true,
                        eyebrowMoveSpeed);

                }



                if (sub.Name == "Eyebrow_R")
                {
                    UpdateFaceElement(new Vector2(sub.Marks[0].X, sub.Marks[0].Y),
                          eyebrow_R,
                          false,
                          true,
                          eyebrowMoveSpeed);
                }

                // Debug.Log(sub.Name);


            }
        }
    }
       
    void UpdateFaceElement(Vector2 markPos,GameObject GO, bool updateX, bool updateY, float moveSpeed) {
        float relativePosX = Mathf.InverseLerp((faceRegionPos.x - (faceRegionSize.x / 2)), (faceRegionPos.x + (faceRegionSize.x / 2)), markPos.x);
        float relativePosY = Mathf.InverseLerp((faceRegionPos.y - (faceRegionSize.y / 2)), (faceRegionPos.y + (faceRegionSize.y / 2)), markPos.y);

        float moveToX = GO.transform.localPosition.x;
        float moveToY = GO.transform.localPosition.y;

        if (updateX) {
            moveToX = Mathf.Lerp(2f, -2f, relativePosX);
        }
        if (updateY)
        {
            moveToY = Mathf.Lerp(2f, -2f, relativePosY);
        }

        GO.transform.localPosition = Vector3.MoveTowards(GO.transform.localPosition,
        new Vector3(moveToX,
        moveToY,
        GO.transform.localPosition.z),
        moveSpeed * Time.deltaTime
        );
    }
}
*/
    public void UpdateFaceElementPositions(DetectedFace face)
    {
        //Calculate the overall size of the face area (how big is your face , how close to the camera are you)
        faceRegionSize = new Vector2(face.Region.Size.Width, face.Region.Size.Height);
        //Calculate the location of your face in the camera viewfinder. (0,0 is top left).
        faceRegionPos = new Vector2(face.Region.Center.X, face.Region.Center.Y);

        CheckFaceRegionSizeChange();

        UpdateTriggerDistances();

        //Moves the facePArent based on your faces lacation within the camera viewfinder. How much is controled by scaleFaceMovement.
       /*
         maskParent_targetPos = new Vector3((face.Region.Center.X - (trackingAreaSize.x / 2.0f)) * scaleMaskMovement,
            -(face.Region.Center.Y - (trackingAreaSize.y / 2.0f)) * scaleMaskMovement,
            maskParent.transform.position.z
        );
        maskParent.transform.position = Vector3.MoveTowards(maskParent.transform.position, maskParent_targetPos, maskMoveSpeed * Time.deltaTime);
        */
        foreach (DetectedObject sub in face.Elements)
        {
            if (sub.Marks != null)
            {

                if (sub.Name == "Eyebrow_L")
                {
                    eyebrow_L_pos = new Vector2(sub.Marks[1].X, sub.Marks[1].Y);
                }

                if (sub.Name == "Eye_L")
                {
                    eye_L_pos = new Vector2(sub.Marks[0].X, sub.Marks[0].Y);
                }

                if (sub.Name == "Eyebrow_R")
                {
                    eyebrow_R_pos = new Vector2(sub.Marks[3].X, sub.Marks[3].Y);
                }

                if (sub.Name == "Eye_R")
                {
                    eye_R_pos = new Vector2(sub.Marks[3].X, sub.Marks[3].Y);
                }

                if (sub.Name == "Lip_Inner")
                {
                    mouth_top = new Vector2(sub.Marks[2].X, sub.Marks[2].Y);
                    mouth_bot = new Vector2(sub.Marks[6].X, sub.Marks[6].Y);
                }

                if (sub.Name == "Lip_Outer")
                {
                    mouth_left = new Vector2(sub.Marks[0].X, sub.Marks[0].Y);
                    mouth_right = new Vector2(sub.Marks[6].X, sub.Marks[6].Y);
                }

                if(sub.Name == "Nose bridge") {
                    nosePos = new Vector2(sub.Marks[3].X, sub.Marks[3].Y);
                }







                // Debug.Log(sub.Name);


            }
        }
        /*
        if (isFaceCloseEnough)
        {
            CalculateAllDistances();
            CalculateAllNormalizedValues();
        }*/

        if (faceTrackingStatus == FaceTrackingStatus.Good)
        {
            CalculateAllDistances();
            CalculateAllNormalizedValues();
        }
    }

    public void SetMinFaceSize(float trackingAreaWidth) {
        faceRegionSize_minFaceSize = trackingAreaWidth*0.2f;
        faceSizeGuide_min.sizeDelta = new Vector2(faceRegionSize_minFaceSize, faceRegionSize_minFaceSize);
        
        CheckFaceRegionSizeChange();
    }

    public void SetMaxFaceSize(float trackingAreaWidth)
    {
        faceRegionSize_maxFaceSize = trackingAreaWidth*0.8f;
        faceSizeGuide_max.sizeDelta = new Vector2(faceRegionSize_maxFaceSize, faceRegionSize_maxFaceSize);

        CheckFaceRegionSizeChange();
    }

    //Check to see how much thie faceRegionSize has changed, if it's a lot we should reset the baselines for trigger.
    //The max and min values of face element distances change a lot depending on how close to the cmaera you are.
    private void CheckFaceRegionSizeChange()
    {
        /*
        if (Mathf.Abs(faceRegionSize_lastRecalculateSize.x - faceRegionSize.x) > faceRegionSize_recalculateThreshold)
        {
            shouldSetBaseline = true;
            Debug.Log("FaceRegionSize changed: UPDATE BASELINE");
        }
        */
        /*
        if (isFaceCloseEnough) {
            if (faceRegionSize.x < faceRegionSize_minFaceSize) {
                SetFaceCloseEnough(false);
            }
            else if (faceRegionSize.x > faceRegionSize_maxFaceSize) {
                SetFaceCloseEnough(false);
            }
        }
        else {
            if ( (faceRegionSize.x > faceRegionSize_minFaceSize) && (faceRegionSize.x < faceRegionSize_maxFaceSize))
            {
                SetFaceCloseEnough(true);
            }
        }
        */
        switch (faceTrackingStatus) {
            case FaceTrackingStatus.TooFar:
            case FaceTrackingStatus.TooFarBuffer:
                if ((faceRegionSize.x > faceRegionSize_minFaceSize) && (faceRegionSize.x < faceRegionSize_maxFaceSize))
                {
                    //SetFaceCloseEnough(true);
                    SwitchFaceTrackingStatus(FaceTrackingStatus.Good);
                }

                break;


            case FaceTrackingStatus.Good:
                if (faceRegionSize.x < faceRegionSize_minFaceSize)
                {
                    SwitchFaceTrackingStatus(FaceTrackingStatus.TooFarBuffer);
                }
                else if (faceRegionSize.x > faceRegionSize_maxFaceSize)
                {

                    SwitchFaceTrackingStatus(FaceTrackingStatus.TooCloseBuffer);
                }

                break;

            case FaceTrackingStatus.TooClose:
            case FaceTrackingStatus.TooCloseBuffer:
                if ((faceRegionSize.x > faceRegionSize_minFaceSize) && (faceRegionSize.x < faceRegionSize_maxFaceSize))
                {
                    //SetFaceCloseEnough(true);
                    SwitchFaceTrackingStatus(FaceTrackingStatus.Good);
                }

                break;
        }

    }

    void SwitchFaceTrackingStatus(FaceTrackingStatus nextState)
    {
        faceTrackingStatus = nextState;
        switch (faceTrackingStatus)
        {
            //Do any game setup here. Spawn player and level etc.
            case FaceTrackingStatus.TooFar:
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_trackingFail"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
                TUA_PPSEffectsManager.Instance.ToggleGreyscale(true);
                break;

            case FaceTrackingStatus.TooFarBuffer:
                //nextFaceTrackingStatus = FaceTrackingStatus.TooFar;
                //StartCoroutine("SwitchFaceTrackingBuffer");
                faceTrackingLoss_coroutine = StartCoroutine(SwitchFaceTrackingStatusWithBuffer(faceTrackingLossBufferTime, FaceTrackingStatus.TooFar));
               // StartCoroutine(faceTrackingLoss_coroutine);
                break;

            case FaceTrackingStatus.Good:
                // StopAllCoroutines();
                if (faceTrackingLoss_coroutine != null)
                {
                    StopCoroutine(faceTrackingLoss_coroutine);
                }
               // StopCoroutine("SwitchFaceTrackingBuffer");
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_CloseSpecific(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_trackingFail"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_CloseSpecific(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_tracking_tooClose"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
                TUA_PPSEffectsManager.Instance.ToggleGreyscale(false);
                break;

            case FaceTrackingStatus.TooCloseBuffer:
                //nextFaceTrackingStatus = FaceTrackingStatus.TooClose;
                //StartCoroutine("SwitchFaceTrackingBuffer");
                faceTrackingLoss_coroutine = StartCoroutine(SwitchFaceTrackingStatusWithBuffer(faceTrackingLossBufferTime, FaceTrackingStatus.TooClose));
                //StartCoroutine(faceTrackingLoss_coroutine);
                break;

            case FaceTrackingStatus.TooClose:
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_tracking_tooClose"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
                TUA_PPSEffectsManager.Instance.ToggleGreyscale(true);
                break;
        }

    }

    public IEnumerator SwitchFaceTrackingStatusWithBuffer(float waitTime, FaceTrackingStatus nextState)
    {
       // print("SwitchFaceTrackingStatusWithBuffer START: " + Time.time);
       // while (true)
       // {
            yield return new WaitForSeconds(waitTime);
       //     print("SwitchFaceTrackingStatusWithBuffer END: " + Time.time);
            SwitchFaceTrackingStatus(nextState);
      //  }
    }

    public void SetFaceCloseEnough(bool isCurrentlyCloseEnough)
    {
        if (isCurrentlyCloseEnough != isFaceCloseEnough)
        {
            isFaceCloseEnough = isCurrentlyCloseEnough;
            if (isFaceCloseEnough)
            {
                //TrackingFound();
                Debug.Log("isFaceCloseEnough: " + isFaceCloseEnough);
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_CloseSpecific(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_trackingFail"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
            }
            else
            {
                Debug.Log("isFaceCloseEnough: " + isFaceCloseEnough);
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_trackingFail"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
                //TrackingLost();
            }
        }
    }

    public void SetFaceFarEnough(bool isCurrentlyFarEnough)
    {
        if (isCurrentlyFarEnough != isFaceFarEnough)
        {
            isFaceCloseEnough = isCurrentlyFarEnough;
            if (isFaceFarEnough)
            {
                //TrackingFound();
                Debug.Log("isFaceFarEnough: " + isFaceFarEnough);
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_CloseSpecific(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_tracking_tooClose"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
            }
            else
            {
                Debug.Log("isFaceFarEnough: " + isFaceFarEnough);
                TUA_ScreenManager.Instance.OutsideOfScreenFlow_OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_tracking_tooClose"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
                //TrackingLost();
            }
        }
    }

    //Update the needed trigger distances based on 
    private void UpdateTriggerDistances() {

        mouthDist_min = faceRegionSize.x * mouthDist_min_ofFace;
        mouthDist_max = faceRegionSize.x * mouthDist_max_ofFace;

        mouthWidthDist_min = faceRegionSize.x * mouthWidthDist_min_ofFace;
        mouthWidthDist_max = faceRegionSize.x * mouthWidthDist_max_ofFace;

        eyeBrowDist_L_min = faceRegionSize.x * eyeBrowDist_min_ofFace;
        eyeBrowDist_L_max = faceRegionSize.x * eyeBrowDist_max_ofFace;

        eyeBrowDist_R_min = faceRegionSize.x * eyeBrowDist_min_ofFace;
        eyeBrowDist_R_max = faceRegionSize.x * eyeBrowDist_max_ofFace;
    }
    /*
    private void CalculateAllTriggerDistances()
    {
        eyeBrowDist_L = Vector2.Distance(eyebrow_L_pos, eye_L_pos);
        eyeBrowDist_R = Vector2.Distance(eyebrow_R_pos, eye_R_pos);
        mouthDist = Vector2.Distance(mouth_top, mouth_bot);

        if (shouldSetBaseline)
        {
            faceRegionSize_lastRecalculateSize = faceRegionSize;

            eyeBrowDist_L_min = eyeBrowDist_L;
            eyeBrowDist_L_max = eyeBrowDist_L;

            eyeBrowDist_R_min = eyeBrowDist_R;
            eyeBrowDist_R_max = eyeBrowDist_R;

            mouthDist_min = mouthDist;
            mouthDist_max = mouthDist;
            shouldSetBaseline = false;
        }
    }
    */
    private void CalculateAllDistances() {
        eyeBrowDist_L = Vector2.Distance(eyebrow_L_pos, eye_L_pos);
        eyeBrowDist_R = Vector2.Distance(eyebrow_R_pos, eye_R_pos);
        mouthDist = Vector2.Distance(mouth_top, mouth_bot);

        mouthWidthDist = Vector2.Distance(mouth_left, mouth_right);

        headTurnAmount = faceRegionPos.x - nosePos.x;
    }

    private void CalculateAllNormalizedValues() { 
        eyeBrowL_normalizedDistValue = Mathf.InverseLerp(eyeBrowDist_L_min, eyeBrowDist_L_max, eyeBrowDist_L);
        //eyeBrowL_normalizedDistValue = Mathf.SmoothStep(eyeBrowDist_L_min, eyeBrowDist_L_max, eyeBrowDist_L);
        eyeBrowR_normalizedDistValue = Mathf.InverseLerp(eyeBrowDist_R_min, eyeBrowDist_R_max, eyeBrowDist_R);
        mouth_normalizedDistValue = Mathf.InverseLerp(mouthDist_min, mouthDist_max, mouthDist);
        mouthWidth_normalizedDistValue = Mathf.InverseLerp(mouthWidthDist_min, mouthWidthDist_max, mouthWidthDist);

        headTurnAmount_normalizedValue = Mathf.InverseLerp(-15, 15, headTurnAmount);


    }

    /* Get the important output values */

    public float GetNormalizedEyebrowLValue()
    {
        return eyeBrowL_normalizedDistValue;
    }

    public float GetNormalizedEyebrowRValue()
    {
        return eyeBrowR_normalizedDistValue;
    }

    public float GetNormalizedMouthValue() {
        return mouth_normalizedDistValue;
    }

    public float GetNormalizedMouthWidthValue()
    {
        return mouthWidth_normalizedDistValue;
    }

    public float GetNormalizedHEadTurnValue() {
        return headTurnAmount_normalizedValue;
    }
}
