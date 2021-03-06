﻿using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class CameraPlacementSwapper : MonoBehaviour
{
    public ShipInsideCollider ShipInsideColliderScript;
    public ShipCameraCollider ShipCameraColliderScript;

    private bool HoldCameraPreference = false;

    [HideInInspector]
    public enum CameraPlacement
    {
        onPlayerDefault,
        onPlayerFocus,
        onShipPlongee}

    ;

    [HideInInspector]
    public CameraPlacement CameraPlace;

    void Start()
    {
        PutCameraOnDefault();
    }

    void Update()
    {
        CameraPlacementSwapByCollision();
        SwapCameraByButton();

        Assert.IsNotNull(ShipInsideColliderScript);
        Assert.IsNotNull(ShipCameraColliderScript);
    }

    void CameraPlacementSwapByCollision()
    {
        if (IsPlayerInsideShip())
        {
            PutCameraOnShip();
        }
        else if (!IsPlayerInsideShip() && HoldCameraPreference == true)
        {
            PutCameraOnDefault();
        }
    }

    void PutCameraOnShip()
    {
        CameraPlace = CameraPlacement.onShipPlongee;
        HoldCameraPreference = true;
    }

    void PutCameraOnDefault()
    {
        CameraPlace = CameraPlacement.onPlayerDefault;
        HoldCameraPreference = false;
    }

    void SwapCameraByButton()
    {
        if (Input.GetButtonUp("Focus Attention") && CameraPlace == CameraPlacement.onPlayerDefault)
        {
            CameraPlace = CameraPlacement.onPlayerFocus;
        }
        else if (Input.GetButtonUp("Focus Attention") && CameraPlace == CameraPlacement.onPlayerFocus)
        {
            CameraPlace = CameraPlacement.onPlayerDefault;
        }
    }

    private bool IsPlayerInsideShip()
    {
        bool playerOnShip;
        if (ShipInsideColliderScript.isPlayerInsideShip || ShipCameraColliderScript.isCameraInsideShip)
        {
            playerOnShip = true;
        }
        else
            playerOnShip = false;
        return playerOnShip;
    }
}
