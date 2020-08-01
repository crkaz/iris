using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagesController : MonoBehaviour
{
    public GameObject messagesPanelPrefab;
    public TextMeshProUGUI messageContent;
    private PatientMessage oldestMessage;
    private static bool open = false;

    // What is displayed on the message panel.
    public string MessageContent
    {
        get
        {
            if (oldestMessage == null)
            {
                return "No messages to show.";
            }
            return oldestMessage.Message;
        }
    }


    public void OpenMessagePanel()
    {
        // Prevent multiple instances of the message-panel from opening.
        if (!open)
        {
            // Instantiate the message-panel in front of the user.
            const int MAX_DISTANCE = 2;
            GameObject gobj = Instantiate(messagesPanelPrefab);
            gobj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * MAX_DISTANCE;
            open = true;

            // Display the message.
            LoadMessage();
        }
    }


    public void CloseMessagePanel(GameObject messagePanel)
    {
        Destroy(messagePanel);
        open = false;
    }


    public void ViewNextMessage()
    {
        try
        {
            int index = IrisService.Instance.Messages.IndexOf(this.oldestMessage) + 1;
            this.LoadMessage(index);
        }
        catch
        {
            this.LoadMessage();
        }
    }


    private async void LoadMessage(int index = 0)
    {
        try
        {
            // Set content to oldest message (first in list).
            this.oldestMessage = IrisService.Instance.Messages[index];
            await IrisService.Instance.MarkMessageAsRead(this.oldestMessage.Id); // Mark that message as read on the server and remove from local collections
        }
        catch
        {
            this.oldestMessage = null;
        }
        SetMessageContent();
    }


    private void SetMessageContent()
    {
        this.messagesPanelPrefab.GetComponent<MessagesPanel>().content.text = this.MessageContent;
    }
}
