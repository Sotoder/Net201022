using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour
{
    [Header("Lobby")]
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private TMP_Text _accauntInfo;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private Image _statusImage;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private Sprite _connectSprite;
    [SerializeField] private Sprite _disconnectSprite;

    [Header("Catalog")]
    [SerializeField] private GameObject _catalogPanel;
    [SerializeField] private TMP_Text _catalog;
    [SerializeField] private Button _showCatalogButton;
    [SerializeField] private Button _closeCatalogButton;

    [Header("Rooms")]
    [SerializeField] private Button _joinToRoomButton;
    [SerializeField] private Button _createRoomButton;   
    [SerializeField] private Transform _roomListTransform;
    [SerializeField] private GameObject _roomButtonPrefab;

    private Dictionary<int, RoomButton> _roomButtons = new Dictionary<int, RoomButton>();
    private RoomButton _selectedRoom;

    public Button ConnectButton => _connectButton;
    public Button DisconnectButton => _disconnectButton;
    public Button JoinToRoomButton => _joinToRoomButton;
    public Button CreateRoomButton => _createRoomButton;
    public TMP_Text AccauntInfo => _accauntInfo;
    public TMP_Text Catalog => _catalog;
    public Transform RoomListTransform => _roomListTransform;
    public GameObject LobbyPanel => _lobbyPanel;
    public Dictionary<int, RoomButton> RoomButtons => _roomButtons;

    private void Start()
    {
        _createRoomButton.interactable = false;
        _showCatalogButton.onClick.AddListener(ShowCatalogPanel);
        _closeCatalogButton.onClick.AddListener(CloseCatalogPanel);

        _disconnectButton.gameObject.SetActive(false);
        _catalogPanel.SetActive(false);
    }

    private void CloseCatalogPanel()
    {
        _catalogPanel.SetActive(false);
        _lobbyPanel.SetActive(true);
    }

    private void ShowCatalogPanel()
    {
        _catalogPanel.SetActive(true);
        _lobbyPanel.SetActive(false);
    }

    public void SetOfflineConnectionStatus()
    {
        _createRoomButton.interactable = false;
        _statusImage.sprite = _disconnectSprite;
        _statusText.text = "<color=red>Offline</color>";
        _connectButton.gameObject.SetActive(true);
        _disconnectButton.gameObject.SetActive(false);
    }
    public void SetOnlineConnectionStatus()
    {
        _createRoomButton.interactable = true;
        _statusImage.sprite = _connectSprite;
        _statusText.text = "<color=green>Online</color>";
        _connectButton.gameObject.SetActive(false);
        _disconnectButton.gameObject.SetActive(true);
    }

    public void UpdateRoomListView(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (!_roomButtons.TryGetValue(roomList[i].masterClientId, out var roomButton))
            {
                var roomObject = Instantiate(_roomButtonPrefab, _roomListTransform);
                roomButton = roomObject.GetComponent<RoomButton>();
                roomButton.Init(roomList[i]);
                _roomButtons.Add(roomList[i].masterClientId, roomButton);

                roomButton.OnClickRoomMiniView += SelectPickedRoom;
            }

            if ((!roomList[i].IsOpen || !roomList[i].IsVisible) && !roomButton.IsCloseOrNotVisible)
            {
                roomButton.SetCloseOrInvisibleStatus();
            } 
            else
            {
                roomButton.TakeOffCloseOrInvisibleStatus();
            }

            if (roomList[i].RemovedFromList)
            {
                RemoveRoomButton(roomButton);
            }
        }
    }

    public void ClearRoomList()
    {
        for(int i = 0; i < _roomButtons.Count; i++)
        {
            RemoveRoomButton(_roomButtons[i]);
        }
    }

    private void RemoveRoomButton(RoomButton roomButton)
    {
        _roomButtons.Remove(roomButton.RoomInfo.masterClientId);
        roomButton.OnClickRoomMiniView -= SelectPickedRoom;
        Destroy(roomButton.gameObject);
    }

    private void SelectPickedRoom(RoomButton pickedRoom)
    {
        _selectedRoom = pickedRoom;

        for (int i = 0; i < _roomButtons.Count; i++)
        {
            if (_roomButtons[i] == pickedRoom)
            {
                if (!_roomButtons[i].IsSelected)
                {
                    _roomButtons[i].SelectView();
                    SetJoinButtonInteractabeState(true);
                }
                else
                {
                    _roomButtons[i].DeselectView();
                    _selectedRoom = null;
                    SetJoinButtonInteractabeState(false);
                }
            }
            else
            {
                if (_roomButtons[i].IsSelected)
                {
                    _roomButtons[i].DeselectView();
                }
            }
        }
    }

    private void SetJoinButtonInteractabeState(bool isInteractable)
    {
        _joinToRoomButton.interactable = isInteractable;
    }
}
