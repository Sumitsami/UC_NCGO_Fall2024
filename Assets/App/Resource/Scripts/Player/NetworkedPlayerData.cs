using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkedPlayerData : NetworkBehaviour
{
    public NetworkList<PlayerInfoData> _allConnectedPlayers;
    private int _players = -1;
    private ulong _serverLocalID;

    private Color[] _PlayerColors = new Color[]
    {
        Color.blue, Color.magenta, Color.cyan, Color.yellow, Color.white
    };

    private void Awake()
    {
        _allConnectedPlayers = new NetworkList<PlayerInfoData>(readPerm: NetworkVariableReadPermission.Everyone);
    }
    public override void OnNetworkDespawn()
    {
        

        if (!IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvents;
            _serverLocalID = NetworkManager.ServerClientId;
        }
        base.OnNetworkDespawn();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvents;
        }
    }
    private void OnConnectionEvents(NetworkManager netManager, ConnectionEventData eventData)
    {
        if (eventData.EventType== ConnectionEvent.ClientConnected)
        {
            CreateNewClientData(eventData.ClientId);
        }
        if (eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            RemovePlayerData(FindPlayerInfoData(eventData.ClientId));
            _players--;
        }
    }

    

    private void CreateNewClientData(ulong clientID)
    {
        PlayerInfoData playerInfoData = new PlayerInfoData(clientID);

        if (_serverLocalID == clientID)
        {
            playerInfoData._isPlayerReady = true;
        }
        else
        {
            playerInfoData._isPlayerReady = false;
        }

        _players++;

        playerInfoData._colorId = _PlayerColors[_players];

        _allConnectedPlayers.Add(playerInfoData);
    }
    public void RemovePlayerData(PlayerInfoData playerData)
    {
        _allConnectedPlayers.Remove(playerData);   
    }

    public PlayerInfoData FindPlayerInfoData(ulong clientID)
    {
        return _allConnectedPlayers[FindPlayerIndex(clientID)];
    }

    private int FindPlayerIndex(ulong clientID) {
        int myMatch = -1;
        for (int i = 0; i < _allConnectedPlayers.Count; i++)
        {
            if (clientID == _allConnectedPlayers[i]._clientId)
            {
                myMatch = i;
            }
        }
        return myMatch;
    }
    public void UpdateReadyClient(ulong clientID, bool isReady) 
    { 
        int idx = FindPlayerIndex(clientID);

        if(idx == -1) { return; }

        PlayerInfoData playerInfo = new PlayerInfoData();
        playerInfo = _allConnectedPlayers[idx];
        playerInfo._isPlayerReady = isReady;
        _allConnectedPlayers[idx] = playerInfo;
    }
}
