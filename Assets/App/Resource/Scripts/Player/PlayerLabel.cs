using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json.Bson;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerText;
    [SerializeField] private Button _kickBttn;
    [SerializeField] private Image _ReadyStatusImg, _PlayerColorImg;

    public event Action<ulong> onKickClicked;
    private ulong _clientId;


    private void OnEnable()
    {
        _kickBttn.onClick.AddListener((BttnKick_Clicked));
    }
    public void  SetPlayerLabelName(ulong playerName) 
    {
        _clientId = playerName;
        _playerText.text = "Player "+playerName.ToString();
    }

    private void BttnKick_Clicked()
    {
        onKickClicked?.Invoke(_clientId);
    }

    public void SetKickActive(bool isOn)
    {
        _kickBttn.gameObject.SetActive(isOn);
    }

    public void SetReady(bool ready)
    {
        if (ready)
        {
            _ReadyStatusImg.color= Color.green;
        }
        else
        {
            _ReadyStatusImg.color = Color.red;   
        }
    }

    public void setPlayerColor(Color color)
    {
        _PlayerColorImg.color = color;
    }

}
