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

    float horizontal;
    float vertical;
    bool isPunching;
    [HideInInspector]
    public bool isPlayerOne;
    bool dealingDamage;

    int playerNum;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        movement = GetComponent<Movement>();
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

        if (inputDevice.RightTrigger)
            Ability1();

        if(canMove)
        Move();

        if (inputDevice.Action1)
            ChargeUp();
        else
            StopChargeUp();

        if (inputDevice.LeftBumper)
            Block();
        else
            StopBlock();
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
        animationManager.CmdCastAbility1();
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
        animationManager.ChargingUp();
    }

    void StopChargeUp()
    {
        animationManager.StopCharging();
    }

    void Block()
    {
        animationManager.IsBlocking();
    }

    void StopBlock()
    {
        animationManager.StoppedBlocking();
    }
}
