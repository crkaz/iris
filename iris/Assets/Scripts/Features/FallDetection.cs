using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities; // Include MRTK REST client.

public class FallDetection : MonoBehaviour
{
    void Start()
    {
        Get();
        Get2();
        Post();
        Put();
        Delete();
        // DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
    }

    private async void Get()
    {
        const string endpoint = "http://localhost:54268/api/values";
        var response = await Rest.GetAsync(endpoint, headers: null, readResponseData: true);
        Debug.Log(response.ResponseBody);
    }

    private async void Get2()
    {
        const string endpoint = "http://localhost:54268/api/values/1";
        var response = await Rest.GetAsync(endpoint, headers: null, readResponseData: true);
        Debug.Log(response.ResponseBody);
    }

    private async void Post()
    {
        const string endpoint = "http://localhost:54268/api/values";
        string body = "1";
        var response = await Rest.PostAsync(endpoint, jsonData: body, headers: null, readResponseData: true);
        Debug.Log(response.ResponseBody);
    }

    private async void Put()
    {
        const string endpoint = "http://localhost:54268/api/values/1";
        string body = "1";
        var response = await Rest.PutAsync(endpoint, jsonData: body, headers: null, readResponseData: true);
        Debug.Log(response.ResponseBody);
    }

    private async void Delete()
    {
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("apiKey", "1902401924809");
        const string endpoint = "http://localhost:54268/api/values/1";
        var response = await Rest.DeleteAsync(endpoint, headers: header, readResponseData: true);
        Debug.Log(response.ResponseBody);
    }
}
