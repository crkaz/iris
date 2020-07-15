using UnityEngine.UI;
using Wikitude;

public class ContinuousRecognitionController : SampleController
{
    public ImageTracker Tracker;
    public Text ButtonText;
    public Text InfoText;
    private bool _trackerRunning = false;
    private bool _connectionInitialized = false;

    private double _recognitionInterval = 1.5;

    #region UI Events
    public void OnToggleClicked() {
        /* Toggle continuous cloud recognition on or off. */
        _trackerRunning = !_trackerRunning;
        ToggleContinuousCloudRecognition(_trackerRunning);
    }
    #endregion

    #region Tracker Events
    public void OnInitialized() {
        base.OnTargetsLoaded();
        _connectionInitialized = true;
        InfoText.text = "";
    }

    protected override void Awake() {
        base.Awake();
        keywords.Add("start scanning", () => {
            if (!_trackerRunning) {
                OnToggleClicked();
            }
        });
        keywords.Add("stop scanning", () => {
            if (_trackerRunning) {
                OnToggleClicked();
            }
        });
    }

    public override void OnInitializationError(Error error) {
        InfoText.text = "Error initializing cloud connection! Please restart the sample!\n" + error.Message;
        PrintError("Error initializing cloud connection!", error);
        ButtonText.gameObject.GetComponentInParent<Button>().gameObject.SetActive(false);
    }

    public void OnRecognitionResponse(CloudRecognitionServiceResponse response) {
        if (response.Recognized) {
            /* If the cloud recognized a target, we stop continuous recognition and track that target locally. */
            ToggleContinuousCloudRecognition(false);
            InfoText.text = response.Info["name"];
        } else {
            InfoText.text = "No target recognized";
        }
    }

    public void OnInterruption(double suggestedInterval) {
        /* If recognition was interrupted, try to start it again using the suggested interval. */
        _recognitionInterval = suggestedInterval;
        Tracker.CloudRecognitionService.StartContinuousRecognition(_recognitionInterval);
    }
    #endregion

    protected override void OnAirTap()
    {
        OnToggleClicked();
    }

    private void ToggleContinuousCloudRecognition(bool enabled) {
        if (Tracker != null && _connectionInitialized) {
            if (enabled) {
                ButtonText.text = "Air tap\n to stop";
                Tracker.CloudRecognitionService.StartContinuousRecognition(_recognitionInterval);
            } else {
                ButtonText.text = "Air tap\n to start";
                Tracker.CloudRecognitionService.StopContinuousRecognition();
            }
            _trackerRunning = enabled;
        }
    }

    public void OnRecognitionError(Error error) {
        InfoText.text = "Recognition failed!";
        PrintError("Recognition error!", error);
    }
}
