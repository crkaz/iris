using UnityEngine;
using UnityEngine.SceneManagement;
using Wikitude;
using System.Text;
using UnityEngine.Windows.Speech;
using UnityEngine.XR.WSA.Input;
using System.Collections.Generic;
using System.Linq;


/* Base class for all sample controllers. Defines functionality that all samples needs. */
public class SampleController : MonoBehaviour
{
    KeywordRecognizer _keywordRecognizer;
    protected Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    private GestureRecognizer _recognizer;

    protected virtual void Awake() {
        keywords.Add("back", () => {
            OnBackButtonClicked();
        });
    }

    protected virtual void Start() {
        InitVoiceCommand();
        _recognizer = new GestureRecognizer();
        _recognizer.Tapped += (args) => {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.Play();
            }
            OnAirTap();
        };
        _recognizer.StartCapturingGestures();
    }

    protected void InitVoiceCommand() {
        _keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
        System.Action keywordAction;
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(args.text, out keywordAction)) {
            keywordAction.Invoke();
        }
    }

    protected virtual void OnAirTap()
    {

    }

    public virtual void OnBackButtonClicked() {
        /* Return to the Main Menu when the Back button is pressed, either from the UI, or when pressing the Android back button. */
        SceneManager.LoadScene("Main Menu");
    }

    public virtual void OnSDKInitialized() {
        Debug.Log("SDK Initialized");
    }

    /* Defines default implementation for some of the most common Wikitude SDK events. */
    /* URLResource events */
    public virtual void OnFinishLoading() {
        Debug.Log("URL Resource loaded successfully.");
    }

    public virtual void OnErrorLoading(Error error) {
        PrintError("Error loading URL Resource!", error);
    }

    public virtual void OnInitializationError(Error error) {
        PrintError("Error initializing tracker!", error);
    }

    /* Tracker events */
    public virtual void OnTargetsLoaded() {
        Debug.Log("Targets loaded successfully.");
    }

    public virtual void OnErrorLoadingTargets(Error error) {
        PrintError("Error loading targets!", error);
    }

    public virtual void OnCameraError(Error error) {
        PrintError("Camera Error!", error);
    }

    /* Prints a Wikitude SDK error to the Unity console, as well as the onscreen console defined below. */
    protected void PrintError(string message, Error error) {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(message);
        AppendError(stringBuilder, error);

        Debug.LogError(stringBuilder.ToString());
    }

    /* Handles underlying errors by recursively calling itself */
    private void AppendError(StringBuilder stringBuilder, Error error) {
        stringBuilder.AppendLine("        Error Code: " + error.Code);
        stringBuilder.AppendLine("        Error Domain: " + error.Domain);
        stringBuilder.AppendLine("        Error Message: " + error.Message);

        if (error.UnderlyingError != null) {
            stringBuilder.AppendLine("    Underlying Error: ");
            AppendError(stringBuilder, error.UnderlyingError);
        }
    }
}
