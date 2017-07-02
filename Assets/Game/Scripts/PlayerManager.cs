using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class PlayerManager : NetworkBehaviour
{
    public static bool canMove;
    
    public static bool isDead;

    public float damage;
    GameManager gameManager;
    [HideInInspector]
    public PlayerManager enemy;

    AnimationManager animationManager;
    Movement movement;

    float horizontal;
    float vertical;
    bool isPunching;
    [HideInInspector]
    public bool isPlayerOne;
    bool dealingDamage;

    int playerNum;

    private void Start()
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
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Punch();
        else
            StopPunching();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            Kick();
        else
            StopKicking();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Ability1();

        if(canMove)
        Move();
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

    void Ability1()
    {
        animationManager.CastAbility1();
    }

    public void Dead()
    {
        isDead = true;
        animationManager.IsDead();
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
}
