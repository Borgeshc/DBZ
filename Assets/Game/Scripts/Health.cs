﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    public float baseHealth;
    public Image myHealthBar;

    PlayerManager playerManager;

    [SyncVar]
    public float health;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();

        health = baseHealth;
    }

    public void SetHealthBars()
    {
        if (playerManager.isPlayerOne)
            myHealthBar = GameObject.Find("PlayerOneHealthBar").GetComponent<Image>();
        else
            myHealthBar = GameObject.Find("PlayerTwoHealthBar").GetComponent<Image>();
        UpdateHealthBar();
    }

    [Command]
    public void CmdTookDamage(float damage)
    {
        RpcTookDamage(damage);
    }

    [ClientRpc]
    public void RpcTookDamage(float damage)
    {
        health -= damage;
        UpdateHealthBar();

        if (health <= 0 && !PlayerManager.isDead)
            Died();
    }

    void UpdateHealthBar()
    {
        CmdUpdateHealthBar();
    }

    [Command]
    void CmdUpdateHealthBar()
    {
        RpcUpdateHealthBar();
    }

    [ClientRpc]
    void RpcUpdateHealthBar()
    {
        myHealthBar.fillAmount = health / baseHealth;
    }


    void Died()
    {
        playerManager.Dead();
    }
}