using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNotesController : MonoBehaviour
{
    public GameObject stickyNotePrefab;


    public void Create()
    {
        const int MAX_DISTANCE = 2;
        GameObject gobj = Instantiate(stickyNotePrefab);
        gobj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * MAX_DISTANCE; // Place sticky in front of user.

    }

    public void Delete(GameObject stickyNote)
    {
        // Remove on server.
        // Remove anchor.
        // Unload.
        Destroy(stickyNote);
    }
    public void Edit()
    {
        // Update on server.
    }
}
