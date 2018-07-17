using UnityEngine;
using System.Collections;

public class ShipBehaviour : MonoBehaviour
{
    [HideInInspector]
    public bool onShipEntrance;
    [HideInInspector]
    public bool applyRotationOnce;

    private bool carryPlayerToShip;

    public Transform Player;
    public Transform VikingShip;
    public Transform groundSpot;

    public VikingControls VikingControlsScript;
    public ShipInsideCollider ShipInsideColliderScript;
    public ShipLandingCollider ShipLandingColliderScript;
    public Viking_anim Viking_animScript;

    public float risingSpeed = 2;
    public float fallingSpeed = 10;

    private float vikingShipOrientation;

    void FixedUpdate()
    {
        CarryPlayerToShip(carryPlayerToShip);
        LaunchPlayerToGroundSpot(Viking_animScript.launchPlayerFromShip);
        LandingPlayer();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            onShipEntrance = true;
            carryPlayerToShip = true;
        } 
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            VikingControlsScript.FreezePlayerMovement(true);
        }
    }

    void ApplyInertiaToPlayer()
    {
        VikingControlsScript.speed = 0f;
        VikingControlsScript.gravity = 0f;
    }

    void CarryPlayerToShip(bool isCarrying)
    {
        if (isCarrying)
        {
            VikingControlsScript.FreezePlayerMovement(true);
            ApplyInertiaToPlayer();
            Vector3 directionToShip = (VikingShip.position - Player.position).normalized;
            Player.transform.position += directionToShip * risingSpeed * Time.fixedDeltaTime;
        }
        if (ShipInsideColliderScript.PlayerIsInsideShip)
        {
            carryPlayerToShip = false;
        }

    }

    void LaunchPlayerToGroundSpot(bool launch)
    {
        if (launch)
        {
            Vector3 directionToGround = (groundSpot.position - Player.position).normalized;
            Player.transform.position += directionToGround * fallingSpeed * Time.fixedDeltaTime;
            Quaternion inclineHeadForward = Quaternion.LookRotation(directionToGround);
            Player.transform.rotation = inclineHeadForward;
        }
    }

    void LandingPlayer()
    {
        if (ShipLandingColliderScript.PlayerHitExitCollider)
        {
            vikingShipOrientation = VikingShip.transform.localEulerAngles.y;
            TurnOnGravity();

            if (applyRotationOnce)
            {
                Player.transform.localEulerAngles = new Vector3(0, vikingShipOrientation, 0);
                applyRotationOnce = false;
            }

        }
    }

    void TurnOnGravity()
    {
        VikingControlsScript.gravity = VikingControlsScript.Lastgravity;
    }

    public bool isCarryingPlayerToShip
    {
        get
        {
            return carryPlayerToShip;
        }
    }
}
