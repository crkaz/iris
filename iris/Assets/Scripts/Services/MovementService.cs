using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementService : MonoBehaviour
{
    private const float ANALYSIS_FREQUENCY = 5.0f; // How many seconds until the transforms should be processed.
    private const float UPDATE_FREQUENCY = 0.25f; // How frequently the current y-pos should be recorded.


    void Start()
    {
        StartCoroutine(AddTransform());
        StartCoroutine(AnalyseMovement());
    }


    private IEnumerator AddTransform()
    {
        for (; ; )
        {
            MovementLog.Instance.Update();
            yield return new WaitForSeconds(UPDATE_FREQUENCY);
        }
    }


    private IEnumerator AnalyseMovement()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(ANALYSIS_FREQUENCY);
            Debug.Log("Analysing patient movement.");
            this.Analyse();
        }
    }


    private async void Analyse()
    {
        // If the specified amount of time HAS passed: analyse transforms.
        string transformsJson = JsonConvert.SerializeObject(MovementLog.Instance.Get()); // convert to json
        var result = await IrisService.Instance.AnalyseMovement(transformsJson); // send to server for analysis
        IDictionary<string, object> response = JsonConvert.DeserializeObject<IDictionary<string, object>>(result.ResponseBody); // get analyses as dict

        // Allow individual features to manage respective responses.
        HandleResponse(response);

        // Clear the stored transforms and repeat the process.
        MovementLog.Instance.Clear();
    }


    private void HandleResponse(IDictionary<string, object> responseDict)
    {
        bool hasFallen = (bool)responseDict["falldetection"];
        Debug.Log(hasFallen);
        if (hasFallen){
            IrisService.Instance.PostActivity("Patient has fallen!");
            IrisService.Instance.UpdateOnlineStatus("alert");
        }
        
        bool shouldDetectRoom = (bool)responseDict["movementdetection"];
        Debug.Log(shouldDetectRoom);
        if (shouldDetectRoom){
            VisionService.Instance.CaptureImage("detectroom");
        }
        
        // bool shouldDetectObjects = (bool)responseDict["confusiondetection"];
        // if (shouldDetectObjects){
        //     VisionService.Instance.CaptureImage("analyse");
        // }
    }


    private class MovementLog
    {
        #region Singleton pattern.
        private static MovementLog instance;
        public static MovementLog Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MovementLog();
                }
                return instance;
            }
        }
        private MovementLog() { }
        #endregion


        public List<float> xPos = new List<float>();
        public List<float> yPos = new List<float>();
        public List<float> zPos = new List<float>();
        public List<float> xRot = new List<float>();
        public List<float> yRot = new List<float>();
        public List<float> zRot = new List<float>();


        public void Update()
        {
            xPos.Add(Camera.main.transform.position.x);
            yPos.Add(Camera.main.transform.position.y);
            zPos.Add(Camera.main.transform.position.z);
            xRot.Add(Camera.main.transform.rotation.x);
            yRot.Add(Camera.main.transform.rotation.y);
            zRot.Add(Camera.main.transform.rotation.z);
        }


        public IDictionary<string, List<float>> Get()
        {
            return new Dictionary<string, List<float>>() {
            {"xPos", xPos },
            {"yPos", yPos },
            {"zPos", zPos },
            {"xRot", xRot },
            {"yRot", yRot },
            {"zRot", zRot },
        };
        }


        public void Clear()
        {
            xPos.Clear();
            yPos.Clear();
            zPos.Clear();
            xRot.Clear();
            yRot.Clear();
            zRot.Clear();
        }
    }
}
