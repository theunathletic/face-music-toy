using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

public class TUA_FaceDetector : MonoBehaviour
{
    /// <summary>
    /// Target surface to render WebCam stream
    /// </summary>
    public GameObject Surface;

    private Nullable<WebCamDevice> webCamDevice = null;
    private WebCamTexture webCamTexture = null;
    private Texture2D renderedTexture = null;

    /// <summary>
    /// A kind of workaround for macOS issue: MacBook doesn't state it's webcam as frontal
    /// </summary>
    protected bool forceFrontalCamera = false;

    protected OpenCvSharp.Unity.TextureConversionParams TextureParameters { get; private set; }

    /// <summary>
    /// Camera device name, full list can be taken from WebCamTextures.devices enumerator
    /// </summary>
    public string DeviceName
    {
        get
        {
            return (webCamDevice != null) ? webCamDevice.Value.name : null;
        }
        set
        {
            // quick test
            if (value == DeviceName)
                return;

            if (null != webCamTexture && webCamTexture.isPlaying)
                webCamTexture.Stop();

            // get device index
            int cameraIndex = -1;
            for (int i = 0; i < WebCamTexture.devices.Length && -1 == cameraIndex; i++)
            {
                if (WebCamTexture.devices[i].name == value)
                    cameraIndex = i;
            }

            // set device up
            if (-1 != cameraIndex)
            {
                webCamDevice = WebCamTexture.devices[cameraIndex];
                webCamTexture = new WebCamTexture(webCamDevice.Value.name);

                // read device params and make conversion map
                ReadTextureConversionParameters();

                webCamTexture.Play();
            }
            else
            {
                throw new ArgumentException(String.Format("{0}: provided DeviceName is not correct device identifier", this.GetType().Name));
            }
        }
    }

    public TextAsset detector_faces;
    public TextAsset detector_eyes;
    public TextAsset detector_shapes;

    private OpenCvSharp.Demo.FaceProcessorLive<WebCamTexture> processor;


    public bool shouldProcessTexure = true;
    public bool showRenderTexture = true; //Maybe remove this;
    public bool showCameraTexture = true;
    public bool showLines = true;

    /// <summary>
    /// This method scans source device params (flip, rotation, front-camera status etc.) and
    /// prepares TextureConversionParameters that will compensate all that stuff for OpenCV
    /// </summary>
    private void ReadTextureConversionParameters()
    {
        OpenCvSharp.Unity.TextureConversionParams parameters = new OpenCvSharp.Unity.TextureConversionParams();

        // frontal camera - we must flip around Y axis to make it mirror-like
        parameters.FlipHorizontally = forceFrontalCamera || webCamDevice.Value.isFrontFacing;

        // TODO:
        // actually, code below should work, however, on our devices tests every device except iPad
        // returned "false", iPad said "true" but the texture wasn't actually flipped

        // compensate vertical flip
        //parameters.FlipVertically = webCamTexture.videoVerticallyMirrored;

        // deal with rotation
        if (0 != webCamTexture.videoRotationAngle)
            parameters.RotationAngle = webCamTexture.videoRotationAngle; // cw -> ccw

        // apply
        TextureParameters = parameters;

        //UnityEngine.Debug.Log (string.Format("front = {0}, vertMirrored = {1}, angle = {2}", webCamDevice.isFrontFacing, webCamTexture.videoVerticallyMirrored, webCamTexture.videoRotationAngle));
    }

    private void Awake()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            DeviceName = WebCamTexture.devices[WebCamTexture.devices.Length - 1].name;
        }
        forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

        byte[] shapeDat = detector_shapes.bytes;
        if (shapeDat.Length == 0)
        {
            string errorMessage =
                "In order to have Face Landmarks working you must download special pre-trained shape predictor " +
                "available for free via DLib library website and replace a placeholder file located at " +
                "\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
                "Without shape predictor demo will only detect face rects.";

            #if UNITY_EDITOR
                        // query user to download the proper shape predictor
                        if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
                            Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
            #else
                         UnityEngine.Debug.Log(errorMessage);
            #endif
        }

        processor = new OpenCvSharp.Demo.FaceProcessorLive<WebCamTexture>();
        processor.Initialize(detector_faces.text, detector_eyes.text, detector_shapes.bytes);

        // data stabilizer - affects face rects, face landmarks etc.
        processor.DataStabilizer.Enabled = true;        // enable stabilizer
        processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
        processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

        // performance data - some tricks to make it work faster
        processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
        processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)

    }


    // Update is called once per frame
    void Update()
    {
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            // this must be called continuously
            ReadTextureConversionParameters();


            // process texture with whatever method sub-class might have in mind
            if (ProcessTexture(webCamTexture, ref renderedTexture))
            {
                if (showRenderTexture)
                {
                    RenderFrame();
                }
            }
        }
    }

    /// <summary>
    /// Per-frame video capture processor
    /// </summary>
    private bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        if (!shouldProcessTexure) {
            return false;
        }

        // detect everything we're interested in
        processor.ProcessTexture(input, TextureParameters);

        // mark detected objects
        if (showLines)
        {
            processor.MarkDetected();
        }
        /*
        foreach (OpenCvSharp.Demo.DetectedFace face in processor.Faces)
        {
            foreach (OpenCvSharp.Demo.DetectedObject sub in face.Elements)
            {
                if (sub.Marks != null)
                {
                    UnityEngine.Debug.Log(sub.Name);
                }
            }
        }
        */
        // processor.Image now holds data we'd like to visualize
        output = OpenCvSharp.Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

        return true;
    }

    /// <summary>
    /// Renders frame onto the surface
    /// </summary>
    private void RenderFrame()
    {
        if (renderedTexture != null)
        {
            // apply
            Surface.GetComponent<RawImage>().texture = renderedTexture;

            // Adjust image ration according to the texture sizes 
            Surface.GetComponent<RectTransform>().sizeDelta = new Vector2(renderedTexture.width, renderedTexture.height);
        }
    }

    public void ToggleShouldProcessTexure(bool toggle) {
        shouldProcessTexure = toggle;
    }

    public void ToggleShowRenderTexture(bool toggle)
    {
        showRenderTexture = toggle;
    }

    public void ToggleShowCameraTexture(bool toggle)
    {
        showCameraTexture = toggle;
        Surface.SetActive(showCameraTexture);
    }

    public void ToggleShowLines(bool toggle)
    {
        showLines = toggle;
    }


    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            if (webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
            }
            webCamTexture = null;
        }

        if (webCamDevice != null)
        {
            webCamDevice = null;
        }
    }
}
