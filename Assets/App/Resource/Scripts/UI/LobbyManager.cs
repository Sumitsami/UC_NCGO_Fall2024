using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _startBttn, _leaveBttn, _readyBttn;
    [SerializeField] private GameObject _panelPrefab;
    [SerializeField] private GameObject _ContentGO;
    [SerializeField] private TMP_Text rdyTxt;

    [SerializeField] private NetworkedPlayerData _networkPlayers;

    private List<GameObject>  _PlayerPanels = new List<GameObject>();

    private ulong _myLocalClientId;

    private bool isReady = false;

    private void Start()
    {
        _myLocalClientId = NetworkManager.ServerClientId;

        if (IsServer)
        {
            rdyTxt.text = "Waiting for Players";
            _readyBttn.gameObject.SetActive(false);
        }
        else
        {
            rdyTxt.text = "Not Ready";
            _readyBttn.gameObject.SetActive(true);
        }

        _networkPlayers._allConnectedPlayers.OnListChanged += NetPlayersChanged;
        _leaveBttn.onClick.AddListener(LeaveBttnClick);
        _readyBttn.onClick.AddListener(ClientRdyBttnToggle);
    }

    private void ClientRdyBttnToggle()
    {
        if(IsServer) { return; }
        isReady = !isReady;
        if (isReady)
        {
            rdyTxt.text = "Ready";
        }
        else
        {
            rdyTxt.text = "Not Ready";
        }

        RdyBttnToggleServerRpc(isReady);
    }
    [Rpc(SendTo.Server,RequireOwnership =false)]
    private void RdyBttnToggleServerRpc(bool readyStatus, RpcParams rpcParams = default)
    {
        Debug.Log("From Rdy bttn RPC");
        _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
    }

    private void LeaveBttnClick()
    {
        if (!IsServer)
        {
            QuitLobbyServerRpc();
            SceneManager.LoadScene(0);
        }
        else
        {
            foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
            {
                if (playerData._clientId != _myLocalClientId)
                {
                    KickUserBttn(playerData._clientId);
                }
            }
            NetworkManager.Shutdown();
            SceneManager.LoadScene(0);
        }
    }
    [Rpc(SendTo.Server, RequireOwnership =false)]

    private void QuitLobbyServerRpc(RpcParams rpcParams=default)
    {
        KickUserBttn(rpcParams.Receive.SenderClientId);
    }

    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeEvent)
    {
        Debug.Log("Net Players has changed event fired!");
        PopulateLabels();
    }

    [ContextMenu("PopulateLabel")]
    private void PopulateLabels()
    {
        ClearPlayerPanel();

        bool allReady = true;

        foreach (PlayerInfoData playerData in  _networkPlayers._allConnectedPlayers)
        {
            GameObject newPlayerPanel = Instantiate(_panelPrefab, _ContentGO.transform);
            PlayerLabel _playerLabel = newPlayerPanel.GetComponent<PlayerLabel>();

            _playerLabel.onKickClicked += KickUserBttn;

            if (IsServer && playerData._clientId != _myLocalClientId)
            {
                _playerLabel.setKickActive(true);
                _readyBttn.GameObject().SetActive(false);
            }
            else
            {
                _playerLabel.setKickActive(false);
                
            }
            _playerLabel.SetPlayerLabelName(playerData._clientId);
            _playerLabel.SetReady(playerData._isPlayerReady);
            _playerLabel.SetPlayerColor(playerData._colorId);
            _PlayerPanels.Add(newPlayerPanel);
            
            if(playerData._isPlayerReady == false)
            {
                allReady = false;
            }
        }
         if (IsServer)
        {
            if (allReady)
            {
                if(_networkPlayers._allConnectedPlayers.Count > 1)
                {
                    rdyTxt.text = "Ready to start";
                    _startBttn.gameObject.SetActive(true);
                }
                else
                {
                    rdyTxt.text = "Empty Lobby";
                }
            }
            else
            {
                _startBttn.gameObject.SetActive(false);
                rdyTxt.text = "waiting for ready players";
            }
        }
    }

    private void KickUserBttn(ulong kickTarget)
    {
     if(!IsServer || !IsHost) return;
     foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            if(playerData._clientId == kickTarget)
            {
                // _networkPlayers._allConnectedPlayers.Remove(playerData);

                KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));

                NetworkManager.Singleton.DisconnectClient(kickTarget);
            }
        }
    }
    [Rpc(SendTo.SpecifiedInParams)]
    private void KickedClientRpc(RpcParams rpcParams)
    {
        SceneManager.LoadScene(0);
    }

    private void ClearPlayerPanel()
    {
        foreach (GameObject panel in _PlayerPanels)
        {
            Destroy(panel);
        }
        _PlayerPanels.Clear();
    }
}

