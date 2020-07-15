using UnityEngine;
using UnityEngine.XR.WSA.Input;
using Wikitude;

public class MultipleTrackersController : SampleController
{
    public ImageTracker RocketTracker;
    public ImageTracker FlowerFarmerTracker;

    /* Prefabs that display which targets should be scanned, based on which tracker is active. */
    public GameObject RocketInstructions;
    public GameObject FlowerFarmerInstructions;

    /* Flag to keep track if the Wikitude SDK is currently transitioning from one tracker to another.
     * We shouldn't start another transition until the previous one completed.
     */
    private bool _waitingForTrackerToLoad = false;

    public void OnTrackRocket() {
        if (!RocketTracker.enabled && !_waitingForTrackerToLoad) {
            _waitingForTrackerToLoad = true;

            FlowerFarmerInstructions.SetActive(false);
            RocketInstructions.SetActive(true);

            RocketTracker.enabled = true;
        }
    }

    public void OnTrackFlowerFarmer() {
        if (!FlowerFarmerTracker.enabled && !_waitingForTrackerToLoad) {
            _waitingForTrackerToLoad = true;
            FlowerFarmerInstructions.SetActive(true);

            RocketInstructions.SetActive(false);

            FlowerFarmerTracker.enabled = true;
        }
    }

    public override void OnTargetsLoaded() {
        _waitingForTrackerToLoad = false;
    }
}
