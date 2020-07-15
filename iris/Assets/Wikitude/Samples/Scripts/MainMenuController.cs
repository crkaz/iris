using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Wikitude;


public class MainMenuController : MonoBehaviour
{
    protected GameObject _pointedObject;

    GestureRecognizer recognizer;

    public GameObject MainMenu;
    public GameObject InfoPanel;

    public Text VersionNumberText;
    public Text BuildDateText;
    public Text BuildNumberText;
    public Text BuildConfigurationText;
    public Text UnityVersionText;

    bool showInfo = false;

    void Awake() {
        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.Tapped += (args) => {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.Play();
            }
            SceneManager.LoadScene(_pointedObject.name);
        };
        recognizer.StartCapturingGestures();
    }

    // Use this for initialization
    void Start() {
        var buildInfo = WikitudeSDK.BuildInformation;
        VersionNumberText.text = buildInfo.SDKVersion;
        BuildDateText.text = buildInfo.BuildDate;
        BuildNumberText.text = buildInfo.BuildNumber;
        BuildConfigurationText.text = buildInfo.BuildConfiguration;
        UnityVersionText.text = Application.unityVersion;
    }

    public void OnInfoButtonPressed() {
        showInfo = !showInfo;
        InfoPanel.SetActive(showInfo);
    }

    public void OnInfoDoneButtonPressed() {
        MainMenu.SetActive(true);
        InfoPanel.SetActive(false);
    }

    public void OnSampleButtonClicked(Button sender) {
        /* Start the appropriate scene based on the button name that was pressed. */
        SceneManager.LoadScene(sender.name);
    }

    // Update is called once per frame
    void Update() {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            _pointedObject = hitInfo.collider.gameObject;
            Debug.Log("Pointing at " + _pointedObject.name);
        }
    }
}

