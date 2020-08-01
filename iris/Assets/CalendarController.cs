using UnityEngine;
using UnityEngine.UI;

public class CalendarController : MonoBehaviour
{
    public GameObject calendarPanelPrefab;
    public Text buttonText;
    public string displayedEvents;
    private string todaysEvents;
    private string tomorrowsEvents;
    private static bool open = false;


    public void OpenCalendarPanel()
    {
        if (!open)
        {
            const int MAX_DISTANCE = 2;
            GameObject gobj = Instantiate(calendarPanelPrefab);
            gobj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * MAX_DISTANCE; // Place in front of user.
            open = true;
        }
    }


    public void CloseCalendarPanel(GameObject calendarPanel)
    {
        Destroy(calendarPanel);
        open = false;
    }


    public void ToggleDay()
    {
        if (buttonText.text == "View Tomorrow")
        {
            displayedEvents = todaysEvents;
            buttonText.text = "View Today";
        }
        else
        {
            displayedEvents = tomorrowsEvents;
            buttonText.text = "View Tomorrow";
        }
    }
}
