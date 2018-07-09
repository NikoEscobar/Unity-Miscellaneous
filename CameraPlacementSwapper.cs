using UnityEngine;
using System.Collections;

public class CameraPlacementSwapper : MonoBehaviour
{

    //botao momentaneo, e apropriado criar um setup ou um botao no hud para isso
    public bool cameraSwitch1;
    public bool cameraSwitch2;
    public bool cameraSwitch3;
    public int cameraStyle;

    private int mainCameraStyle;
    private bool returnCameraDestrava;

    public ShipInside ShipInsideScript;
    public ShipCamFix ShipCamFixScript;

    // Use this for initialization
    void Start()
    {
        cameraSwitch1 = true;
        cameraSwitch2 = false;
        cameraStyle = 1;
        mainCameraStyle = 1;

    }
	
    // Update is called once per frame
    void Update()
    {
        CameraSwap();
        ForceCamera();
    }

    void CameraSwap()
    {
        if (ShipInsideScript.insideShip == true || ShipCamFixScript.GoingToShip == true)
        {
            cameraSwitch3 = true;
            cameraSwitch2 = false;
            cameraSwitch1 = false;
            cameraStyle = 3;
            returnCameraDestrava = true;
        }

        if (Input.GetButtonUp("CameraSwap") && cameraStyle == 1)
        {
            cameraSwitch2 = true;
            cameraSwitch1 = false;
            cameraStyle = 2;
            mainCameraStyle = 2;
        }
        else if (Input.GetButtonUp("CameraSwap") && cameraStyle == 2)
        {
            cameraSwitch2 = false;
            cameraSwitch1 = true;
            cameraStyle = 1;
            mainCameraStyle = 1;
        }
    }

    void ForceCamera()
    {
        if (ShipInsideScript.insideShip == false && ShipCamFixScript.GoingToShip == false)
        {
            if (mainCameraStyle == 1 && returnCameraDestrava == true)
            {
                cameraSwitch2 = false;
                cameraSwitch1 = true;
                cameraStyle = 1;
                returnCameraDestrava = false;
            }
            else if (mainCameraStyle == 2 && returnCameraDestrava == true)
            {
                cameraSwitch2 = true;
                cameraSwitch1 = false;
                cameraStyle = 2;
                returnCameraDestrava = false;
            }
        }
    }
}
