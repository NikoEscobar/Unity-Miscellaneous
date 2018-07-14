using UnityEngine;
using System.Collections;

public class ShipBehaviour : MonoBehaviour
{
	
    public bool shipIn;
    public bool goToShip;
    public bool goToGround;
    public bool runOnce;
    public Transform Player;
    public Transform Ship;
    public Transform groundSpot;
    public VikingControls VikingControlsScript;
    public ShipInsideCollider ShipInsideColliderScript;
    public Viking_anim Viking_animScript;
    public ShipOutLand ShipOutLandScript;

    public float velocidade = 2;
    public float velocidadeQueda = 10;

    public float range;

    public float shipYRot;

    void FixedUpdate()
    {
        controleDaDistancia();
        OnShip();
        OutaShip();
        VickLand();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            shipIn = true;
            goToShip = true;
            ShipOutLandScript.vLand = false;
        } 
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            VikingControlsScript.Frozen(true);
        }
    }

    void GoToShip(bool go)
    {
        if (go)
        {
            if (velocidade <= 0f)
            {
                velocidade = 2f;
            }
            VikingControlsScript.speed = 0f;
            VikingControlsScript.gravity = 0f;
            Vector3 direcao = (Ship.position - Player.position).normalized;
            Player.transform.position += direcao * velocidade * Time.fixedDeltaTime;
        }
        else
        {
            VikingControlsScript.gravity = VikingControlsScript.Lastgravity;
            return;
        }
    }

    void GoToGround(bool go)
    {
        if (go)
        {
            VikingControlsScript.gravity = 0f;
            Vector3 direcao = (groundSpot.position - Player.position).normalized;
            Player.transform.position += direcao * velocidadeQueda * Time.fixedDeltaTime;
            Quaternion olharPara = Quaternion.LookRotation(direcao);
            Player.transform.rotation = olharPara;
        }
        else
        {
            VikingControlsScript.gravity = VikingControlsScript.Lastgravity;
            return;
        }
    }

    void controleDaDistancia()
    {
        if (goToShip == true)
        {
            GoToShip(true);
        } 

        if (goToGround == true)
        {
            goToShip = false;
            GoToShip(false);
            GoToGround(true);
        }
    }

    void OnShip()
    {
        if (ShipInsideColliderScript.playerIsInsideShip == true)
        {
            velocidade = 0f;
            goToShip = false;
        } 
    }

    void OutaShip()
    {
        if (Viking_animScript.shipOut == true)
        {
            goToGround = true;
        }
    }

    void VickLand()
    {
        if (ShipOutLandScript.vLand == true)
        {
            Transform Ship = GameObject.Find("Ship_Prototype").transform;
            shipYRot = Ship.transform.localEulerAngles.y;
            goToGround = false;
            VikingControlsScript.gravity = VikingControlsScript.Lastgravity;

            if (runOnce)
            {
                Player.transform.localEulerAngles = new Vector3(0, shipYRot, 0);
                runOnce = false;
            }

        }
    }
}
