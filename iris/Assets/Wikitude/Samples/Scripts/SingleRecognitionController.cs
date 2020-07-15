using UnityEngine;
using UnityEngine.UI;
using Wikitude;

public class SingleRecognitionController : SampleController
{
    public ImageTracker Tracker;

    public Text InfoText;

    private float _timer;
    private bool _waitForRecognize = false;

    protected override void Awake() {
        base.Awake();
        keywords.Add("scan", () => {
            OnRecognizeClicked();
        });
    }

    public void OnRecognizeClicked() {
        _timer = 3.1f;
        _waitForRecognize = true;
    }

    protected override void OnAirTap()
    {
        OnRecognizeClicked();
    }

    void Update() {
        if(_waitForRecognize) {
            _timer -= Time.deltaTime;
            InfoText.text = "Recognizing in: " + Mathf.Round(_timer);
            if (_timer < 0) {
                Tracker.CloudRecognitionService.Recognize();
                _waitForRecognize = false;
            }
        }
    }

    /* Called when the tracker finished initializing and is ready to receive recognition requests. */
    public void OnConnectionInitialized() {
        InfoText.text = "Waiting to scan";
    }

    public void OnConnectionInitializationError(Error error) {
        InfoText.text = "Error initializing cloud connection!\n" + error.Message;
        PrintError("Error initializing cloud connection!", error);
    }

    public void OnRecognitionResponse(CloudRecognitionServiceResponse response) {
        if (response.Recognized) {
            InfoText.text = "Target found: " + response.Info["name"];
        } else {
            InfoText.text = "No target recognized";
        }
    }

    public void OnRecognitionError(Error error) {
        InfoText.text = "Recognition failed!";
        PrintError("Recognition error!", error);
    }
}
