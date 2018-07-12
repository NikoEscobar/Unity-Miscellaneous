using UnityEngine;
using System.Collections;

public class MainCameraBehaviour : MonoBehaviour
{
    public Transform[] cameraSpots;
    public Transform[] targets;
   
    public int cameraSpeed = 3;

    public CameraPlacementSwapper CameraPlacementSwapperScript;
	
    LayerMask mask;
    public float offset = 0;
    public float camFollow = 0.1f;
    Camera myCamera;
    Transform camTransform;
    Transform pivot;
    public int cameraClipSpeed = 1;

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
        CentralRay();

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

    void CentralRay()
    {
	
        float unobstructed = offset;
        Vector3 idealPostion = pivot.TransformPoint(Vector3.forward * offset);
 
        RaycastHit hit;
        if (Physics.Linecast(pivot.position, idealPostion, out hit, mask.value))
        {
            unobstructed = -hit.distance + 1f;

		
            Vector3 desiredPos = pivot.TransformPoint(Vector3.forward * unobstructed);
            Vector3 currentPos = camTransform.position;
		
            Vector3 goToPos = Vector3.Slerp(camTransform.position, desiredPos, Time.fixedDeltaTime * cameraClipSpeed);
 		
            camTransform.localPosition = goToPos;
        }      
    }
}


