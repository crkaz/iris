using System;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public Text debugLog;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DebugLog(string logMsg)
    {
        debugLog.text += logMsg + "\n";
    }

    public void DebugLogTime()
    {
        DateTime d = DateTime.Now;
        debugLog.text += "The time is: " + d.ToString() + "\n";
    }

    public void ClearLog()
    {
        debugLog.text = "";
    }
}
