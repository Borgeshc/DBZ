using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Stamina : NetworkBehaviour
{
    public float maxStamina;
    [HideInInspector, SyncVar]
    public float stamina;
    Image staminaBar;
    PlayerManager playerManager;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void SetStaminaBars()
    {
        if (playerManager.isPlayerOne)
            staminaBar = GameObject.Find("PlayerOneStaminaBar").GetComponent<Image>();
        else
            staminaBar = GameObject.Find("PlayerTwoStaminaBar").GetComponent<Image>();

    }

    [Command]
    public void CmdGainStamina(float gainAmount)
    {
        RpcGainStamina(gainAmount);
    }

    [ClientRpc]
    public void RpcGainStamina(float gainAmount)
    {
        if (stamina + gainAmount < maxStamina)
            stamina += gainAmount;
        else
            stamina = maxStamina;

        CmdUpdateStaminaBar();
    }

    [Command]
    public void CmdConsumeStamina(float consumedAmount)
    {
        RpcConsumeStamina(consumedAmount);
    }

    [ClientRpc]
    public void RpcConsumeStamina(float consumedAmount)
    {
        if (stamina - consumedAmount > 0)
            stamina -= consumedAmount;
        else
            stamina = 0;

        CmdUpdateStaminaBar();
    }

    void CmdUpdateStaminaBar()
    {
        RpcUpdateStaminaBar();
    }

    void RpcUpdateStaminaBar()
    {
        staminaBar.fillAmount = stamina / maxStamina;
    }
}
