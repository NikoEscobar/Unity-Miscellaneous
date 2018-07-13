using System;
using UnityEngine;
using System.Collections;


public class MainCameraBehaviour : MonoBehaviour
{
    public Transform[] cameraSpots = new Transform[numberOfCameraHoldersInScene];
    public Transform[] targets = new Transform[numberOfCameraHoldersInScene];

    private const int numberOfCameraHoldersInScene = 3;
    public int cameraSpeed = 3;

    public CameraPlacementSwapper CameraPlacementSwapperScript;
	
    private LayerMask mask;
    private Camera myCamera;

    public float offset = 0;
    public float camFollow = 0.1f;
    public int cameraClipSpeed = 1;

    private Transform camTransform;
    private Transform pivot;

    void OnValidate()
    {
        if (cameraSpots.Length != numberOfCameraHoldersInScene || targets.Length != numberOfCameraHoldersInScene)
        {
            Debug.LogWarning("Do not change the numberOfCameraHoldersInScene field's array size!");
            Array.Resize(ref cameraSpots, numberOfCameraHoldersInScene);
        }
    }

    void OnEnable()
    {
        pivot = targets[0].transform;
        myCamera = Camera.main;
        camTransform = myCamera.transform;
        camTransform.position = pivot.TransformPoint(Vector3.forward * offset);
		
        mask = 1 << LayerMask.NameToLayer("Clippable") | 0 << LayerMask.NameToLayer("NotClippable");
    }

    void FixedUpdate()
    {
        CameraTrackByPlacement();
        OverlapCameraColliders();

    }

    void TrackingTarget(Transform target, Transform cameraSpot)
    {
        transform.position = Vector3.Slerp(transform.position, cameraSpot.position, Time.fixedDeltaTime * cameraSpeed); 
        transform.LookAt(target);
    }

    CameraPlacementSwapper.CameraPlacement GetCameraPlacement()
    {
        CameraPlacementSwapper.CameraPlacement cameraPlacement = CameraPlacementSwapperScript.CameraPlace;
        return cameraPlacement;
    }

    void CameraTrackByPlacement()
    {
        int i = (int)GetCameraPlacement();
        TrackingTarget(targets[i], cameraSpots[i]);
    }

    void OverlapCameraColliders()
    {
	
        float unobstructed = offset;
        Vector3 idealPosition = pivot.TransformPoint(Vector3.forward * offset);
 
        RaycastHit hit;
        if (Physics.Linecast(pivot.position, idealPosition, out hit, mask.value))
        {
            unobstructed = -hit.distance + 1f;
            Vector3 desiredPosition = pivot.TransformPoint(Vector3.forward * unobstructed);
            Vector3 currentPosition = camTransform.position;
            Vector3 goToPosition = Vector3.Slerp(camTransform.position, desiredPosition, Time.fixedDeltaTime * cameraClipSpeed);
            camTransform.localPosition = goToPosition;
        }      
    }
}


