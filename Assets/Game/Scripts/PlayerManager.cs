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
    Coroutine charge;
 
    int playerNum;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        movement = GetComponent<Movement>();
        stamina = GetComponent<Stamina>();
        canMove = true;

        if (isPlayerOne)
        {
            animationManager.inverted = true;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
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
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if(inputDevice.RightStick.X > 0)
        {
            animationManager.inverted = false;
            transform.rotation = Quaternion.identity;
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
        animationManager.IsHit();
    }

    void Ability1()
    {
        isBusy = true;
        if (stamina.stamina >= 50)
        {
            stamina.ConsumeStamina(50);
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
        StopCoroutine(charge);
        animationManager.StopCharging();
    }

    void Block()
    {
        isBusy = true;
        animationManager.IsBlocking();
    }

    void StopBlock()
    {
        isBusy = false;
        animationManager.StoppedBlocking();
    }

    IEnumerator GainStamina()
    {
        stamina.GainStamina(1);
        yield return new WaitForSeconds(.1f);
        gainingStamina = false;
    }
}
