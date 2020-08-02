using UnityEngine;

public class StickyNotesController : MonoBehaviour
{
    public GameObject stickyNotePrefab;
    private GameObject instantiatedPrefab;


    public void Create()
    {
        // Instantiate the message-panel in front of the user.
        const int DISTANCE_FROM_USER = 2;
        this.instantiatedPrefab = Instantiate(stickyNotePrefab);
        this.instantiatedPrefab.transform.position = Camera.main.transform.position + Camera.main.transform.forward * DISTANCE_FROM_USER;

        IrisService.Instance.PostActivity("Created Sticky");
    }


    public void Delete(GameObject stickyNote)
    {
        // Remove on server.
        // Remove anchor.
        // Unload.
        Destroy(stickyNote);

        IrisService.Instance.PostActivity("Deleted Sticky");
    }


    public void Edit()
    {
        // Update on server.
        IrisService.Instance.PostActivity("Edited Sticky", this.GetStickyContent());
    }


    private string GetStickyContent()
    {
        return this.instantiatedPrefab.GetComponent<StickyNotePrefab>().content.text;
    }
}
