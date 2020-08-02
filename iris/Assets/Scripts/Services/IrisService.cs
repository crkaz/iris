using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

public class IrisService : MonoBehaviour
{
    // Consts.
    public const string HOST = "http://localhost:54268/api/";
    public const string API_KEY = "testpatient";
    public const string ID_PARAM = "/?id=testpatient";
    private readonly Dictionary<string, string> HEADERS = new Dictionary<string, string>() { { "ApiKey", API_KEY } };
    private const float POLL_RATE = 3.0f;




    // Collections/members.
    public IList<PatientMessage> Messages { get; private set; }
    public IList<CalendarEntry> CalendarEntries { get; private set; }
    private bool firstDetect = true;
    public string Location = "unknown";


    #region Implements singleton pattern.
    public static IrisService Instance;
    private void Awake()
    {
        Instance = this;
        Init();
        StartCoroutine(ObserveServer());
    }
    #endregion


    private void OnDestroy()
    {
        Destroy();
    }


    private async void Init()
    {
        var response = await GetServerStatus();
        if (response.ResponseCode == 200)
        {
            IrisNotificationsService.Instance.Notify("Connected to IRIS server successfully.");
            UpdateOnlineStatus("online");
            PostActivity("Connected");
            VisionService.Instance.CaptureImage("detectroom");

            // Initialise collections.
            this.Messages = new List<PatientMessage>();
            this.CalendarEntries = new List<CalendarEntry>();

            // TODO: Get config. Pass to LoadFeatures().
            //await LoadFeatures();

        }
        else
        {
            IrisNotificationsService.Instance.Notify("Failed to connect to IRIS server.");
        }
    }


    private void Destroy()
    {
        PostActivity("Disconnected");
        UpdateOnlineStatus("offline");
    }


    #region Private

    private async void UpdateCollections()
    {
        try
        {

            int nMessagesBeforeUpdate = this.Messages.Count;
            this.Messages = await this.GetMessages();

            // Notify user of new messages.
            if (this.Messages.Count > nMessagesBeforeUpdate)
            {
                Debug.Log("New messages");
                IrisNotificationsService.Instance.Notify("You've received a new message. Say, 'Messages', when you want to view it.");
            }
            // await LoadMessages();
            this.CalendarEntries = await this.GetCalendarEntries();
        }
        catch (Exception e)
        {
            Debug.Log("Unxepected error whilst updating collections:\n" + e.Message);
        }
    }


    // private async Task LoadMessages()
    // {
    //     int currntNumMessages = this.Messages.Count;
    //     this.Messages = await this.GetMessages();

    //     // Notify user of new messages.
    //     if (currntNumMessages != this.Messages.Count)
    //     {
    //         IrisNotificationsService.Instance.Notify("You've received a new message. Say, 'Messages', when you want to view it.");
    //     }
    // }



    // Coroutine for fetching server changes. Not truly "observing" : server would need to implement SSE or websockets.
    private IEnumerator ObserveServer()
    {
        for (; ; )
        {
            this.UpdateCollections();
            yield return new WaitForSeconds(POLL_RATE); // Wait a defined time before the next call to the server.
        }
    }


    private async Task<Response> GetConfig()
    {
        // TODO
        const string ENDPOINT = HOST + "config/get";
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
        return response;
    }


    private async Task<Response> GetServerStatus()
    {
        const string ENDPOINT = HOST + "test/status";
        var response = await Rest.GetAsync(ENDPOINT, readResponseData: true);
        return response;
    }


    private async Task<IList<PatientMessage>> GetMessages()
    {
        const string ENDPOINT = HOST + "message/get";
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
        IList<PatientMessage> messages = JsonConvert.DeserializeObject<IList<PatientMessage>>(response.ResponseBody);
        return messages;
    }


    private async Task<IList<CalendarEntry>> GetCalendarEntries()
    {
        const string ENDPOINT = HOST + "calendar/patientget";
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
        IList<CalendarEntry> calendarEntries = JsonConvert.DeserializeObject<IList<CalendarEntry>>(response.ResponseBody);
        return calendarEntries;
    }

    #endregion


    #region  Public

    public async void UpdateOnlineStatus(string status)
    {
        string ENDPOINT = HOST + "patient/updatestatus/?status=" + status;
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
    }


    public async Task<Response> AnalyseImage(byte[] imageBytes)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "ApiKey", API_KEY },
            { "Content-Type", "application/octet-stream" }
             };
        const string ENDPOINT = HOST + "compute/analyseimage";
        var response = await Rest.PostAsync(ENDPOINT, imageBytes, headers, readResponseData: true);
        Debug.Log(response.ResponseBody);
        return response;
    }


    public async void DetectRoom(byte[] imageBytes)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "ApiKey", API_KEY },
            { "Content-Type", "application/octet-stream" }
             };
        const string ENDPOINT = HOST + "compute/detectroom";
        var response = await Rest.PostAsync(ENDPOINT, imageBytes, headers, readResponseData: true);

        // Update room.
        Debug.Log("Detected room: " + response.ResponseBody);
        if (this.Location != response.ResponseBody && !this.firstDetect)
        {
            this.firstDetect = false;
            PostActivity("Entered " + response.ResponseBody);
        }
        this.Location = response.ResponseBody;
    }


    public async void PostActivity(string caption, string jsonDescription = "")
    {
        ActivityLog log = new ActivityLog() { Caption = caption, Location = this.Location, JsonDescription = jsonDescription };
        string bodyJson = JsonConvert.SerializeObject(log);
        const string ENDPOINT = HOST + "patient/logs";
        var response = await Rest.PostAsync(ENDPOINT, bodyJson, HEADERS);
    }


    public async Task<ICollection<Sticky>> GetStickyNotes()
    {
        const string ENDPOINT = HOST + "stickies/get";
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
        ICollection<Sticky> stickies = JsonConvert.DeserializeObject<ICollection<Sticky>>(response.ResponseBody);
        return stickies;
    }


    public async Task<bool> DetectFall(string yTransformsJson)
    {
        const string ENDPOINT = HOST + "compute/detectfall";
        var response = await Rest.PostAsync(ENDPOINT, yTransformsJson, HEADERS, readResponseData: true);
        Dictionary<string, object> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.ResponseBody);
        bool fallDetected = (bool)jsonDict["falldetection"];
        Debug.Log(fallDetected);
        return fallDetected;
    }


    public async Task<Response> AnalyseMovement(string transformsJson)
    {
        const string ENDPOINT = HOST + "compute/analysemovement";
        var response = await Rest.PostAsync(ENDPOINT, transformsJson, HEADERS, readResponseData: true);
        Dictionary<string, object> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.ResponseBody);
        bool fallDetected = (bool)jsonDict["falldetection"];
        Debug.Log(fallDetected);
        return response;
    }


    public async void MarkMessageAsRead(PatientMessage patientMessage)
    {
        this.Messages.Remove(patientMessage);
        Debug.Log("Remobing patientmessage w id " + patientMessage.Id);
        string ENDPOINT = HOST + "message/read/?id=" + patientMessage.Id;
        await Rest.GetAsync(ENDPOINT, HEADERS);
    }

    #endregion
}