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
 
    int playerNum;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
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

        if (inputDevice.RightTrigger && !isBusy)
            Ability1();

        if(canMove)
        Move();

        if (inputDevice.Action1 && !isBusy)
            ChargeUp();
        else
            StopChargeUp();

        if (inputDevice.LeftBumper && !isBusy)
            Block();
        else
            StopBlock();

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
    }

    void StopPunching()
    {
        animationManager.StoppedPunching();
    }

    void Kick()
    {
        animationManager.IsKicking();
    }

    void StopKicking()
    {
        animationManager.StoppedKicking();
    }

    public void Hit()
    {
        stamina.CmdGainStamina(1);
        animationManager.IsHit();
    }

    void Ability1()
    {
        isBusy = true;
        if (stamina.stamina >= 50)
        {
            stamina.CmdConsumeStamina(50);
            animationManager.CmdCastAbility1();
        }
        else
        isBusy = false;
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
        stamina.CmdGainStamina(1);
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
}
