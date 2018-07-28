using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Assertions;

public class VikingControls : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;
    public float turnSpeed = 200.0f;

    private float turn;
    private float walk;

    private float defaultSpeed;
    private float defaultJumpForce;
    private float defaultTurnSpeed;
    private float defaultGravity;

    private bool tryingToMove;
    private bool jump;
    private bool attack;
    private bool defense;
    private bool specialAttack;

    private VikingAnimator VikingAnimatorScript;

    private Vector3 movement = Vector3.zero;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        VikingAnimatorScript = GetComponent<VikingAnimator>();
    }

    void Start()
    {	
        defaultSpeed = speed;
        defaultJumpForce = jumpForce;
        defaultTurnSpeed = turnSpeed;
        defaultGravity = gravity;

        FreezePlayerMovement(false);
    }

    void FixedUpdate()
    {   
        SetInputsByPlataform();
        WalkingControls(turn, walk);
    }

    #if UNITY_EDITOR
    void Update()
    {
        Assert.IsNotNull(VikingAnimatorScript);
    }
    #endif

    void SetInputsByPlataform()
    {
        #if MOBILE_INPUT
        turn = CrossPlatformInputManager.GetAxis("Horizontal_ThirdPerson");
        walk = CrossPlatformInputManager.GetAxis("Vertical_ThirdPerson");
        jump = CrossPlatformInputManager.GetButton("Jump");
        attack = CrossPlatformInputManager.GetButton("Fire1");
        defense = CrossPlatformInputManager.GetButton("Fire2");
        specialAttack = CrossPlatformInputManager.GetButton("E");

        #else
        turn = Input.GetAxis("Horizontal");
        walk = Input.GetAxis("Vertical");
        jump = Input.GetButton("Jump");
        attack = Input.GetButton("Fire1");
        defense = Input.GetButton("Fire2");
        specialAttack = Input.GetButton("E");

        #endif
    }

    void WalkingControls(float turnCharacterInput, float moveCharacterInput)
    {        
        if (controller.isGrounded)
        {	                                        
            ApplyCharacterMovement(turnCharacterInput, moveCharacterInput);
            JumpControl();
        }
        ApplyGravity();
        controller.Move(movement * Time.fixedDeltaTime);

        tryingToMove = IsMovementInputsHeldDown(turnCharacterInput, moveCharacterInput);
    }

    public void FreezePlayerMovement(bool frozen)
    {
        if (frozen)
        {
            speed = 0f;
            jumpForce = 0f;
            turnSpeed = 0f;
        }

        if (!frozen)
        {
            speed = defaultSpeed;
            jumpForce = defaultJumpForce;
            turnSpeed = defaultTurnSpeed;
        }
    }

    bool IsMovementInputsHeldDown(float horizontalControllerInput, float verticalControllerInput)
    {
        if (horizontalControllerInput != 0 || verticalControllerInput != 0)
            return true;
        else
            return false;
    }

    void ApplyCharacterMovement(float turnInput, float moveInput)
    {
        movement = new Vector3(0, -1f, moveInput);
        movement = transform.TransformDirection(movement);
        movement *= speed;
        this.transform.Rotate(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
    }

    void JumpControl()
    {
        if (VikingAnimatorScript.IsJumpInBoostingAnimation())
        {
            movement.y = jumpForce;
        }
    }

    void ApplyGravity()
    {
        movement.y -= gravity * Time.fixedDeltaTime;
    }

    public void RestoreGravity()
    {
        gravity = defaultGravity;
    }

    public void ApplyInertiaToPlayer()
    {
        speed = 0f;
        gravity = 0f;
    }

    public bool IsTryingToMove
    {
        get
        {
            return tryingToMove;
        }
    }

    public bool JumpButton
    {
        get
        {
            return jump;
        }
    }

    public bool AttackButton
    {
        get
        {
            return attack;
        }
    }

    public bool DefenseButton
    {
        get
        {
            return defense;
        }
    }

    public bool SpecialAttackButton
    {
        get
        {
            return specialAttack;
        }
    }
}