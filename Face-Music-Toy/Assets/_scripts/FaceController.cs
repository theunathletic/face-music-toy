
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;

public class FaceController : MonoBehaviour
{
    public bool isTrackingFace = false;

    public Vector2 trackingAreaSize;

    //General mask and mask movement;
    public GameObject maskParent;
    public Vector3 maskParent_targetPos;
    public float scaleMaskMovement = 0.01f;
    public float maskMoveSpeed = 0.01f;

    public GameObject eyebrow_L;
    public GameObject eyebrow_R;

    //General face positioning and size
    public Vector2 faceRegionSize;
    public Vector2 faceRegionPos;

    public bool shouldSetBaseline = false;

    public Vector2 faceRegionSize_lastRecalculateSize;
    public float faceRegionSize_recalculateThreshold = 40f;

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

    public Vector2 mouth_top;
    public Vector2 mouth_bot;
    public float mouthDist;
    public float mouthDist_min;
    public float mouthDist_max;

    //Trigger trackers and sensitivities;
    public float mouthTrigger;
    public float mouthTriggerSensitivity = 0.5f;

    public float eyebrowTrigger_L;
    public float eyebrowTriggerSensitivity_L = 0.5f;

    public float eyebrowTrigger_R;
    public float eyebrowTriggerSensitivity_R = 0.5f;


    //Mask element and movement
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

    // Start is called before the first frame update
    void Start()
    {
        maskMouthBot_startingPos = maskMouthBot.transform.localPosition;
        maskMouthTop_startingPos = maskMouthTop.transform.localPosition;

        maskEyebrow_L_startingPos = maskEyebrow_L.transform.localPosition;
        maskEyebrow_R_startingPos = maskEyebrow_R.transform.localPosition;




    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTrackingAreaSize(float w, float h)
    {
        trackingAreaSize = new Vector2(w, h);

        Debug.Log("SetTrackingAreaSize: " + trackingAreaSize);
    }

    public void CheckTrackingState(bool isCurrentlyTracking)
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
    }

    private void TrackingLost()
    {
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

        //Moves the facePArent based on your faces lacation within the camera viewfinder. How much is controled by scaleFaceMovement.
        maskParent_targetPos = new Vector3((face.Region.Center.X - (trackingAreaSize.x / 2.0f)) * scaleMaskMovement,
            -(face.Region.Center.Y - (trackingAreaSize.y / 2.0f)) * scaleMaskMovement,
            maskParent.transform.position.z
        );
        maskParent.transform.position = Vector3.MoveTowards(maskParent.transform.position, maskParent_targetPos, maskMoveSpeed * Time.deltaTime);

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
                /*
                if (sub.Name == "Lip_Outer")
                {
                    mouth_top = new Vector2(sub.Marks[3].X, sub.Marks[3].Y);
                    mouth_bot = new Vector2(sub.Marks[9].X, sub.Marks[9].Y);
                }*/





                // Debug.Log(sub.Name);


            }
        }
        CalculateAllTriggerDistances();

        TriggerCheck_Eyebrow();
        TriggerCheck_Mouth();
    }

    //Check to see how much thie faceRegionSize has changed, if it's a lot we should reset the baselines for trigger.
    //The max and min values of face element distances change a lot depending on how close to the cmaera you are.
    private void CheckFaceRegionSizeChange() {
        if( Mathf.Abs(faceRegionSize_lastRecalculateSize.x - faceRegionSize.x) > faceRegionSize_recalculateThreshold) {
            shouldSetBaseline = true;
            Debug.Log("FaceRegionSize changed: UPDATE BASELINE");
        }
    }

    private void CalculateAllTriggerDistances() {
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

    private void TriggerCheck_Eyebrow()
    {
        if (eyeBrowDist_L < eyeBrowDist_L_min) { eyeBrowDist_L_min = eyeBrowDist_L; }
        if (eyeBrowDist_L > eyeBrowDist_L_max) { eyeBrowDist_L_max = eyeBrowDist_L; }

        if (eyeBrowDist_R < eyeBrowDist_R_min) { eyeBrowDist_R_min = eyeBrowDist_R; }
        if (eyeBrowDist_R > eyeBrowDist_R_max) { eyeBrowDist_R_max = eyeBrowDist_R; }

        eyebrowTrigger_L = Mathf.InverseLerp(eyeBrowDist_L_min, eyeBrowDist_L_max, eyeBrowDist_L);
        eyebrowTrigger_R = Mathf.InverseLerp(eyeBrowDist_R_min, eyeBrowDist_R_max, eyeBrowDist_R);

        if (eyebrowTrigger_L > eyebrowTriggerSensitivity_L)
        {
           // Debug.Log("Trigger: EYEBROW L");
        }

        if (eyebrowTrigger_R > eyebrowTriggerSensitivity_R)
        {
           // Debug.Log("Trigger: EYEBROW R");
        }

        SetMaskPosition_Eyebrow();

    }

    private void TriggerCheck_Mouth()
    {
        if (mouthDist < mouthDist_min) { mouthDist_min = mouthDist; }
        if (mouthDist > mouthDist_max) { mouthDist_max = mouthDist; }

        mouthTrigger = Mathf.InverseLerp(mouthDist_min, mouthDist_max, mouthDist);
        if(mouthTrigger > mouthTriggerSensitivity) {
           // Debug.Log("Trigger: MOUTH");
        }

        SetMaskPosition_Mouth();
    }

    private void SetMaskPosition_Eyebrow() {

        //Eyebrow L
        maskEyebrow_L_targetPos = new Vector3(maskEyebrow_L_startingPos.x,
               maskEyebrow_L_startingPos.y + (eyebrowTrigger_L * maskEyebrow_L_maxMovement),
               maskEyebrow_L_startingPos.z
           );
        maskEyebrow_L.transform.localPosition = Vector3.MoveTowards(maskEyebrow_L.transform.localPosition, maskEyebrow_L_targetPos, maskEyebrow_L_moveSpeed * Time.deltaTime);

        //Eyebrow R
        maskEyebrow_R_targetPos = new Vector3(maskEyebrow_R_startingPos.x,
               maskEyebrow_R_startingPos.y + (eyebrowTrigger_R * maskEyebrow_R_maxMovement),
               maskEyebrow_R_startingPos.z
           );
        maskEyebrow_R.transform.localPosition = Vector3.MoveTowards(maskEyebrow_R.transform.localPosition, maskEyebrow_R_targetPos, maskEyebrow_R_moveSpeed * Time.deltaTime);
    }

    private void SetMaskPosition_Mouth() {

        //Bot
        maskMouthBot_targetPos = new Vector3(maskMouthBot_startingPos.x,
               maskMouthBot_startingPos.y + (mouthTrigger * maskMouthBot_maxMovement),
               maskMouthBot_startingPos.z
           );
        maskMouthBot.transform.localPosition = Vector3.MoveTowards(maskMouthBot.transform.localPosition, maskMouthBot_targetPos, maskMouthBot_moveSpeed * Time.deltaTime);

        //Top
        maskMouthTop_targetPos = new Vector3(maskMouthTop_startingPos.x,
               maskMouthTop_startingPos.y + (mouthTrigger * maskMouthTop_maxMovement),
               maskMouthTop_startingPos.z
           );
        maskMouthTop.transform.localPosition = Vector3.MoveTowards(maskMouthTop.transform.localPosition, maskMouthTop_targetPos, maskMouthTop_moveSpeed * Time.deltaTime);
    }
}