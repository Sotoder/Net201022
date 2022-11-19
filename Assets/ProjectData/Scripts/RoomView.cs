using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomView : MonoBehaviour
{
    [SerializeField] private Button _closeRoomButton;
    [SerializeField] private TMP_Text _closeButtonText;
    [SerializeField] private Button _hideRoomButton;
    [SerializeField] private TMP_Text _hideButtonText;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private List<PlayerSlotView> _playerInRoomViews;

    private bool _isVisible = true;
    private bool _isOpen = true;
    private string _roomOwner;
    private string _playerName;

    public Button CloseRoomButton => _closeRoomButton;
    public Button HideRoomButton => _hideRoomButton;
    public Button StartGameButton => _startGameButton;

    public void ShowRoom()
    {
        _nameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        _roomOwner = PhotonNetwork.CurrentRoom.CustomProperties[LobbyManager.OWNER].ToString();
        _playerName = PhotonNetwork.LocalPlayer.NickName;

        if (_roomOwner != _playerName)
        {
            _closeRoomButton.gameObject.SetActive(false);
            _hideRoomButton.gameObject.SetActive(false);
            _startGameButton.gameObject.SetActive(false);
        } else
        {
            _closeRoomButton.onClick.AddListener(ChangeOpenRoomStatus);
            _hideRoomButton.onClick.AddListener(ChangeVisibleRoomStatus);
            _startGameButton.onClick.AddListener(StartGame);
        }
    }

    private void StartGame()
    {
        Debug.Log("And Finaly we started /sigh");
    }

    private void ChangeVisibleRoomStatus()
    {
        _isVisible = _isVisible ? false : true;
        PhotonNetwork.CurrentRoom.IsVisible = _isVisible;
        _hideButtonText.text = _isVisible ? "Hide" : "Show";
    }

    private void ChangeOpenRoomStatus()
    {
        _isOpen = _isOpen ? false : true;
        PhotonNetwork.CurrentRoom.IsOpen = _isOpen;
        _closeButtonText.text = _isOpen ? "Close" : "Open";
    }

    public void OnNewPlayerEntredInRoom(Player newPlayer)
    {
        FindAndTakeFreeSlot(newPlayer);

        if (_roomOwner == _playerName)
        {
            CheckOnMaxPlayers();
        }
    }

    public void OnJoinRoom()
    {     
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            FindAndTakeFreeSlot(player.Value);
        }
    }

    private void CheckOnMaxPlayers()
    {
        if (_isOpen && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            _isOpen = false;
            PhotonNetwork.CurrentRoom.IsOpen = _isOpen;
            _closeRoomButton.interactable = _isOpen;
        }
        else
        {
            _closeRoomButton.interactable = true;
        }
    }

    private void FindAndTakeFreeSlot(Player newPlayer)
    {
        for (int i = 0; i < _playerInRoomViews.Count; i++)
        {
            if (!_playerInRoomViews[i].IsBusy)
            {
                _playerInRoomViews[i].TakeSlot(newPlayer);
                break;
            }
        }
    }

    public void OnPlayerLeftRoom(Player player)
    {
        for(int i = 0; i < _playerInRoomViews.Count; i++)
        {
            if (_playerInRoomViews[i].Player != null && (_playerInRoomViews[i].Player.NickName == player.NickName))
            {
                _playerInRoomViews[i].ClearSlot();
            }
        }

        CheckOnMaxPlayers();
    }

    private void OnDestroy()
    {
        _closeRoomButton.onClick.RemoveAllListeners();
        _hideRoomButton.onClick.RemoveAllListeners();
        _startGameButton.onClick.RemoveAllListeners();
    }
}
