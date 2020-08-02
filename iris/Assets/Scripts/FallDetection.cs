using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetection : MonoBehaviour
{
    private const float ANALYSIS_FREQUENCY = 10.0f; // How many seconds until the transforms should be processed.
    private const float UPDATE_FREQUENCY = 1.0f; // How frequently the current y-pos should be recorded.
    private List<float> yTransforms = new List<float>();

    // Update is called once per frame
    void Start()
    {
        StartCoroutine("AddTransform");
        StartCoroutine("AnalyseMovement");

    }

    private IEnumerator AddTransform()
    {
        for (; ; )
        {
            this.yTransforms.Add(Camera.main.transform.position.y);
            yield return new WaitForSeconds(UPDATE_FREQUENCY);
        }
    }

    private IEnumerator AnalyseMovement()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(ANALYSIS_FREQUENCY);
            this.Analyse();
        }
    }

    private async void Analyse()
    {
        // If the specified amount of time HAS passed: analyse transforms.
        string yTransformsJson = JsonConvert.SerializeObject(yTransforms); // convert to json
        bool fallDetected = await IrisService.Instance.DetectFall(yTransformsJson); // send to server for analysis

        if (fallDetected)
        {
            HandleFall();
        }

        // Clear the stored transforms and repeat the process.
        this.yTransforms.Clear();
    }


    private void HandleFall()
    {
        Debug.Log("Fall detected.");
    }
}
