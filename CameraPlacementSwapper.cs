using UnityEngine;
using System.Collections;

public class CameraPlacementSwapper : MonoBehaviour
{

    public int cameraStyle;

    public ShipInsideCollider ShipInsideColliderScript;
    public ShipCamFix ShipCamFixScript;

    private bool returnCameraTrigger = false;

    private enum CameraPlacement
    {
        onPlayerDefault,
        onPlayerFocus,
        onShipPlongee}

    ;

    [SerializeField]
    private CameraPlacement CameraPlace;

    void Start()
    {
        cameraStyle = 1;
        CameraPlace = CameraPlacement.onPlayerDefault;

    }

    void Update()
    {
        CameraPlacementSwap();
    }

    void CameraPlacementSwap()
    {
        if (ShipInsideColliderScript.insideShip == true || ShipCamFixScript.GoingToShip == true)
        {
            cameraStyle = 3;
            CameraPlace = CameraPlacement.onShipPlongee;
            returnCameraTrigger = true;
        }
        else if (ShipInsideColliderScript.insideShip == false && ShipCamFixScript.GoingToShip == false && returnCameraTrigger == true)
        {
            cameraStyle = 1;
            CameraPlace = CameraPlacement.onPlayerDefault;
            returnCameraTrigger = false;
        }

        if (Input.GetButtonUp("CameraSwap") && cameraStyle == 1)
        {
            cameraStyle = 2;
            CameraPlace = CameraPlacement.onPlayerFocus;
        }
        else if (Input.GetButtonUp("CameraSwap") && cameraStyle == 2)
        {
            cameraStyle = 1;
            CameraPlace = CameraPlacement.onPlayerDefault;
        }
    }
}
