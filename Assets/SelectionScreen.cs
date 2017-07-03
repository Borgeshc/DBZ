using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour
{
    public static GameObject playerPrefab;
    public Prototype.NetworkLobby.LobbyManager lobbyManager;

    public void SelectedFighter(GameObject fighterPrefab)
    {
        playerPrefab = fighterPrefab;
        lobbyManager.gameObject.SetActive(true);
    }
}
