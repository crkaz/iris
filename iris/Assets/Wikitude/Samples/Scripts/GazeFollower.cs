using UnityEngine;
using System.Collections;

public class GazeFollower : MonoBehaviour
{
    public float distance;
    public float smoothness;
    public float verticalSnap = 0.5f;

    void LateUpdate () {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        var uiDirection = (this.transform.position - headPosition).normalized;
        var wantedPosition = headPosition + gazeDirection * distance;
        var wantedDirection = (wantedPosition - headPosition).normalized;

        var uiEuler = Quaternion.LookRotation(uiDirection).eulerAngles;
        var uiYaw = uiEuler.y;
        var uiPitch = uiEuler.x;

        var wantedEuler = Quaternion.LookRotation(wantedDirection).eulerAngles;
        var wantedYaw = wantedEuler.y;
        var wantedPitch = wantedEuler.x;

        var deltaYaw = (540f + wantedYaw - uiYaw) % 360f - 180f;
        var horizontalHalfAngle = Camera.main.fieldOfView * 0.5f * Camera.main.aspect;
        var verticalHalfAngle = Camera.main.fieldOfView * verticalSnap;
        if (Mathf.Abs(deltaYaw) > horizontalHalfAngle) {
            deltaYaw = -deltaYaw / Mathf.Abs(deltaYaw) * (horizontalHalfAngle - Mathf.Abs(deltaYaw));
        } else {
            deltaYaw = 0f;
        }

        var deltaPitch = (540f + wantedPitch - uiPitch) % 360f - 180f;
        if(Mathf.Abs(deltaPitch) > verticalHalfAngle)
        {
            deltaPitch = -deltaPitch / Mathf.Abs(deltaPitch) * (verticalHalfAngle - Mathf.Abs(deltaPitch));
        }
        else
        {
            deltaPitch = 0f;
        }

        uiEuler.x += deltaPitch;
        uiEuler.y += deltaYaw;
        wantedPosition = headPosition + Quaternion.Euler(uiEuler) * Vector3.forward * distance;

        this.transform.position = Vector3.Lerp(this.transform.position, wantedPosition, smoothness);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(this.transform.position- headPosition), smoothness);
    }
}

