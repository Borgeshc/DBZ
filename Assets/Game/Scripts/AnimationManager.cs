using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnimationManager : NetworkBehaviour
{
    public GameObject ability1Cast;
    public GameObject ability1;
    public float ability1Cooldown;
    bool ability1OnCooldown;

    Animator anim;
    int punchCombo;
    int kickCombo;
    Rigidbody2D rb;

    Coroutine ability1Coroutine;
    PlayerManager playerManager;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        ability1.GetComponent<Projectile>().SetPlayer(playerManager.isPlayerOne);
    }

    public void IsMoving(float horizontal)
    {
        if (playerManager.isDead) return;

        if(playerManager.isPlayerOne)
            anim.SetFloat("Horizontal", horizontal);
        else
            anim.SetFloat("Horizontal", -horizontal);
    }

    public void IsPunching()
    {
        if (playerManager.isDead) return;

        anim.SetBool("IsPunching", true);
        punchCombo++;
        if (punchCombo > 5)
            punchCombo = 1;

        anim.SetInteger("PunchCombo", punchCombo);
    }

    public void StoppedPunching()
    {
        if (playerManager.isDead) return;

        if (anim.GetBool("IsPunching").Equals(true))
        {
            anim.SetBool("IsPunching", false);
        }
    }

    public void IsKicking()
    {
        if (playerManager.isDead) return;

        anim.SetBool("IsKicking", true);
        kickCombo++;
        if (kickCombo > 5)
            kickCombo = 1;

        anim.SetInteger("KickCombo", kickCombo);
    }


    public void StoppedKicking()
    {
        if (playerManager.isDead) return;

        if (anim.GetBool("IsKicking").Equals(true))
        {
            anim.SetBool("IsKicking", false);
        }
    }

    public void IsHit()
    {
        int hitEffect = Random.Range(0, 1);

        if (hitEffect == 0)
            anim.SetTrigger("Hit1");
        else
            anim.SetTrigger("Hit2");
    }

    [Command]
    public void CmdCastAbility1()
    {
        RpcCastAbility1();
    }

    [ClientRpc]
    public void RpcCastAbility1()
    {
        ability1Coroutine = StartCoroutine(Ability1Cast());
    }

    public IEnumerator Ability1Cast()
    {
        if (playerManager.isDead && ability1Coroutine != null) StopCoroutine(ability1Coroutine);

        if (!ability1OnCooldown)
        {
            playerManager.canMove = false;
            rb.velocity = Vector2.zero;

            ability1OnCooldown = true;
            anim.SetBool("Ability1Cast", true);

            yield return new WaitForSeconds(1);

            anim.SetBool("Ability1Cast", false);
            yield return new WaitForSeconds(.15f);
            ability1Cast.SetActive(true);

            yield return new WaitForSeconds(.1f);

            if(playerManager.isPlayerOne)
            ability1.transform.position = new Vector3(transform.position.x + 5, transform.position.y,transform.position.z);
            else
            {
                ability1.transform.rotation = Quaternion.Euler(0,180,0);
                ability1.transform.position = new Vector3(transform.position.x - 5, transform.position.y, transform.position.z);
            }

            ability1.SetActive(true);

            yield return new WaitForSeconds(1f);
            anim.SetTrigger("Ability1Done");
            ability1Cast.SetActive(false);

            playerManager.canMove = true;

            StartCoroutine(Ability1Cooldown());
        }
    }

    IEnumerator Ability1Cooldown()
    {
        yield return new WaitForSeconds(ability1Cooldown);
        ability1OnCooldown = false;
    }

    public IEnumerator IsDead()
    {
        playerManager.canMove = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1.5f;
        anim.SetBool("IsDead", true);
        yield return new WaitForSeconds(1);
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
