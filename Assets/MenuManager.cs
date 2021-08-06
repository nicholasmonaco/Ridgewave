using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using TMPro;
using System;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Camera _menuCamera;
    [SerializeField] private Transform _gameUI;

    [SerializeField] private TMP_InputField _ipJoinField;
    [SerializeField] private TMP_InputField _portJoinField;

    [SerializeField] private TMP_InputField _passwordHostField;
    [SerializeField] private TMP_InputField _portHostField;


    [SerializeField] private Transform _mainPanel;
    [SerializeField] private Transform _joinPanel;
    [SerializeField] private Transform _hostPanel;
    [SerializeField] private Transform _optionsPanel;

    [SerializeField] private Transform _characterPanel;

    private Transform _curPanel;


    private void Start() {
        _curPanel = _mainPanel;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }


    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
        bool approve = true;


        callback(true, null, approve, new Vector3(0, 5, 0), Quaternion.identity);
    }


    private void OnPlay() {
        Game.Manager.InGame = true;
        _gameUI.gameObject.SetActive(true);

        this.gameObject.SetActive(false);
    }

    public void BackToMainMenu() {
        Game.Manager.InGame = false;

        _gameUI.gameObject.SetActive(false);
        this.gameObject.SetActive(true);

        _mainPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _mainPanel;
    }

    public void OnHost() {
        _hostPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _hostPanel;
    }

    public void OnJoin() {
        _joinPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _joinPanel;
    }

    public void OnOptions() {
        _optionsPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _optionsPanel;
    }

    public void OnBackToMain() {
        _mainPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _mainPanel;
    }

    public void OnBackToMain_Options() {
        Options.SaveSettings();

        _mainPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _mainPanel;
    }


    public void StartHost() {
        int port;
        try { 
            if(_portHostField.text.Trim() == "") {
                port = 7777;
            } else {
                port = Convert.ToInt32(_portHostField.text);
            }
        } catch { port = 7777; }

        NetworkManager.Singleton.GetComponent<UNetTransport>().ServerListenPort = port;
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = port;

        OnPlay();
        NetworkManager.Singleton.StartHost(); 
    }

    public void OnConnect_Join() {
        string ip = _ipJoinField.text.Trim();
        if (ip == "") {
            ip = "127.0.0.1";
        }

        int port;
        try { 
            if(_portHostField.text.Trim() == "") {
                port = 7777;
            } else {
                port = Convert.ToInt32(_portJoinField.text);
            }
        } catch { port = 7777; }

        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ip;
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = port;

        OnPlay();
        NetworkManager.Singleton.StartClient();
    }


    








    public void ExitGame() {
        Application.Quit();
    }
}
