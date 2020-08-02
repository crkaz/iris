using UnityEngine;
using System.Collections.Generic;

public class CalendarController : MonoBehaviour
{
    public GameObject calendarPanelPrefab;
    private GameObject instantiatedPrefab;
    private IList<CalendarEntry> entries;
    private static bool open = false;

    // public Text buttonText;
    // public string displayedEvents;
    // private string todaysEvents;
    // private string tomorrowsEvents;


    public void OpenCalendarPanel()
    {
        if (!open)
        {
            // Get ref to calendar entries in irisservice.
            this.entries = IrisService.Instance.CalendarEntries;

            // Instantiate the message-panel in front of the user.
            const int MAX_DISTANCE = 2;
            this.instantiatedPrefab = Instantiate(calendarPanelPrefab);
            this.instantiatedPrefab.transform.position = Camera.main.transform.position + Camera.main.transform.forward * MAX_DISTANCE;
            open = true;

            this.SetPanelContent();

            IrisService.Instance.PostActivity("Checked Calendar");
        }
    }


    public void CloseCalendarPanel(GameObject calendarPanel)
    {
        Destroy(calendarPanel);
        open = false;
    }


    private void SetPanelContent()
    {
        string panelConent = "";

        try
        {
            const int MAX_DISPLAY = 3;
            int i = 0;
            foreach (var e in this.entries)
            {
                panelConent += e.Start.ToShortDateString();
                panelConent += "  -  " + e.Start.ToShortTimeString();
                panelConent += "<br>" + e.Description;
                panelConent += "<br><br>";
                if (++i > MAX_DISPLAY - 1)
                {
                    break;
                }
            }
        }
        catch
        {
            panelConent = "No entries to show";
        }

        this.instantiatedPrefab.GetComponent<CalendarPanelPrefab>().content.SetText(panelConent);
    }
}
