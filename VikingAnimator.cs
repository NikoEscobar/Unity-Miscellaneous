using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Assertions;

public class VikingAnimator : MonoBehaviour
{
    private GameObject vikingShipObject;

    private Animator animator;

    private CharacterController controller;
    private AnimatorStateInfo stateInfo;

    private VikingControls VikingControlsScript;
    private ShipBehaviour ShipBehaviourScript;
    private ShipInsideCollider ShipInsideColliderScript;
    private ShipLandingCollider ShipLandingColliderScript;

    public AnyDamage AnyDamageScript;

    private int jumpHash = Animator.StringToHash("Base Layer.Jump");
    [SerializeField]
    private int eventsInARow = 0;
    [SerializeField]
    private float secondsPastWhileSleeping;
    [HideInInspector]
    public bool launchPlayerFromShip;

    private bool animPlayOnce = true;
    private bool animPlayOnce2 = false;
    private bool isAttacking = false;
    private bool runAnimationOnce = true;

    private AnimatorStateInfo currentBaseState;
    private AnimatorStateInfo layer2CurrentState;
	
    //[New animations viking1.1]--------------------------------------------
    static int jump_Start = Animator.StringToHash("Base Layer.Jump_Start");
    static int jump_Boost = Animator.StringToHash("Base Layer.Jump_Boost");
    static int jump_Falling = Animator.StringToHash("Base Layer.Jump_Falling");
    static int jump_Fall = Animator.StringToHash("Base Layer.Jump_Fall");
    //[New animations viking1.2]--------------------------------------------
    static int shipInState = Animator.StringToHash("Base Layer.Ship_In");
    static int shipOutStartState = Animator.StringToHash("Base Layer.Ship_Out_Start");
    static int shipFallLoopState = Animator.StringToHash("Base Layer.Ship_Fall_Loop");
    static int shipFallEndState = Animator.StringToHash("Base Layer.Ship_Fall_End");
    //[New animations viking1.3]--------------------------------------------
    static int Run_Start_State = Animator.StringToHash("Base Layer.Run_Start");
    static int Run_Loop_State = Animator.StringToHash("Base Layer.Run_Loop");
    static int NeoIdle_State = Animator.StringToHash("Base Layer.Idle_States.Idle_Loop");
    static int SleepLoop_State = Animator.StringToHash("Base Layer.Idle_States.Sleep_Loop");
    //[New animations viking1.4]--------------------------------------------
    static int Atk1_State = Animator.StringToHash("Base Layer.Atk1");
    static int Def1_State = Animator.StringToHash("Base Layer.Def_axeShield");
    static int Def_axeShield_State = Animator.StringToHash("Base Layer.Def_axeShield");
    static int Def_shieldStand_State = Animator.StringToHash("Base Layer.Def_shieldStand");
    static int Skalagrim_throw_State = Animator.StringToHash("Base Layer.Skalagrim_throw");
    static int Damage_Front_State = Animator.StringToHash("Base Layer.Damage_Front");
    static int Damage_Back_State = Animator.StringToHash("Base Layer.Damage_Back");

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        VikingControlsScript = GetComponent<VikingControls>();

        vikingShipObject = GameObject.FindWithTag("Viking_Ship");
        ShipBehaviourScript = vikingShipObject.GetComponentInChildren<ShipBehaviour>();
        ShipInsideColliderScript = vikingShipObject.GetComponentInChildren<ShipInsideCollider>();
        ShipLandingColliderScript = vikingShipObject.GetComponentInChildren<ShipLandingCollider>();

    }

    void Start()
    {   
        animator = GetComponent<Animator>();
        if (animator.layerCount == 2)
            animator.SetLayerWeight(1, 1);
    }

    #if UNITY_EDITOR
    void Update()
    {
        Assert.IsNotNull(VikingControlsScript);
        Assert.IsNotNull(ShipBehaviourScript);
        Assert.IsNotNull(ShipInsideColliderScript);
        Assert.IsNotNull(ShipLandingColliderScript);
    }
    #endif

    void FixedUpdate()
    {	
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        PreventMovementByAnimation();
        SetWalkAnimatorBool();
        SetFallingAnimatorBool();
        GroundedOnlyAnimatorTriggers();
        SetShipAnimatorTrigger();
        SetAttackAnimatorBool();
        SetDefenseAnimatorBool();
        SetKnockbacksAnimatorTriggers();
        SetDamageAnimatorTriggers();		
        SetIdlesEventsAnimatorStates();
    }

    IEnumerator SetRandomIdleEventTrigger()
    { 	
        yield return new WaitForSeconds(30f);
        while (!VikingControlsScript.IsTryingToMove)
        {
            if (eventsInARow <= 3)
            {
                int randomState = (Random.Range(0, 9));
                eventsInARow += 1;
                switch (randomState)
                {
                    case 0:
                        animator.SetTrigger("Coff_State_Trigger");
                        break;
                    case 1:
                        animator.SetTrigger("Cold_State_Trigger");
                        break;
                    case 2:
                        animator.SetTrigger("Hot_State_Trigger");
                        break;
                    case 3:
                        animator.SetTrigger("Itchy_State_Trigger");
                        break;
                    case 4:
                        animator.SetTrigger("Look_Around_Trigger");
                        break;
                    case 5:
                        animator.SetTrigger("Look_Hand_Sky_Trigger");
                        break;
                    case 6:
                        animator.SetTrigger("Search_Floor_Trigger");
                        break;
                    case 7:
                        animator.SetTrigger("Tired_State_Trigger");
                        break;
                    case 8:
                        animator.SetTrigger("Turn_Back_Complain_Trigger");
                        break;
                    default:
                        #if UNITY_EDITOR
                        Debug.LogError("randomStateValue Variable Out of Range");
                        #endif
                        break;
                }
            }
            else if (eventsInARow > 3)
            {
                int randomStateForEndlessIdles = (Random.Range(0, 3));
                eventsInARow = 0;
                switch (randomStateForEndlessIdles)
                {
                    case 0:
                        animator.SetTrigger("Sit_State_In_Trigger");
                        break;
                    case 1:
                        animator.SetTrigger("Hold_Arms_In_Trigger");
                        break;
                    case 2:
                        animator.SetTrigger("Sleep_In_Trigger");
                        break;
                    default:
                        #if UNITY_EDITOR
                        Debug.LogError("randomStateValue[2] Variable Out of Range");
                        #endif
                        break;
                }
            }
            yield break;
        }
        yield return null;
    }

    void GroundedOnlyAnimatorTriggers()
    {
        if (controller.isGrounded && IsInteractionInputsEnable() && IsAnimationInterruptible() && !IsJumpAnimationRunning())
        {           
            SetJumpAnimatorTrigger();
            SetAttackAnimatorTrigger();
            SetDefenseAnimatorTrigger();
            SetSpecialAnimatorTrigger();
        }
    }

    void PreventMovementByAnimation()
    {
        if (VikingControlsScript.IsTryingToMove && !IsAnimationInterruptible())
        {    
            VikingControlsScript.FreezePlayerMovement(true);
        }
        else if (IsInteractionInputsEnable())
        {
            VikingControlsScript.FreezePlayerMovement(false);
        }
    }

    void SetIdlesEventsAnimatorStates()
    {
        if (IsIdleAnimationRunning() && runAnimationOnce == true)
        {
            StartCoroutine(SetRandomIdleEventTrigger());
            runAnimationOnce = false;
        }
        else if (!IsIdleAnimationRunning())
        {
            runAnimationOnce = true;
        }
		
        SetDeepSleepAnimatorTrigger(30f);
    }

    void SetDamageAnimatorTriggers()
    {
        if (AnyDamageScript.DamageEnter == true && stateInfo.fullPathHash != Damage_Front_State)
        {
            animator.SetTrigger("Damage_F");
            AnyDamageScript.DamageEnter = false;
        }

        if (AnyDamageScript.DamageExit == true && stateInfo.fullPathHash != Damage_Back_State)
        {
            animator.SetTrigger("Damage_B");
            AnyDamageScript.DamageExit = false;
        }
    }

    bool IsJumpAnimationRunning()
    {
        if (stateInfo.fullPathHash == jump_Start || stateInfo.fullPathHash == jump_Boost
            || stateInfo.fullPathHash == jump_Fall || stateInfo.fullPathHash == jump_Falling)
            return true;
        else
            return false;
    }

    public bool IsJumpInBoostingAnimation()
    {
        if (stateInfo.fullPathHash == jump_Boost)
            return true;
        else
            return false;
    }

    bool IsWalkingAnimationRunning()
    {
        if (stateInfo.fullPathHash == Run_Start_State || stateInfo.fullPathHash == Run_Loop_State)
            return true;
        else
            return false;
    }

    bool IsShipAnimationRunning()
    {
        if (stateInfo.fullPathHash == shipInState && stateInfo.fullPathHash == shipOutStartState && stateInfo.fullPathHash == shipFallLoopState
            && stateInfo.fullPathHash == shipFallEndState)
            return true;
        else
            return false;
    }

    bool IsAnimationInterruptible()
    {
        if (IsIdleAnimationRunning() || IsWalkingAnimationRunning() || IsJumpAnimationRunning())
            return true;
        else
            return false;
    }

    bool IsInteractionInputsEnable()
    {
        if (ShipBehaviourScript.isCarryingPlayerToShip)
            return false;
        else
            return true;
    }

    bool IsIdleAnimationRunning()
    {
        if (stateInfo.fullPathHash == NeoIdle_State)
            return true;
        else
            return false;
    }

    void SetWalkAnimatorBool()
    {
        if (VikingControlsScript.IsTryingToMove)
            animator.SetBool("TryingToMove", true);
        else
            animator.SetBool("TryingToMove", false);
    }

    void SetShipAnimatorTrigger()
    {
        if (ShipInsideColliderScript.isPlayerInsideShip)
        {
            if (VikingControlsScript.JumpButton && !IsShipAnimationRunning())
            {
                animator.SetTrigger("ShipOut");
                launchPlayerFromShip = true;
            }
        }

        if (ShipBehaviourScript.isCarryingPlayerToShip && animPlayOnce == true)
        {
            animator.SetTrigger("ShipIn");
            animPlayOnce = false;
            animPlayOnce2 = true;
        }

        if (ShipLandingColliderScript.isPlayerInsideLandingCollider && animPlayOnce2 == true)
        {
            animator.SetTrigger("GroundVLand");
            animPlayOnce2 = false;
            animPlayOnce = true;
        }
    }

    void SetAttackAnimatorBool()
    {
        if (VikingControlsScript.AttackButton)
        {
            animator.SetBool("AtkButtonDown", true);
        }
        else if (!VikingControlsScript.AttackButton)
        {
            animator.SetBool("AtkButtonDown", false);
        }
    }

    void SetAttackAnimatorTrigger()
    {
        if (VikingControlsScript.AttackButton && stateInfo.fullPathHash != Atk1_State)
        {
            animator.SetTrigger("Atk1");
            isAttacking = true;
        }
        else if (stateInfo.fullPathHash != Atk1_State)
        {
            isAttacking = false;
        }
    }

    void SetDefenseAnimatorBool()
    {
        if (VikingControlsScript.DefenseButton)
        {
            animator.SetBool("DefButtonDown", true);
        }
        else if (!VikingControlsScript.DefenseButton)
        {
            animator.SetBool("DefButtonDown", false);
        }
    }

    void SetJumpAnimatorTrigger()
    {
        if (VikingControlsScript.JumpButton)
        {   
            animator.SetTrigger("Jump");
        }
    }

    void SetDefenseAnimatorTrigger()
    {
        if (VikingControlsScript.DefenseButton && stateInfo.fullPathHash != Def1_State)
        {
            animator.SetTrigger("Def1");
        }
    }

    void SetKnockbacksAnimatorTriggers()
    {
        if (Input.GetButtonUp("Q") && !IsDefenseAnimationRunning())
        {
            animator.SetTrigger("Damage_F");
        } 
        
        if (Input.GetButtonUp("Q") && IsDefenseAnimationRunning())
        {
            animator.SetTrigger("Damage_KB");
        }
    }

    void SetSpecialAnimatorTrigger()
    {
        if (VikingControlsScript.SpecialAttackButton && stateInfo.fullPathHash != Skalagrim_throw_State)
        {
            animator.SetTrigger("Skalla_Special");
        }
    }

    bool IsDefenseAnimationRunning()
    {
        if (stateInfo.fullPathHash == Def_axeShield_State || stateInfo.fullPathHash == Def_shieldStand_State)
            return true;
        else
            return false;
    }

    void SetDeepSleepAnimatorTrigger(float secondsToEnterDeepSleeping)
    {
        if (stateInfo.fullPathHash == SleepLoop_State)
        {
            secondsPastWhileSleeping += 1 * Time.deltaTime;
            if (secondsPastWhileSleeping > secondsToEnterDeepSleeping)
            {
                animator.SetTrigger("Deep_Sleep_In_Trigger");
                secondsPastWhileSleeping = 0f;
            }
        }
        else
            secondsPastWhileSleeping = 0f;
    }

    void SetFallingAnimatorBool()
    {
        if (!controller.isGrounded && IsJumpAnimationRunning())
        {
            animator.SetBool("Grounded", false);
        }
        if (controller.isGrounded)
        {
            animator.SetBool("Grounded", true);
        }
        if (!controller.isGrounded && !IsJumpAnimationRunning())
        {
            animator.SetBool("Falling", true);
        }
        else
            animator.SetBool("Falling", false);
    }

    public bool IsAttacking
    {
        get
        {
            return isAttacking;
        }
    }
}