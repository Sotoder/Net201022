using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private LobbyView _lobbyView;
    [SerializeField] private CreateRoomView _createRoomView;
    [SerializeField] private RoomView _roomView;

    public const string MAP_PROP_KEY = "map";
    public const string OWNER = "owner";
    public const string FRIENDS = "friends";
    public const string PASSWORD = "password";


    private string _playerName;
    private string _playerID;
    private string[] _friendsList;

    private void Start()
    {
        _lobbyView.SetOfflineConnectionStatus();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);

        _lobbyView.ConnectButton.onClick.AddListener(Connect);
        _lobbyView.DisconnectButton.onClick.AddListener(Disconnect);
        _lobbyView.CreateRoomButton.onClick.AddListener(ShowRoomCreationPanel);
        _createRoomView.CreateRoomButton.onClick.AddListener(CreateRoom);
    }

    private void ShowRoomCreationPanel()
    {
        _createRoomView.CreateRoomPanel.gameObject.SetActive(true);
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        var accountInfo = result.AccountInfo;
        _playerID = accountInfo.PlayFabId;
        _playerName = accountInfo.Username;

        _lobbyView.AccauntInfo.text = $"Hi! {_playerName} \n" +
                           $"{_playerID} \n";
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        WriteCatalog(result.Catalog);
        Debug.Log("Catalog is loaded");
    }

    private void WriteCatalog(List<CatalogItem> catalog)
    {
        _lobbyView.Catalog.text = "Items catalog:";

        foreach (var item in catalog)
        {
            _lobbyView.Catalog.text += $"\n{item.DisplayName}";                                 
        }
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.AuthValues = new AuthenticationValues(_playerID);
        PhotonNetwork.NickName = _playerName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
    }

    private void CreateRoom()
    {
        var roomName = _createRoomView.RoomName;
        RoomOptions roomOptions = new RoomOptions 
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            PublishUserId = true,
            PlayerTtl = 10000
        };
        
        if(_createRoomView.ByFriendsToggle.isOn)
        {
            _friendsList = _createRoomView.FriendsList.Split(",");

            var customRoomProperties = new Hashtable { { MAP_PROP_KEY, "Map_3" }, { OWNER, _playerName }, { FRIENDS, _friendsList } };
            var customRoomPropertiesForLobby = new[] { MAP_PROP_KEY, OWNER, FRIENDS };

            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;

        } 
        else if (_createRoomView.ByPasswordToggle.isOn)
        {
            var password = _createRoomView.Password;
            var customRoomProperties = new Hashtable { { MAP_PROP_KEY, "Map_3" }, { OWNER, _playerName }, { PASSWORD, password } };
            var customRoomPropertiesForLobby = new[] { MAP_PROP_KEY, OWNER, PASSWORD };

            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;
        }
        else
        {
            var customRoomProperties = new Hashtable { { MAP_PROP_KEY, "Map_3" }, { OWNER, _playerName }};
            var customRoomPropertiesForLobby = new[] { MAP_PROP_KEY, OWNER};

            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;
        }

        if(_createRoomView.ByFriendsToggle.isOn)
        {
            PhotonNetwork.CreateRoom(roomName, roomOptions, expectedUsers: _friendsList);
        } else
        {
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        Debug.Log("CreateRoom");
        _createRoomView.CreateRoomPanel.SetActive(false);
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
        _lobbyView.ClearRoomList();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
        _lobbyView.SetOnlineConnectionStatus();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("OnJoinedLobby");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        _lobbyView.LobbyPanel.SetActive(false);
        _roomView.gameObject.SetActive(true);
        _roomView.ShowRoom();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
        _roomView.OnJoinRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        _lobbyView.UpdateRoomListView(roomList);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected");
        _lobbyView.SetOfflineConnectionStatus();
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        Debug.Log("Left Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        _roomView.OnNewPlayerEntredInRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        _roomView.OnPlayerLeftRoom(otherPlayer);
    }
}