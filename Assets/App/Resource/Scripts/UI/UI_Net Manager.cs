using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class UI_NetManager : NetworkBehaviour
{
    [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn, _startBttn;

    [SerializeField] private GameObject _connectionbttnGroup, _socialPanel;

    [SerializeField] private SpawnController _mySpawnController;
    // Start is called before the first frame update
    void Start()
    {
        _startBttn.gameObject.SetActive(false);
       if (_hostBttn != null) _hostBttn.onClick.AddListener(HostClick);
       if (_clientBttn != null) _clientBttn.onClick.AddListener(ClientClick);
       if (_serverBttn != null) _serverBttn.onClick.AddListener(ServerClick);
       if (_startBttn != null) _startBttn.onClick.AddListener(StartClick);
    }

    private void StartClick()
    {
        // hook up spawning
        if (IsServer)
        {
            _mySpawnController.SpawnAllPlayers();
            
            
            HideGuiRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void HideGuiRpc()
    {
        _socialPanel.SetActive(false);
    }

    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
        _connectionbttnGroup.SetActive(false);
        _startBttn.gameObject.SetActive(true);
    }
    private void ClientClick()
    {
        NetworkManager.Singleton.StartClient();
        _connectionbttnGroup.SetActive(false);
    }
    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
        _connectionbttnGroup.SetActive(false);
        _startBttn.gameObject.SetActive(true);
    }
}
