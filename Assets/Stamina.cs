using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Stamina : NetworkBehaviour
{
    public float maxStamina;
    [HideInInspector]
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

    public void GainStamina(float gainAmount)
    {
        if (stamina + gainAmount < maxStamina)
            stamina += gainAmount;
        else
            stamina = maxStamina;

        CmdUpdateStaminaBar();
    }

    public void ConsumeStamina(float consumedAmount)
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
