using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

public class IrisService : MonoBehaviour
{
    public const string HOST = "http://localhost:54268/api/";
    public const string API_KEY = "testpatient";
    public const string ID_PARAM = "/?id=testpatient";
    private readonly Dictionary<string, string> HEADERS = new Dictionary<string, string>() { { "ApiKey", API_KEY } };
    private const float POLL_RATE = 10.0f;

    public static IrisService Instance;


    public IList<PatientMessage> Messages { get; private set; }
    public IList<CalendarEntry> CalendarEntries { get; private set; }


    void Awake()
    {
        Instance = this;
        Init();
        StartCoroutine(ObserveServer());
    }


    void OnDestroy()
    {
        Destroy();
    }


    private async void Init()
    {
        var response = await GetServerStatus();
        if (response.ResponseCode == 200)
        {
            Debug.Log("Connected to IRIS server sucessfully.");
            await UpdateOnlineStatus("online");
            // TODO: Get config. Pass to LoadFeatures().
            //await LoadFeatures();

        }
        else
        {
            Debug.Log("Failed to connect to IRIS server.");
        }
    }


    private async void Destroy()
    {
        await UpdateOnlineStatus("offline");
    }


    private async void UpdateCollections()
    {
        // Get patient messages ordered by date.
        this.Messages = await this.GetMessages();
        this.Messages = this.Messages.OrderBy(msg => msg.Sent).ToList();
        //this.CalendarEntries = await this.GetCalendarEntries();
        //Debug.Log("Got calendars:");
        //Debug.Log(CalendarEntries);
    }


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
        Debug.Log(response.ResponseCode);
        return response;
    }


    //private async Task LoadFeatures()
    //{
    // TODO: If stickies enabled, add manager.
    // var stickies = await GetStickyNotes();
    // Debug.Log(stickies.ResponseBody);
    // ISticky[] stickies = JsonConvert.DeserializeObject<ISticky[]>(response.ResponseBody);
    // Debug.Log(stickies);
    // // Dese<ICollection<ISticky>>(stickies.ResponseBody);
    // foreach (ISticky s in stickies) {
    //     Debug.Log(s);
    //  }

    // var messages = await GetMessages();
    //}

    #region  Endpoints


    private async Task<Response> GetServerStatus()
    {
        const string ENDPOINT = HOST + "test/status";
        var response = await Rest.GetAsync(ENDPOINT, readResponseData: true);
        Debug.Log(response.ResponseCode);
        Debug.Log(response.ResponseBody);
        return response;
    }


    private async Task UpdateOnlineStatus(string status)
    {
        string ENDPOINT = HOST + "patient/updatestatus/?status=" + status;
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
        Debug.Log(response.ResponseCode);
        Debug.Log(response.ResponseBody);
    }


    public async Task<ICollection<Sticky>> GetStickyNotes()
    {
        const string ENDPOINT = HOST + "stickies/get";
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
        ICollection<Sticky> stickies = JsonConvert.DeserializeObject<ICollection<Sticky>>(response.ResponseBody);
        return stickies;
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
        const string ENDPOINT = HOST + "calendar/get";
        var response = await Rest.GetAsync(ENDPOINT, HEADERS, readResponseData: true);
        IList<CalendarEntry> calendarEntries = JsonConvert.DeserializeObject<IList<CalendarEntry>>(response.ResponseBody);
        return calendarEntries;
    }


    // CAN POTENTIALL REMOVE IN FAVOUR OF ANALYSEMOVEMENT()
    public async Task<bool> DetectFall(string yTransformsJson)
    {
        const string ENDPOINT = HOST + "compute/detectfall";
        var response = await Rest.PostAsync(ENDPOINT, yTransformsJson, HEADERS, readResponseData: true);
        Debug.Log(response.ResponseBody);
        bool fallDetected = bool.Parse(response.ResponseBody);
        Debug.Log(fallDetected);
        return fallDetected;
    }


    public async Task<Response> AnalyseMovement(string transformsJson)
    {
        const string ENDPOINT = HOST + "compute/analysemovement";
        var response = await Rest.PostAsync(ENDPOINT, transformsJson, HEADERS, readResponseData: true);
        //Debug.Log(response.ResponseBody);
        //bool fallDetected = bool.Parse(response.ResponseBody);
        //Debug.Log(fallDetected);
        return response;
    }


    public async void MarkMessageAsRead(PatientMessage patientMessage)
    {
        this.Messages.Remove(patientMessage);
        Debug.Log("Remobing patientmessage w id " + patientMessage.Id);
        string ENDPOINT = HOST + "message/read/?id=" + patientMessage.Id;
        await Rest.GetAsync(ENDPOINT, HEADERS);//, readResponseData: true);
    }
    #endregion
}