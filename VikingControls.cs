using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class VikingControls : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float turnSpeed = 200.0f;

    public bool TryingToMove;

    public Viking_anim Viking_animScript;

    public ShipBehaviour ShipAnimScript;

    private Vector3 moveDirection = Vector3.zero;

    float LastSpeed;
    float LastJumpSpeed;
    float LastturnSpeed;
    public float Lastgravity;

    void Start()
    {	
        LastSpeed = speed;
        LastJumpSpeed = jumpSpeed;
        LastturnSpeed = turnSpeed;
        Lastgravity = gravity;

        FreezePlayerMovement(false);
    }

    void FixedUpdate()
    {   
        #if MOBILE_INPUT
		VickCommand_Touch ();
        #else
        VickCommand();
        #endif
    }

    void VickCommand()
    {
        CharacterController controller = GetComponent<CharacterController>();
        float turn = Input.GetAxis("Horizontal");

        if (controller.isGrounded)
        {	
            moveDirection = new Vector3(0, -1f, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            this.transform.Rotate(0, turn * turnSpeed * Time.fixedDeltaTime, 0);

            if (Viking_animScript.jumpBoost == true)
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.fixedDeltaTime;
        controller.Move(moveDirection * Time.fixedDeltaTime);

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            TryingToMove = false;
        }
        else
            TryingToMove = true;

    }

    void VickCommand_Touch()
    {
        CharacterController controller = GetComponent<CharacterController>();
        float turn = CrossPlatformInputManager.GetAxis("Horizontal_ThirdPerson");


        if (controller.isGrounded)
        {	
            moveDirection = new Vector3(0, -1f, CrossPlatformInputManager.GetAxis("Vertical_ThirdPerson"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            this.transform.Rotate(0, turn * turnSpeed * Time.fixedDeltaTime, 0);

            if (Viking_animScript.jumpBoost == true)
                moveDirection.y = jumpSpeed;
        }	
        moveDirection.y -= gravity * Time.fixedDeltaTime;
        controller.Move(moveDirection * Time.fixedDeltaTime);

        if (CrossPlatformInputManager.GetAxis("Horizontal_ThirdPerson") == 0 && CrossPlatformInputManager.GetAxis("Vertical_ThirdPerson") == 0)
        {
            TryingToMove = false;
        }
        else
            TryingToMove = true;
    }


    public void FreezePlayerMovement(bool frozen)
    {
        if (frozen)
        {
           
            speed = 0f;
            jumpSpeed = 0f;
            turnSpeed = 0f;
        }

        if (!frozen)
        {
           
            speed = LastSpeed;
            jumpSpeed = LastJumpSpeed;
            turnSpeed = LastturnSpeed;
        }
    }
}