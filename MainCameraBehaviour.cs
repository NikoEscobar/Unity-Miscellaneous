using UnityEngine;
using System.Collections;

public class MainCameraBehaviour : MonoBehaviour
{

    public Transform cameraPosition;
    public Transform Target;
    public Transform cameraPosition2;
    public Transform Target2;
    public Transform cameraPosition3;
    public Transform Target3;

    public int CameraSpeed;

    public CameraPlacementSwapper CameraPlacementSwapperScript;
	
    LayerMask mask;
    public float offset = 0;
    public float camFollow = 0.1f;
    Camera myCamera;
    Transform camTransform;
    Transform pivot;
    public int CameraClipSpeed = 1;

    void OnEnable()
    {
        pivot = Target.transform;
        myCamera = Camera.main;
        camTransform = myCamera.transform;
        
        camTransform.position = pivot.TransformPoint(Vector3.forward * offset);
		
        mask = 1 << LayerMask.NameToLayer("Clippable") | 0 << LayerMask.NameToLayer("NotClippable");
    }

    void Start()
    {

    }

    void FixedUpdate() 
    {
        FollowPlayer1();
        FollowPlayer2();
        FollowPlayer3();
        CentralRay();
    }

    void FollowPlayer1()
    {   
        if (Target != null && CameraPlacementSwapperScript.CameraPlace == CameraPlacementSwapper.CameraPlacement.onPlayerDefault)
        {   
            transform.position = Vector3.Slerp(transform.position, cameraPosition.position, Time.fixedDeltaTime * CameraSpeed);	
            transform.LookAt(Target);
        }
    }

    void FollowPlayer2()
    {
        if (Target != null && CameraPlacementSwapperScript.CameraPlace == CameraPlacementSwapper.CameraPlacement.onPlayerFocus)
        {
            transform.position = Vector3.Slerp(transform.position, cameraPosition2.position, Time.fixedDeltaTime * CameraSpeed);	
            transform.LookAt(Target2);
        }
    }

    void FollowPlayer3()
    {
        if (Target != null && CameraPlacementSwapperScript.CameraPlace == CameraPlacementSwapper.CameraPlacement.onShipPlongee)
        {
            transform.position = Vector3.Slerp(transform.position, cameraPosition3.position, Time.fixedDeltaTime * CameraSpeed);	
            transform.LookAt(Target3);
        }
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
		
            
            Vector3 goToPos = Vector3.Slerp(camTransform.position, desiredPos, Time.fixedDeltaTime * CameraClipSpeed);
 		
            camTransform.localPosition = goToPos;
        }      
    }
}


