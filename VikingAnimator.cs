using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class VikingAnimator : MonoBehaviour
{

    private Animator anim;
    public VikingControls VikingControlsScript;
    public ShipBehaviour ShipBehaviourScript;
    public ShipInsideCollider ShipInsideColliderScript;
    public ShipLandingCollider ShipOutLandScript;

    public AnyDamage AnyDamageScript;

    int jumpHash = Animator.StringToHash("Base Layer.Jump");

    public float animSpeed = 1f;
    public float speed;
    public bool jumpBoost = false;
    public bool jumping = false;

    public bool launchPlayerFromShip;
    public bool onAnim;

    bool animPlayOnce = true;
    bool animPlayOnce2 = false;

    private AnimatorStateInfo currentBaseState;
    private AnimatorStateInfo layer2CurrentState;
	
    static int Idle_SState = Animator.StringToHash("Base Layer.Idle_Start");
    static int Idle_LState = Animator.StringToHash("Base Layer.Idle_Loop");
    static int Idle_OState = Animator.StringToHash("Base Layer.Idle_Out");
    //----------------------------------------------------------------------
    static int jump_State = Animator.StringToHash("Jump");
    static int jump_Start = Animator.StringToHash("Base Layer.Jump_Start");
    static int jump_End = Animator.StringToHash("Base Layer.Jump_End");
    static int jump_Boost = Animator.StringToHash("Base Layer.Jump_Boost");
    static int jump_Falling = Animator.StringToHash("Base Layer.Jump_Falling");
    static int jump_Fall = Animator.StringToHash("Base Layer.Jump_Fall");
    //----------------------------------------------------------------------
    static int shipInState = Animator.StringToHash("Base Layer.Ship_In");
    static int shipOutStartState = Animator.StringToHash("Base Layer.Ship_Out_Start");
    static int shipFallLoopState = Animator.StringToHash("Base Layer.Ship_Fall_Loop");
    static int shipFallEndState = Animator.StringToHash("Base Layer.Ship_Fall_End");
    //----------------------------------------------------------------------
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

    public bool OnAtk = false;
	
    public bool runconce = true;
    public bool dontmove;
    public int idleStateCounter = 0;
    public float time;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim.layerCount == 2)
            anim.SetLayerWeight(1, 1);
    }

    void FixedUpdate()
    {	
        ShipInteractions();

        CharacterController controller = GetComponent<CharacterController>();

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        #if MOBILE_INPUT
        if ((CrossPlatformInputManager.GetAxis("Horizontal_ThirdPerson") != 0 || CrossPlatformInputManager.GetAxis("Vertical_ThirdPerson") != 0) && dontmove == true)
        {
            VikingControlsScript.FreezePlayerMovement(true);
        }
        else if (!ShipBehaviourScript.isCarryingPlayerToShip)
        {
            VikingControlsScript.FreezePlayerMovement(false);
        }
        #else
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && dontmove == true)
        {       //[New animations viking1.3]
            VikingControlsScript.FreezePlayerMovement(true);
        }
        else if (!ShipBehaviourScript.isCarryingPlayerToShip)
        {
            VikingControlsScript.FreezePlayerMovement(false);
        }
        #endif

        if (!VikingControlsScript.IsTryingToMove)
        {
            anim.SetBool("TryingToMoving", false);
        }
        else
            anim.SetBool("TryingToMoving", true);

        #if MOBILE_INPUT

        if (controller.isGrounded && !ShipBehaviourScript.isCarryingPlayerToShip)
        {
            if (CrossPlatformInputManager.GetButton("Jump") && dontmove == false && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_End && stateInfo.fullPathHash != jump_Boost
                && stateInfo.fullPathHash != jump_Fall && stateInfo.fullPathHash != jump_Falling)
            {
                anim.SetTrigger("Jump");

            }
            //[New animations viking1.4]
            if (CrossPlatformInputManager.GetButton("Fire1") && dontmove == false && stateInfo.fullPathHash != Atk1_State && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_Boost)
            {
                anim.SetTrigger("Atk1");
                OnAtk = true;
            }
            else if (stateInfo.fullPathHash != Atk1_State)
            {
                OnAtk = false;
            }
            
            if (CrossPlatformInputManager.GetButton("Fire2") && dontmove == false && stateInfo.fullPathHash != Def1_State && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_Boost)
            {
                anim.SetTrigger("Def1");
            }

        }

        #else
        if (controller.isGrounded && !ShipBehaviourScript.isCarryingPlayerToShip)
        {
            if (Input.GetButton("Jump") && dontmove == false && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_End && stateInfo.fullPathHash != jump_Boost
                && stateInfo.fullPathHash != jump_Fall && stateInfo.fullPathHash != jump_Falling)
            {	
                anim.SetTrigger("Jump");
            }
            //[New animations viking1.4]
            if (Input.GetButton("Fire1") && dontmove == false && stateInfo.fullPathHash != Atk1_State && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_Boost)
            {
                anim.SetTrigger("Atk1");
                OnAtk = true;
            }
            else if (stateInfo.fullPathHash != Atk1_State)
            {
                OnAtk = false;
            }
			
            if (Input.GetButton("Fire2") && dontmove == false && stateInfo.fullPathHash != Def1_State && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_Boost)
            {
                anim.SetTrigger("Def1");
            }
        }
        #endif
        if (stateInfo.fullPathHash == jump_Boost)
        {
            jumpBoost = true;
        }
        else
            jumpBoost = false;

        if (stateInfo.fullPathHash == jump_Boost || stateInfo.fullPathHash == jump_Falling || stateInfo.fullPathHash == jump_Fall)
        {
            jumping = true;
        }
        else
            jumping = false;

        if (!controller.isGrounded && jumping == true)
        {
            anim.SetBool("Grounded", false);
        }
        else
            anim.SetBool("Grounded", true);

        #if MOBILE_INPUT

        if (ShipInsideColliderScript.isPlayerInsideShip)
        {
            if (CrossPlatformInputManager.GetButton("Jump") && stateInfo.fullPathHash != shipOutStartState && stateInfo.fullPathHash != shipFallLoopState
                && stateInfo.fullPathHash != shipFallEndState)
            {
                anim.SetTrigger("ShipOut");
                launchPlayerFromShip = true;
            }
        }

        if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            anim.SetBool("AtkButtonDown", true);
        }
        else if (!CrossPlatformInputManager.GetButton("Fire1"))
        {
            anim.SetBool("AtkButtonDown", false);
        }
            
        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            anim.SetBool("DefButtonDown", true);
        }
        else if (!CrossPlatformInputManager.GetButton("Fire2"))
        {
            anim.SetBool("DefButtonDown", false);
        }

        if (CrossPlatformInputManager.GetButton("E") && dontmove == false && stateInfo.fullPathHash != Skalagrim_throw_State && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_Boost
            && stateInfo.fullPathHash != jump_Fall && stateInfo.fullPathHash != jump_Falling)
        {
            anim.SetTrigger("Skalla_Special");
        }

        #else
		
        if (ShipInsideColliderScript.isPlayerInsideShip)
        {
            if (Input.GetButton("Jump") && stateInfo.fullPathHash != shipOutStartState && stateInfo.fullPathHash != shipFallLoopState
                && stateInfo.fullPathHash != shipFallEndState)
            {
                anim.SetTrigger("ShipOut");
                launchPlayerFromShip = true;
            }
        }

        //[New animations viking1.4]
        if (Input.GetButton("Fire1"))
        {
            anim.SetBool("AtkButtonDown", true);
        }
        else if (!Input.GetButton("Fire1"))
        {
            anim.SetBool("AtkButtonDown", false);
        }
            
        if (Input.GetButton("Fire2"))
        {
            anim.SetBool("DefButtonDown", true);
        }
        else if (!Input.GetButton("Fire2"))
        {
            anim.SetBool("DefButtonDown", false);
        }
        //[New animations viking1.4] //precisa colocar os idles das respectivas defesas só assim vai fazer sentido os knockbacks
        if (Input.GetButtonUp("Q") && stateInfo.fullPathHash != Def_axeShield_State && stateInfo.fullPathHash != Def_shieldStand_State)
        {
            anim.SetTrigger("Damage_F");
        } 
        
        if (Input.GetButtonUp("Q") && (stateInfo.fullPathHash == Def_axeShield_State || stateInfo.fullPathHash == Def_shieldStand_State))
        {
            anim.SetTrigger("Damage_KB");
        }
        
        if (Input.GetButtonUp("E") && dontmove == false && stateInfo.fullPathHash != Skalagrim_throw_State && stateInfo.fullPathHash != jump_Start && stateInfo.fullPathHash != jump_Boost
            && stateInfo.fullPathHash != jump_Fall && stateInfo.fullPathHash != jump_Falling)
        {
            anim.SetTrigger("Skalla_Special");
        }

        #endif        		

        DamageReceiver();		
        IdleStatesController();
        AnimationMotionCrontrol();
    }

    void ShipInteractions()
    {
        if (ShipBehaviourScript.isCarryingPlayerToShip && animPlayOnce == true)
        {
            anim.SetTrigger("ShipIn");
            animPlayOnce = false;
            animPlayOnce2 = true;
        }

        if (ShipOutLandScript.isPlayerInsideLandingCollider && animPlayOnce2 == true)
        {
            anim.SetTrigger("GroundVLand");
            animPlayOnce2 = false;
            animPlayOnce = true;
        }
    }

    IEnumerator IdleRandomStates()
    { 	
        yield return new WaitForSeconds(30f);
        while (!VikingControlsScript.IsTryingToMove)
        {
            if (idleStateCounter <= 3)
            {
                int randomStateValue = (Random.Range(0, 9));
                idleStateCounter = idleStateCounter + 1;
                switch (randomStateValue)
                {
                    case 0:
                        anim.SetTrigger("Coff_State_Trigger");
                        break;
                    case 1:
                        anim.SetTrigger("Cold_State_Trigger");
                        break;
                    case 2:
                        anim.SetTrigger("Hot_State_Trigger");
                        break;
                    case 3:
                        anim.SetTrigger("Itchy_State_Trigger");
                        break;
                    case 4:
                        anim.SetTrigger("Look_Around_Trigger");
                        break;
                    case 5:
                        anim.SetTrigger("Look_Hand_Sky_Trigger");
                        break;
                    case 6:
                        anim.SetTrigger("Search_Floor_Trigger");
                        break;
                    case 7:
                        anim.SetTrigger("Tired_State_Trigger");
                        break;
                    case 8:
                        anim.SetTrigger("Turn_Back_Complain_Trigger");
                        break;
                    default:
                        Debug.LogError("randomStateValue Variable Out of Range");
                        break;
                }
            }
            else if (idleStateCounter > 3)
            {
                int randomStateValueSecond = (Random.Range(0, 3));
                idleStateCounter = 0;
                switch (randomStateValueSecond)
                {
                    case 0:
                        anim.SetTrigger("Sit_State_In_Trigger");
                        break;
                    case 1:
                        anim.SetTrigger("Hold_Arms_In_Trigger");
                        break;
                    case 2:
                        anim.SetTrigger("Sleep_In_Trigger");
                        break;
                    default:
                        Debug.LogError("randomStateValue[2] Variable Out of Range");
                        break;
                }
            }
            yield break;
        }
        yield return null;
    }

    void IdleStatesController()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.fullPathHash == NeoIdle_State && runconce == true)
        {
            StartCoroutine(IdleRandomStates());
            runconce = false;
        }
        else if (stateInfo.fullPathHash != NeoIdle_State)
        {
            runconce = true;
        }
		
        if (stateInfo.fullPathHash == SleepLoop_State)
        {
            time = time + 1 * Time.deltaTime;
            if (time > 30f)
            {
                anim.SetTrigger("Deep_Sleep_In_Trigger");
                time = 0f;
            }
        }
        else
            time = 0f;
    }

    void AnimationMotionCrontrol()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		
        if (stateInfo.fullPathHash == NeoIdle_State || stateInfo.fullPathHash == Run_Start_State || stateInfo.fullPathHash == Run_Loop_State ||
            stateInfo.fullPathHash == jump_Fall || stateInfo.fullPathHash == jump_Falling || stateInfo.fullPathHash == jump_Boost ||
            stateInfo.fullPathHash == jump_Start)
        {
            dontmove = false;
        }
        else
            dontmove = true; 
    }

    void DamageReceiver()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (AnyDamageScript.DamageEnter == true && stateInfo.fullPathHash != Damage_Front_State)
        {
            anim.SetTrigger("Damage_F");
            AnyDamageScript.DamageEnter = false;
        }

        if (AnyDamageScript.DamageExit == true && stateInfo.fullPathHash != Damage_Back_State)
        {
            anim.SetTrigger("Damage_B");
            AnyDamageScript.DamageExit = false;
        }
    }
}