using System.Collections;
using UnityEngine;

public class IrisNotificationsService : MonoBehaviour
{
    // Implements singleton pattern.
    public static IrisNotificationsService Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject notificationsBarPrefab;

    private const float NOTIFICATION_LIFETIME = 3.0f; // How long notif should appear for.


    public void Notify(string notificationText, float duration = NOTIFICATION_LIFETIME)
    {
        StartCoroutine(DisplayNotification(notificationText, duration));
    }


    private IEnumerator DisplayNotification(string notificationText, float duration)
    {
        this.ShowNotificationBar(notificationText);
        yield return new WaitForSeconds(duration);
        this.HideNotificationBar();
    }


    private void ShowNotificationBar(string notificationText)
    {
        this.notificationsBarPrefab.SetActive(true);
        SetNotificationText(notificationText);
    }


    private void HideNotificationBar()
    {
        this.notificationsBarPrefab.SetActive(false);
    }


    private void SetNotificationText(string notificationText)
    {
        this.notificationsBarPrefab.GetComponent<NotificationBarPrefab>().notificationText.text = notificationText;
    }
}
