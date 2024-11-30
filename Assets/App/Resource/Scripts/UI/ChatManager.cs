using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using UnityEngine.UI;
using System.Reflection;
using Unity.VisualScripting;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;
    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;
 
   
    public string playerName;

    void Awake()
    {
        ChatManager.Singleton = this;
    }
    void Start()
    {

    }
   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) 
        {
            SendChatMessage(chatInput.text, playerName);
            chatInput.text = "";
        }
       

    }
    
    public void SendChatMessage(string _message, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;

        string S = _fromWho + " > " + _message;
        SendChatMessageServerRPC(S);
    }

    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRPC(string message)
    {
        RecieveChatMessageClientRpc(message);
    }
    [ClientRpc]
    private void RecieveChatMessageClientRpc(string message)
    {
        ChatManager.Singleton.AddMessage(message);
    }
}
