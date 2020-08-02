using System.IO;
using System.Linq;
using UnityEngine;

public class VisionService : MonoBehaviour
{
    #region Implements singleton pattern.
    public static VisionService Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    private UnityEngine.Windows.WebCam.PhotoCapture photoCaptureObject = null;
    private bool currentlyCapturing = false;
    private string imagePath;
    private string callbackAction;


    private void OnCapturedPhotoToDisk(UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        // Call  once the image has successfully captured
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }


    private void OnStoppedPhotoMode(UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        // Dispose of object and analyse image.
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
        switch (callbackAction.ToLower())
        {
            case "analyse":
                this.AnalyseImage();
                break;
            case "detectroom":
                this.DetectRoom();
                break;
        }
    }


    private async void AnalyseImage()
    {
        byte[] imageBytes = GetImageAsByteArray(imagePath);
        await IrisService.Instance.AnalyseImage(imageBytes);
    }


    private void DetectRoom()
    {
        byte[] imageBytes = GetImageAsByteArray(imagePath);
        IrisService.Instance.DetectRoom(imageBytes);
    }


    private static byte[] GetImageAsByteArray(string imageFilePath)
    {
        FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
    }


    // Capture an image.   
    public void CaptureImage(string callbackAction)
    {
        this.callbackAction = callbackAction;

        // Only allow capturing, if not currently processing a request.
        if (currentlyCapturing == false)
        {
            currentlyCapturing = true;

            // Set the camera resolution to be the highest possible    
            Resolution cameraResolution = UnityEngine.Windows.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

            // Begin capture process, set the image format    
            UnityEngine.Windows.WebCam.PhotoCapture.CreateAsync(false, delegate (UnityEngine.Windows.WebCam.PhotoCapture captureObject)
            {
                photoCaptureObject = captureObject;
                UnityEngine.Windows.WebCam.CameraParameters camParameters = new UnityEngine.Windows.WebCam.CameraParameters();
                camParameters.hologramOpacity = 0.0f;
                camParameters.cameraResolutionWidth = targetTexture.width;
                camParameters.cameraResolutionHeight = targetTexture.height;
                camParameters.pixelFormat = UnityEngine.Windows.WebCam.CapturePixelFormat.BGRA32;

                // Capture the image from the camera and save it in the App internal folder    
                captureObject.StartPhotoModeAsync(camParameters, delegate (UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result)
                    {
                        imagePath = Path.Combine(Application.persistentDataPath, "capture.jpg");
                        photoCaptureObject.TakePhotoAsync(imagePath, UnityEngine.Windows.WebCam.PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
                        currentlyCapturing = false;
                    });
            });
        }
    }

}