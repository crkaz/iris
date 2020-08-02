using UnityEngine;

public class Iris : MonoBehaviour
{
    [HideInInspector]
    public TextMesh debug_info; // Temp text label for displaying sensitivity value.

    /// <summary>
    /// Instance of the singleton class, Iris.
    /// </summary>
    public static Iris instance;

    /// <summary>
    /// Hololens max fps, used for calculating n samples.
    /// </summary>
    private const int MAX_FPS = 60;

    /// <summary>
    /// Number of SECONDS (max-fps(30) * value) until prompting user. Actual delay is an estimate.
    /// </summary>
    private const int ASSISTANCE_DELAY = 3;

    /// <summary>
    /// The aggregateDifference threshold - if it is less than sensitivity after the assistance delay, prompt the user.
    /// </summary>
    public float SENSITIVITY = 0.03f; // @TODO: THIS WILL NORMALLY BE PRIVATE CONST

    /// <summary>
    /// Maintains the current sample number.
    /// </summary>
    public int sampleCounter; // @TODO:
    //private int sampleCounter;

    /// <summary>
    /// Stores the main camera rotation from the last update.
    /// </summary>
    private Quaternion lastRotation;

    /// <summary>
    /// Current rotation of the main camera.
    /// </summary>
    private Quaternion currentRotation;

    /// <summary>
    /// Holds the difference between the current/last rotation values.
    /// </summary>
    private Quaternion rotationDelta;

    /// <summary>
    /// Stores the main camera transform from the last update.
    /// </summary>
    private Vector3 lastPosition;

    /// <summary>
    /// Current transform of the main camera.
    /// </summary>
    private Vector3 currentPosition;

    /// <summary>
    /// Holds the difference between the current/last transform values.
    /// </summary>
    private Vector3 positionDelta;

    /// <summary>
    /// Maintains the moving average of the change in rotation accross samples rather than storing a collection of samples.
    /// </summary>
    public float aggregateDifference;
    //private float aggregateDifference;

    /// <summary>
    /// The number of samples to get the average rotation accross before checking if the user needs assistance.
    /// </summary>
    private readonly int sampleRate = MAX_FPS * ASSISTANCE_DELAY;

    // @TODO:
    public bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        sampleCounter = 1;
        this.SENSITIVITY = 0.03f;
        lastRotation = transform.rotation; // Get current camera rotation in quaternions.
        lastPosition = transform.position;
        // Create sext.
    }

    // Update is called once per frame
    void Update()
    {
        currentRotation = transform.rotation;
        currentPosition = transform.position;
        CheckDiff();
    }


    // Treat class as a singleton.
    private void Awake()
    {
        instance = this;
    }

    #region TEMPORARY VOICE COMMANDS
    void IncreaseSensitivity()
    {
        SENSITIVITY *= 2.0f; // Increase threshold to make it more sensitve to rotation.
    }
    void DecreaseSensitivity()
    {
        SENSITIVITY *= 0.5f; // Reduce threshold to make it less sensitve to rotation.
    }
    #endregion

    //@TODO: TEMPORARY FOR DEBUGGING
    public float rotDiff;
    public float posDiff;
    public bool capturing = false;

    /// <summary>
    /// 
    /// Detect lack of head movement over n samples, suggesting the 
    /// user is stuck-on (or observing) something.
    /// 
    /// </summary>
    void CheckDiff()
    {
        void reset()
        {
            // Reset checks if user moved significantly.
            Debug.Log("Unstable");

            moving = true;
            sampleCounter = 0;
            aggregateDifference = 0;
        }

        // https://forum.unity.com/threads/get-the-difference-between-two-quaternions-and-add-it-to-another-quaternion.513187/
        rotationDelta = lastRotation * Quaternion.Inverse(currentRotation); // Get change in rotation since last sample.
        rotDiff = Mathf.Abs(rotationDelta.x + rotationDelta.y);
        posDiff = Mathf.Abs(lastPosition.magnitude - currentPosition.magnitude);

        lastRotation = currentRotation;
        lastPosition = currentPosition;

        // Reset if user moved significantly.
        if (rotDiff > 0.003 || posDiff > 0.003) // 0.01 still allows significant amount of steady movement
        {
            reset();
        }
        else
        {
            moving = false;
            // Running avg. Values squared to make absolute and maintain precision. 
            aggregateDifference += rotDiff * 0.5f;

            // Determine if the gaze was relatively stable for the period of time defined as ASSISTANCE_DELAY.
            if (++sampleCounter == sampleRate)
            {
                //Debug.Log("summedDiff = " + aggregateDifference.ToString());

                if (aggregateDifference < SENSITIVITY)
                {
                    Debug.Log("DETECTED STARE");

                    // DISPLAY BUTTONS TO CONFIRM
                    // Run azure CV analysis.
                    BroadcastMessage("ExecuteImageCaptureAndAnalysis");
                    capturing = true;
                }
                
                reset();
            }
        }

    }
}
