using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class PlayerSetup : NetworkBehaviour
{
    public Behaviour[] componentsToDisable;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(WaitToRegister());
    }

    IEnumerator WaitToRegister()
    {
        yield return new WaitUntil(CheckSpawns);
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager _player = GetComponent<PlayerManager>();
        PlayerWrangler.RegisterPlayer(_netID, _player);
    }

    bool CheckSpawns()
    {
        return NetworkServer.SpawnObjects();
    }
}