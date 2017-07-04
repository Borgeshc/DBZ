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

    public GameObject ability2Cast;
    public GameObject ability2;
    public float ability2Cooldown;
    bool ability2OnCooldown;

    public GameObject kiBlastCast;
    public GameObject kiBlast;

    Animator anim;
    int punchCombo;
    int kickCombo;
    Rigidbody2D rb;

    Coroutine ability1Coroutine;
    Coroutine ability2Coroutine;

    SoundManager soundManager;
    PlayerManager playerManager;
    [HideInInspector]
    public bool inverted;

    bool castingKiBlast;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        soundManager = GetComponent<SoundManager>();
        rb = GetComponent<Rigidbody2D>();
        ability1.GetComponent<Projectile>().SetPlayer(playerManager.isPlayerOne);
    }

    public void IsMoving(float horizontal)
    {
        if (playerManager.isDead) return;

        if(transform.rotation == Quaternion.identity)
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
            soundManager.CmdSoundEffect("AbilityCharge");

            yield return new WaitForSeconds(1);

            anim.SetBool("Ability1Cast", false);
            yield return new WaitForSeconds(.15f);
            ability1Cast.SetActive(true);

            yield return new WaitForSeconds(.1f);

            ability1.transform.localPosition = new Vector3(1, 0,0);

            soundManager.CmdSoundEffect("BeamRelease");
            ability1.SetActive(true);

            yield return new WaitForSeconds(1f);
            anim.SetTrigger("Ability1Done");
            ability1Cast.SetActive(false);

            playerManager.isBusy = false;
            playerManager.isUsingAbility = false;
            playerManager.canMove = true;

            StartCoroutine(Ability1Cooldown());
        }
    }

    IEnumerator Ability1Cooldown()
    {
        yield return new WaitForSeconds(ability1Cooldown);
        ability1OnCooldown = false;
    }


    [Command]
    public void CmdCastAbility2()
    {
        RpcCastAbility2();
    }

    [ClientRpc]
    public void RpcCastAbility2()
    {
        ability1Coroutine = StartCoroutine(Ability2Cast());
    }

    public IEnumerator Ability2Cast()
    {
        if (playerManager.isDead && ability2Coroutine != null) StopCoroutine(ability2Coroutine);

        if (!ability2OnCooldown)
        {
            playerManager.canMove = false;
            rb.velocity = Vector2.zero;

            ability2OnCooldown = true;
            anim.SetBool("Ability2Cast", true);
            yield return new WaitForSeconds(.15f);

            ability2Cast.SetActive(true);
            soundManager.CmdSoundEffect("Bomb");
            yield return new WaitForSeconds(1.5f);

            ability2Cast.SetActive(false);
            anim.SetBool("Ability2Cast", false);
            yield return new WaitForSeconds(.1f);
            ability2.transform.localPosition = new Vector3(.5f, 0, 0);

            ability2.SetActive(true);
            anim.SetTrigger("Ability2Done");

            playerManager.isBusy = false;
            playerManager.isUsingAbility = false;
            playerManager.canMove = true;

            StartCoroutine(Ability2Cooldown());
        }
    }

    IEnumerator Ability2Cooldown()
    {
        yield return new WaitForSeconds(ability2Cooldown);
        ability2OnCooldown = false;
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

    public void ChargingUp()
    {
        playerManager.canMove = false;
        rb.velocity = Vector2.zero;
        anim.SetBool("ChargeUp", true);
    }

    public void StopCharging()
    {
        playerManager.canMove = true;
        anim.SetBool("ChargeUp", false);
    }

    public void IsBlocking()
    {
        playerManager.canMove = false;
        rb.velocity = Vector2.zero;
        anim.SetBool("Block", true);
    }

    public void StoppedBlocking()
    {
        playerManager.canMove = true;
        anim.SetBool("Block", false);
    }

    public void CastingKiBlast()
    {
        if(!castingKiBlast)
        {
            castingKiBlast = true;
            StartCoroutine(CastKiBlast());
        }
    }

    IEnumerator CastKiBlast()
    {
        anim.SetBool("KiBlastCast", true);
        yield return new WaitForSeconds(.15f);
        kiBlast.transform.localPosition = new Vector3(.5f, 0, 0);
        kiBlastCast.SetActive(true);
        kiBlast.SetActive(true);
        yield return new WaitForSeconds(1);
        anim.SetBool("KiBlastCast", false);
        kiBlastCast.SetActive(false);
        castingKiBlast = false;
    }
}
