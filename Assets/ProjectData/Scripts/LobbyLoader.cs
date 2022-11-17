using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyLoader : MonoBehaviourPunCallbacks
{
    [SerializeField] private LobbyView _lobbyView;

    public const string MAP_PROP_KEY = "map";
    public const string OWNER = "owner";


    private string _playerName;
    private string _playerID;

    private void Start()
    {
        _lobbyView.SetOfflineConnectionStatus();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);

        _lobbyView.ConnectButton.onClick.AddListener(Connect);
        _lobbyView.DisconnectButton.onClick.AddListener(Disconnect);
        _lobbyView.CreateRoomButton.onClick.AddListener(CreateRoom);
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
        var roomName = $"Game Room {Random.Range(0, 100)}";

        var roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            CustomRoomProperties = new Hashtable { { MAP_PROP_KEY, "Map_3" }, { OWNER, _playerName} },
            CustomRoomPropertiesForLobby = new[] { MAP_PROP_KEY, OWNER },
            IsVisible = true,
            IsOpen = true,
            PublishUserId = true,
            PlayerTtl = 10000
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);

        Debug.Log("CreateRoom");
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
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
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
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
}
