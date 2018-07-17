using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class ShipBehaviour : MonoBehaviour
{
    [HideInInspector]
    public bool applyRotationOnce;

    private bool playerIsOnShipEntrance;
    private bool carryPlayerToShip;

    private Transform Player;
    private Transform VikingShip;
    private Transform landingSpot;

    private VikingControls VikingControlsScript;
    private ShipInsideCollider ShipInsideColliderScript;
    private ShipLandingCollider ShipLandingColliderScript;
    private Viking_anim Viking_animScript;

    public float risingSpeed = 2;
    public float fallingSpeed = 10;

    private float vikingShipOrientation;

    private GameObject playerGameObject;
    private GameObject vikingShipObject;

    void Awake()
    {
        playerGameObject = GameObject.FindWithTag("Player");
        Player = playerGameObject.GetComponent<Transform>();
        VikingControlsScript = playerGameObject.GetComponent<VikingControls>();
        Viking_animScript = playerGameObject.GetComponent<Viking_anim>();

        vikingShipObject = GameObject.FindWithTag("Viking_Ship");
        ShipInsideColliderScript = vikingShipObject.GetComponentInChildren<ShipInsideCollider>();
        ShipLandingColliderScript = vikingShipObject.GetComponentInChildren<ShipLandingCollider>();
        VikingShip = vikingShipObject.GetComponent<Transform>();

        landingSpot = vikingShipObject.GetComponentInChildren<Transform>().GetChild(1).GetChild(2);
    }

    void FixedUpdate()
    {
        CarryPlayerToShip(carryPlayerToShip);
        LaunchPlayerToLandingSpot(Viking_animScript.launchPlayerFromShip);
        LandingPlayer();
    }

    void Update()
    {
        Assert.IsNotNull(VikingControlsScript);
        Assert.IsNotNull(ShipInsideColliderScript);
        Assert.IsNotNull(ShipLandingColliderScript);
        Assert.IsNotNull(Viking_animScript);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsOnShipEntrance = true;
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

    void LaunchPlayerToLandingSpot(bool launch)
    {
        if (launch)
        {
            Vector3 directionToGround = (landingSpot.position - Player.position).normalized;
            Player.transform.position += directionToGround * fallingSpeed * Time.fixedDeltaTime;
            Quaternion inclineHeadForward = Quaternion.LookRotation(directionToGround);
            Player.transform.rotation = inclineHeadForward;
        }
    }

    void LandingPlayer()
    {
        if (ShipLandingColliderScript.PlayerHitExitCollider)
        {
            TurnOnGravity();
            MakePlayerParallelToShip(applyRotationOnce);
        }
    }

    void TurnOnGravity()
    {
        VikingControlsScript.gravity = VikingControlsScript.Lastgravity;
    }

    void MakePlayerParallelToShip(bool rotatePlayer)
    {
        if (rotatePlayer)
        {
            vikingShipOrientation = VikingShip.transform.localEulerAngles.y;
            Player.transform.localEulerAngles = new Vector3(0, vikingShipOrientation, 0);
            applyRotationOnce = false;
        }
    }

    public bool isCarryingPlayerToShip
    {
        get
        {
            return carryPlayerToShip;
        }
    }

    public bool isPlayerOnShipEntrance
    {
        get
        {
            return playerIsOnShipEntrance;
        }
        set
        {
            playerIsOnShipEntrance = value;
        }
    }
}
