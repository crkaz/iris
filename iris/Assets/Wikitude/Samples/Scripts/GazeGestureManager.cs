using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.Events;
using System.Collections;

public class GazeGestureManager : MonoBehaviour
{

    public GameObject _pointedObject { get; private set; }

    GestureRecognizer recognizer;

    void Awake() {
        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.Tapped += (args) => {
            // Send an OnSelect message to the focused object and its ancestors.
            if (_pointedObject != null) {
                _pointedObject.SendMessageUpwards("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        };
        recognizer.StartCapturingGestures();
    }

    void Update () {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo)) {
            // If the raycast hit a hologram...
            // Display the cursor mesh.
            //meshRenderer.enabled = true;

            // Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;

            // Rotate the cursor to hug the surface of the hologram.
            //this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

            _pointedObject = hitInfo.collider.gameObject;
        } else {
            // If the raycast did not hit a hologram, hide the cursor mesh.
            //meshRenderer.enabled = false;
        }
    }
}

