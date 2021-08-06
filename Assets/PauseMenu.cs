using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Transform _menu;
    [SerializeField] private MenuManager _mainMenu;

    [SerializeField] private Transform _mainPanel;
    [SerializeField] private Transform _optionsPanel;

    private Transform _curPanel;

    [HideInInspector] public bool Paused = false;
    private bool PausedDown = false;


    private void OnEnable() {
        ResetPauseMenu();
    }

    private void OnDisable() {
        ResetPauseMenu();
    }


    private void Update() {
        if (Game.Input.Player.Paused.ReadValue<float>() > 0) {
            if (!PausedDown) {
                Paused = !Paused;
                PausedDown = true;
                UpdatePauseMenu();
            }
        } else {
            PausedDown = false;
        }
    }

    public void ResetPauseMenu() {
        PausedDown = false;
        Paused = false;

        if (_curPanel == null) _curPanel = _mainPanel;

        UpdatePauseMenu();
    }

    public void UpdatePauseMenu() {
        if (_curPanel != _mainPanel) {
            Options.SaveSettings();

            _curPanel.gameObject.SetActive(false);
            _mainPanel.gameObject.SetActive(true);
            _curPanel = _mainPanel;
        }

        if(Game.Manager.InGame) Cursor.lockState = Paused ? CursorLockMode.None : CursorLockMode.Locked;

        _menu.gameObject.SetActive(Paused);
    }



    public void OnOptions() {
        _optionsPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _optionsPanel;
    }

    public void OnResume() {
        Paused = false;
        PausedDown = false;

        UpdatePauseMenu();
    }

    public void OnMainMenu() {
        Game.Manager.InGame = false;

        if (NetworkManager.Singleton.IsServer) {
            NetworkManager.Singleton.StopServer();
        } else if (NetworkManager.Singleton.IsHost) {
            NetworkManager.Singleton.StopHost();
        } else {
            NetworkManager.Singleton.StopClient();
        }

        _mainMenu.BackToMainMenu();
    }

    public void OnOptions_Back() {
        Options.SaveSettings();

        _mainPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _mainPanel;
    }
}
