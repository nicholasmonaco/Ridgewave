using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Camera _menuCamera;
    [SerializeField] private Transform _gameUI;


    private void OnPlay() {
        _gameUI.gameObject.SetActive(true);

        this.gameObject.SetActive(false);
    }

    public void OnHost() {
        NetworkManager.Singleton.StartHost(); //lots of options here, do it boiiiiiii
        OnPlay();
    }

    public void OnJoin() {
        NetworkManager.Singleton.StartClient();
        OnPlay();
    }

    public void ExitGame() {
        Application.Quit();
    }
}
