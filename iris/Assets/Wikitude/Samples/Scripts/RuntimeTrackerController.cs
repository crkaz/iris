using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using Wikitude;

public class RuntimeTrackerController : SampleController
{
    /* UI control to specify the URL from which the TargetCollectionResource should be loaded. */
    public string Url;
    public GameObject AugmentationPrefab;

    public GameObject Instructions;

    private ImageTracker _currentTracker;
    /* Flag to keep track if a tracker is currently loading. */
    private bool _isLoadingTracker = false;

    public void OnLoadTracker() {
        if (_isLoadingTracker) {
            /* Wait until previous request was completed. */
            return;
        }
        /* Destroy any previously loaded tracker. */
        if (_currentTracker != null) {
            Destroy(_currentTracker.gameObject);
        }

        _isLoadingTracker = true;

        /* Create and configure the tracker. */
        GameObject trackerObject = new GameObject("ImageTracker");
        _currentTracker = trackerObject.AddComponent<ImageTracker>();
        _currentTracker.TargetSourceType = TargetSourceType.TargetCollectionResource;
        _currentTracker.TargetCollectionResource = new TargetCollectionResource();
        _currentTracker.TargetCollectionResource.UseCustomURL = true;
        _currentTracker.TargetCollectionResource.TargetPath = Url;

        _currentTracker.TargetCollectionResource.OnFinishLoading.AddListener(OnFinishLoading);
        _currentTracker.TargetCollectionResource.OnErrorLoading.AddListener(OnErrorLoading);

        _currentTracker.OnTargetsLoaded.AddListener(OnTargetsLoaded);
        _currentTracker.OnErrorLoadingTargets.AddListener(OnErrorLoadingTargets);
        _currentTracker.OnInitializationError.AddListener(OnInitializationError);

        /* Create and configure the trackable. */
        GameObject imageTrackable = new GameObject ("ImageTrackable");
        imageTrackable.transform.SetParent (_currentTracker.transform, false);
        ImageTrackable trackable = imageTrackable.AddComponent<ImageTrackable> ();

        trackable.Drawable = AugmentationPrefab;
    }

    public override void OnTargetsLoaded() {
        base.OnTargetsLoaded();
        Instructions.SetActive(true);
        _isLoadingTracker = false;
    }

    public override void OnErrorLoadingTargets(Error error) {
        base.OnErrorLoadingTargets(error);
        _isLoadingTracker = false;
    }
}
