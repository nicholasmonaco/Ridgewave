using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using TMPro;
using System;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Camera _menuCamera;
    [SerializeField] private Transform _gameUI;
    [SerializeField] private TMP_InputField _ipField;
    [SerializeField] private TMP_InputField _portField;

    [SerializeField] private Transform _mainPanel;
    [SerializeField] private Transform _connectPanel;

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
        _gameUI.gameObject.SetActive(true);

        this.gameObject.SetActive(false);
    }

    public void OnHost() {
        OnPlay();
        NetworkManager.Singleton.StartHost(); //lots of options here, do it boiiiiiii
    }

    public void OnJoin() {
        _connectPanel.gameObject.SetActive(true);
        _mainPanel.gameObject.SetActive(false);

        _curPanel = _connectPanel;
    }

    public void OnBackToMain() {
        _mainPanel.gameObject.SetActive(true);
        _curPanel.gameObject.SetActive(false);

        _curPanel = _mainPanel;
    }

    public void OnConnect() {
        string ip = _ipField.text;
        int port;
        try { port = Convert.ToInt32(_portField.text); } catch { port = 7777; }

        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ip;
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = port;

        OnPlay();
        NetworkManager.Singleton.StartClient();
    }


    








    public void ExitGame() {
        Application.Quit();
    }
}
