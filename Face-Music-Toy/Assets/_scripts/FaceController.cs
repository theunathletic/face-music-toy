
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;

public class FaceController : MonoBehaviour
{
    public bool isTrackingFace = false;

    private Vector2 trackingAreaSize;

    public GameObject faceParent;
    public Vector3 faceParent_targetPos;
    public float scaleFaceMovement = 0.01f;
    public float faceMoveSpeed = 0.01f;

    public GameObject nose;
    public GameObject eyebrow_L;
    public GameObject eyebrow_R;
    public GameObject eye_L;
    public GameObject eye_R;

    public float eyebrowMoveSpeed = 1f;

    public Vector2 eyebrow_L_pos;
    public Vector2 eye_L_pos;

    public Vector2 eyebrow_R_pos;
    public Vector2 eye_R_pos;

    public float eyeBrowDist_L;

    public Vector2 faceRegionSize;
    public Vector2 faceRegionPos;

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
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTrackingAreaSize(float w, float h) {
        trackingAreaSize = new Vector2(w, h);

        Debug.Log("SetTrackingAreaSize: " + trackingAreaSize);
    }

    public void CheckTrackingState(bool isCurrentlyTracking) {
        if(isCurrentlyTracking != isTrackingFace) {
            isTrackingFace = isCurrentlyTracking;
            if (isTrackingFace) {
                TrackingFound();
            }
            else {
                TrackingLost();
            }
        }
    }

    private void TrackingFound() {
        Debug.Log("FaceController: TrackingFound.");
    }

    private void TrackingLost()
    {
        Debug.Log("FaceController: TrackingLost.");
    }

    public void UpdateFaceElementPositions(DetectedFace face) {

        // Debug.Log("Region size: " + face.Region.Size);
        //Debug.Log("face.Region.Location:" + face.Region.Center);
        faceParent_targetPos = new Vector3((face.Region.Center.X - (trackingAreaSize.x / 2.0f)) * scaleFaceMovement,
            -(face.Region.Center.Y - (trackingAreaSize.y / 2.0f)) * scaleFaceMovement,
            faceParent.transform.position.z
        );
        /*
        faceParent.transform.position = new Vector3((face.Region.Center.X - (trackingAreaSize.x/2.0f))* scaleFaceMovement,
            -(face.Region.Center.Y - (trackingAreaSize.y/2.0f))* scaleFaceMovement,
            faceParent.transform.position.z
        );*/
       // Debug.Log("face.Region.Size:" + face.Region.Size);
        faceRegionSize =  new Vector2(face.Region.Size.Width, face.Region.Size.Height);
        faceRegionPos = new Vector2(face.Region.Center.X, face.Region.Center.Y);

        faceParent.transform.position = Vector3.MoveTowards(faceParent.transform.position, faceParent_targetPos, faceMoveSpeed * Time.deltaTime);

        foreach (DetectedObject sub in face.Elements)
            {
                if (sub.Marks != null)
                {

                if(sub.Name == "Eyebrow_L") {
                    /*
                    eyebrow_L_pos = new Vector2(sub.Marks[0].X, sub.Marks[0].Y);
                    //Debug.Log("eyebrow_L_pos" + eyebrow_L_pos);
                    Debug.Log("faceRegionPos Y:" + faceRegionPos.y +
                                "  faceRegionSize/2:" + (faceRegionSize.y / 2) +
                                "  eyebrow_L_pos.y:" + eyebrow_L_pos.y);

                    //faceRegionPos.y - eyebrow_L_pos.y
                    float relativePos = Mathf.InverseLerp((faceRegionPos.y - (faceRegionSize.y / 2)), (faceRegionPos.y + (faceRegionSize.y / 2)), eyebrow_L_pos.y);
                    Debug.Log("relativePos:" + relativePos);

                    eyebrow_L.transform.localPosition = Vector3.MoveTowards(eyebrow_L.transform.localPosition,
                    new Vector3(eyebrow_L.transform.localPosition.x,
                    Mathf.Lerp(2f, -2f, relativePos),
                    eyebrow_L.transform.localPosition.z),
                    eyebrowMoveSpeed * Time.deltaTime
                    );
                    */

                    UpdateFaceElement(new Vector2(sub.Marks[0].X, sub.Marks[0].Y),
                           eyebrow_L,
                           false,
                           true,
                           eyebrowMoveSpeed);

                }

                if (sub.Name == "Eye_L")
                {
               /*
                    float relativePosX = Mathf.InverseLerp((faceRegionPos.x - (faceRegionSize.x / 2)), (faceRegionPos.x + (faceRegionSize.x / 2)), sub.Marks[0].X);
                    float relativePosY = Mathf.InverseLerp((faceRegionPos.y - (faceRegionSize.y / 2)), (faceRegionPos.y + (faceRegionSize.y / 2)), sub.Marks[0].Y);


                    eye_L.transform.localPosition = Vector3.MoveTowards(eye_L.transform.localPosition,
                    new Vector3(eye_L.transform.localPosition.x,
                    Mathf.Lerp(2f, -2f, relativePosY),
                    eye_L.transform.localPosition.z),
                    eyebrowMoveSpeed * Time.deltaTime
                    );
                    */

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
                    /*
                    eyebrow_R_pos = new Vector2(sub.Marks[0].X, sub.Marks[0].Y);

                    float relativePos = Mathf.InverseLerp((faceRegionPos.y - (faceRegionSize.y / 2)), (faceRegionPos.y + (faceRegionSize.y / 2)), eyebrow_R_pos.y);

                    eyebrow_R.transform.localPosition = Vector3.MoveTowards(eyebrow_R.transform.localPosition,
                    new Vector3(eyebrow_R.transform.localPosition.x,
                    Mathf.Lerp(2f, -2f, relativePos),
                    eyebrow_R.transform.localPosition.z),
                    eyebrowMoveSpeed * Time.deltaTime
                    );
                    */
                    UpdateFaceElement(new Vector2(sub.Marks[0].X, sub.Marks[0].Y),
                          eyebrow_R,
                          false,
                          true,
                          eyebrowMoveSpeed);
                }

                // Debug.Log(sub.Name);


            }
        }

        //EyebrowTriggerCheck();
    }


    private void EyebrowTriggerCheck() {
        eyeBrowDist_L = eyebrow_L_pos.y - eye_L_pos.y;
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
