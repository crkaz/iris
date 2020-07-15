using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Pointer : MonoBehaviour
{
    public GraphicRaycaster UIGraphicsRaycaster;
    public EventSystem EventSystem;

    private Camera _mainCamera;

    void Start () {
        _mainCamera = Camera.main;
    }
    
    void Update () {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = _mainCamera.transform.position;
        var gazeDirection = _mainCamera.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo)) {
            // If the raycast hit a hologram...
            UpdatePointer(hitInfo.point, hitInfo.normal);
        } else {
            // Test for UI elements
            var pointerEventData = new PointerEventData(EventSystem);
            pointerEventData.position = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);

            var results = new List<RaycastResult>();
            UIGraphicsRaycaster.Raycast(pointerEventData, results);

            if (results.Count > 0) {
                // Results are sorted by distance
                UpdatePointer(headPosition + gazeDirection * results[0].distance, -gazeDirection);
            } else {
                UpdatePointer(headPosition + gazeDirection * 10.0f, -gazeDirection);
            }
        }
    }

    void UpdatePointer(Vector3 position, Vector3 normal) {
        // Move the cursor to the point where the raycast hit.
        // Offset the position by one cm to avoid z-fighting with planar surfaces.
        transform.position = position + normal * 0.01f;

        // Rotate the cursor to hug the surface of the hologram.
        transform.rotation = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }
}

