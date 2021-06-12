using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject ObjectToPlace;
    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycastManager;
    private Pose PlacementPose;
    //private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            placeObject();
        }   
    }

    private void placeObject()
    {
        Instantiate(ObjectToPlace, PlacementPose.position, PlacementPose.rotation);
    }

    void UpdatePlacementIndicator()
    {
        if(placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if(placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
            var cameraFarward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraFarward.x, 0, cameraFarward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }


}
