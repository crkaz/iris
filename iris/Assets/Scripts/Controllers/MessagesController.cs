using System.Collections.Generic;
using UnityEngine;

public class MessagesController : MonoBehaviour
{
    public GameObject messagesPanelPrefab;
    private GameObject instantiatedPrefab;
    private IList<PatientMessage> messages;
    private static bool open = false;


    public void OpenMessagePanel()
    {
        // Prevent multiple instances of the message-panel from opening.
        if (!open)
        {
            // Get messages from iris service.
            this.messages = IrisService.Instance.Messages;

            // Instantiate the message-panel in front of the user.
            const int DISTANCE_FROM_USER = 2;
            this.instantiatedPrefab = Instantiate(messagesPanelPrefab);
            this.instantiatedPrefab.transform.position = Camera.main.transform.position + Camera.main.transform.forward * DISTANCE_FROM_USER;
            open = true;

            // Reset message index on panel open.
            this.SetMessageContent();

            IrisService.Instance.PostActivity("Checked Messages");
        }
    }


    public void CloseMessagePanel(GameObject messagePanel)
    {
        Destroy(messagePanel);
        open = false;
    }


    public void ViewNextMessage()
    {
        this.SetMessageContent();
    }


    private void SetMessageContent()
    {
        string messageContent = "No messages to show";

        try
        {
            this.messages = IrisService.Instance.Messages;
            PatientMessage message = this.messages[0];
            messageContent = message.Message;
            // Mark that message as read on the server and remove from local collection.
            IrisService.Instance.MarkMessageAsRead(message);
        }
        catch
        {
            // No more messages.
        }

        this.instantiatedPrefab.GetComponent<MessagesPanelPrefab>().content.SetText(messageContent);
    }
}
