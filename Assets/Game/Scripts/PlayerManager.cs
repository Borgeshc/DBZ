using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using InControl;

public class PlayerManager : NetworkBehaviour
{
    public bool canMove;
    
    public bool isDead;

    public float damage;
    GameManager gameManager;
    [HideInInspector]
    public PlayerManager enemy;

    AnimationManager animationManager;
    Movement movement;
    InputDevice inputDevice;
    Stamina stamina;
    SoundManager soundManager;

    float horizontal;
    float vertical;
    bool isPunching;
    [HideInInspector]
    public bool isPlayerOne;
    bool dealingDamage;
    [HideInInspector]
    public bool isBusy;
    bool gainingStamina;
    [HideInInspector]
    public bool blocking;
    Coroutine charge;
    Coroutine block;

    bool isCharging;
    [HideInInspector]
    public bool isUsingAbility;
 
    int playerNum;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        soundManager = GetComponent<SoundManager>();
        movement = GetComponent<Movement>();
        stamina = GetComponent<Stamina>();
        canMove = true;
    }
    
    public void SetUp(bool _isPlayerOne)
    {
        isPlayerOne = _isPlayerOne;

        if(isPlayerOne == false)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        GetComponent<Health>().SetHealthBars();
        stamina.SetStaminaBars();
     //   Debug.LogError("Player Set Up called" +  transform.name + "IsPlayerOne is : " + isPlayerOne);
    }

    private void Update()
    {
        inputDevice = InputManager.ActiveDevice;

        horizontal = inputDevice.LeftStickX;
        vertical = inputDevice.LeftStickY;

        if (inputDevice.Action3)
            Punch();
        else
            StopPunching();

        if (inputDevice.Action4)
            Kick();
        else
            StopKicking();

        if (inputDevice.RightTrigger && !isBusy && !isUsingAbility && !blocking && !isCharging)
            Ability1();

        if (inputDevice.LeftTrigger && !isBusy && !isUsingAbility && !blocking && !isCharging)
            Ability2();

        if (canMove && !blocking && !isCharging && !isUsingAbility)
        Move();

        if (inputDevice.Action1 && !isBusy && !isUsingAbility)
            ChargeUp();
        else
            StopChargeUp();

        if (inputDevice.LeftBumper && !isBusy && !isUsingAbility)
            Block();
        else
            StopBlock();

        if (inputDevice.Action2 && !isBusy && !isUsingAbility)
            CastKiBlast();

        if (inputDevice.RightStick.X < 0)
        {
            animationManager.inverted = true;
            CmdTurn("Left");
        }
        else if(inputDevice.RightStick.X > 0)
        {
            animationManager.inverted = false;
            CmdTurn("Right");
        }
    }

    void Move()
    {
        movement.Move(horizontal, vertical);
        animationManager.IsMoving(horizontal);
    }

    void Punch()
    {
        animationManager.IsPunching();
        soundManager.CmdMelee("Punch");
    }

    void StopPunching()
    {
        animationManager.StoppedPunching();
    }

    void Kick()
    {
        animationManager.IsKicking();
        soundManager.CmdMelee("Kick");
    }

    void StopKicking()
    {
        animationManager.StoppedKicking();
    }

    public void Hit()
    {
        stamina.CmdGainStamina(3);
        animationManager.IsHit();
    }

    void Ability1()
    {
        isBusy = true;
        isUsingAbility = true;
        if (stamina.stamina >= 50)
        {
            if (charge != null)
                StopCoroutine(charge);

            if (block != null)
                StopCoroutine(block);

            stamina.CmdConsumeStamina(50);
            animationManager.CmdCastAbility1();
        }
        else
        {
            isUsingAbility = false;
            isBusy = false;
        }
    }


    void Ability2()
    {
        isBusy = true;
        isUsingAbility = true;
        if (stamina.stamina >= 75)
        {
            if (charge != null)
                StopCoroutine(charge);

            if (block != null)
                StopCoroutine(block);

            stamina.CmdConsumeStamina(75);
            animationManager.CmdCastAbility2();
        }
        else
        {
            isUsingAbility = false;
            isBusy = false;
        }
    }

    public void Dead()
    {
        isDead = true;
        StartCoroutine(animationManager.IsDead());
    }

    public void TookDamage(float damage)
    {
        GetComponent<Health>().CmdTookDamage(damage);
    }

    public void DealDamage()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + transform.right, new Vector2(.3f, .3f), 0,transform.right, .3f);
       // Debug.LogError(hit.transform);
        if(hit.transform != null && hit.transform.tag == "Player" && hit.transform.gameObject != gameObject)
        {
            if(!dealingDamage)
            {
                dealingDamage = true;
                stamina.CmdGainStamina(5);
                PlayerWrangler.GetPlayer(hit.transform.name).TookDamage(damage);
                StartCoroutine(Wait());
            }
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(.5f);
        dealingDamage = false;
    }

    void ChargeUp()
    {
        soundManager.CmdSoundEffect("Charge");
        isBusy = true;
        if(!gainingStamina)
        {
            gainingStamina = true;
            charge = StartCoroutine(GainStamina());
        }
        animationManager.ChargingUp();
    }

    void StopChargeUp()
    {
        isBusy = false;
        gainingStamina = false;
        if(charge != null)
        StopCoroutine(charge);
        animationManager.StopCharging();
    }

    void Block()
    {
        isBusy = true;
        if (!blocking && stamina.stamina > 5)
        {
            blocking = true;
            block = StartCoroutine(LoseStamina());
            animationManager.IsBlocking();
        }
    }

    void StopBlock()
    {
        isBusy = false;
        blocking = false;
        if (block != null)
            StopCoroutine(block);
        animationManager.StoppedBlocking();
    }

    IEnumerator GainStamina()
    {
        stamina.CmdGainStamina(3);
        yield return new WaitForSeconds(.1f);
        gainingStamina = false;
    }


    IEnumerator LoseStamina()
    {
        stamina.CmdConsumeStamina(5);

        if (stamina.stamina < 5)
            StopBlock();
        yield return new WaitForSeconds(.5f);
        blocking = false;
    }

    [Command]
    void CmdTurn(string direction)
    {
        RpcTurn(direction);
    }

    [ClientRpc]
    void RpcTurn(string direction)
    {
        if (direction == "Left")
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.identity;
    }

    void CastKiBlast()
    {
        soundManager.CmdSoundEffect("KiBlast");
        animationManager.CastingKiBlast();
    }
}
