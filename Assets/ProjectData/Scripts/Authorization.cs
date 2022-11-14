using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

public class Authorization : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _playFabTitle;
    [SerializeField] private ConnectionPanel _connectionPanel;

    private string _customID;
    private bool _isNeedCreation;

    private const string AUTHINTEFICATION_KEY = "AUTHINTEFICATION_KEY";

    void Start()
    {
        _connectionPanel.SetOfflineConnectionStatus();
        _connectionPanel.ConnectButton.interactable = false;

        _connectionPanel.LoginButton.onClick.AddListener(Login);
        _connectionPanel.ConnectButton.onClick.AddListener(Connect);
        _connectionPanel.DisconnectButton.onClick.AddListener(Disconnect);
        _connectionPanel.ResetButton.onClick.AddListener(ResetProfile);
        _connectionPanel.OldLoginToggle.onValueChanged.AddListener(SetOldLogin);

        if (PlayerPrefs.HasKey(AUTHINTEFICATION_KEY))
        {
            _connectionPanel.OldLoginToggle.gameObject.SetActive(true);
            _connectionPanel.OldLoginToggle.isOn = false;
        }
    }

    private void Login()
    {
        if (_connectionPanel.OldLoginToggle.isOn)
        {
            _isNeedCreation = false;
            _customID = PlayerPrefs.GetString(AUTHINTEFICATION_KEY);

            StartLogin();
        }
        else
        {
            if (string.IsNullOrEmpty(_connectionPanel.LoginInputField.text))
            {
                _connectionPanel.LoginWarning();
                return;
            }
            else
            {
                if (_connectionPanel.IsWarning)
                {
                    _connectionPanel.ResetWarning();
                }

                _isNeedCreation = _customID == PlayerPrefs.GetString(AUTHINTEFICATION_KEY) ? false : true;
                _customID = _connectionPanel.LoginInputField.text;

                if (_isNeedCreation)
                {
                    PlayerPrefs.SetString(AUTHINTEFICATION_KEY, _customID);
                }

                StartLogin();
            }
        }
    }

    private void StartLogin()
    {
        _connectionPanel.ResetButton.gameObject.SetActive(false);
        _connectionPanel.IsLogginInProgress = true;
        _connectionPanel.StartLoginCorutine();

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = _customID,
            CreateAccount = _isNeedCreation
        };

        PlayFabClientAPI.LoginWithCustomID(request, Success, Fail, GetLocalIPv4());
    }

    private void Fail(PlayFabError error)
    {
        Debug.LogError(error);
        _connectionPanel.IsLogginInProgress = false;
        _connectionPanel.SetOfflineConnectionStatus();
    }

    private void Success(LoginResult result)
    {
        Debug.Log(result.PlayFabId);
        Debug.Log((string)result.CustomData);
        PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
        PhotonNetwork.NickName = result.PlayFabId;

        _connectionPanel.ConnectButton.interactable = true;
        _connectionPanel.DisconnectButton.interactable = true;
        _connectionPanel.IsLogginInProgress = false;
        _connectionPanel.SetOfflineConnectionStatus();
        _connectionPanel.SetLoginText($"Player: {result.PlayFabId} \nIP: {(string)result.CustomData}");
    }

    private void SetOldLogin(bool isUseOldLogin)
    {
        if (isUseOldLogin)
        {
            _connectionPanel.LoginInputField.text = PlayerPrefs.GetString(AUTHINTEFICATION_KEY);
        }
        else
        {
            _connectionPanel.LoginInputField.text = "";
        }
    }
    private void ResetProfile()
    {
        PlayerPrefs.DeleteKey(AUTHINTEFICATION_KEY);
        _connectionPanel.OldLoginToggle.gameObject.SetActive(false);
        _connectionPanel.LoginInputField.text = "";
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        }        
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        if (!PhotonNetwork.InRoom)
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");

        _connectionPanel.SetOnlineConnectionStatus();
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected");
        _connectionPanel.SetOfflineConnectionStatus();
    }

    private void OnDestroy()
    {
        _connectionPanel.LoginButton.onClick.RemoveAllListeners();
        _connectionPanel.ConnectButton.onClick.RemoveAllListeners();
        _connectionPanel.DisconnectButton.onClick.RemoveAllListeners();
    }
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }
}
