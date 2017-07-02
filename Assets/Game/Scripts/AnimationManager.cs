using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public GameObject ability1Cast;
    public GameObject ability1;
    public float ability1Cooldown;
    bool ability1OnCooldown;

    Animator anim;
    int punchCombo;
    int kickCombo;

    Coroutine ability1Coroutine;
    PlayerManager playerManager;
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        ability1.GetComponent<Projectile>().SetPlayer(playerManager.isPlayerOne);
    }

    public void IsMoving(float horizontal)
    {
        if (PlayerManager.isDead) return;

        if(playerManager.isPlayerOne)
            anim.SetFloat("Horizontal", horizontal);
        else
            anim.SetFloat("Horizontal", -horizontal);
    }

    public void IsPunching()
    {
        if (PlayerManager.isDead) return;

        anim.SetBool("IsPunching", true);
        punchCombo++;
        if (punchCombo > 5)
            punchCombo = 1;

        anim.SetInteger("PunchCombo", punchCombo);
    }

    public void StoppedPunching()
    {
        if (PlayerManager.isDead) return;

        if (anim.GetBool("IsPunching").Equals(true))
        {
            anim.SetBool("IsPunching", false);
        }
    }

    public void IsKicking()
    {
        if (PlayerManager.isDead) return;

        anim.SetBool("IsKicking", true);
        kickCombo++;
        if (kickCombo > 5)
            kickCombo = 1;

        anim.SetInteger("KickCombo", kickCombo);
    }


    public void StoppedKicking()
    {
        if (PlayerManager.isDead) return;

        if (anim.GetBool("IsKicking").Equals(true))
        {
            anim.SetBool("IsKicking", false);
        }
    }

    public void CastAbility1()
    {
        ability1Coroutine = StartCoroutine(Ability1Cast());
    }

    public IEnumerator Ability1Cast()
    {
        if (PlayerManager.isDead) StopCoroutine(ability1Coroutine);

        if (!ability1OnCooldown)
        {
            PlayerManager.canMove = false;
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
            ability1Cast.SetActive(false);

            anim.SetTrigger("Ability1Done");
            PlayerManager.canMove = true;

            StartCoroutine(Ability1Cooldown());
        }
    }

    IEnumerator Ability1Cooldown()
    {
        yield return new WaitForSeconds(ability1Cooldown);
        ability1OnCooldown = false;
    }

    public void IsDead()
    {
        PlayerManager.canMove = false;
        GetComponent<Rigidbody2D>().gravityScale = 2;
        anim.SetBool("IsDead", true);
    }
}
