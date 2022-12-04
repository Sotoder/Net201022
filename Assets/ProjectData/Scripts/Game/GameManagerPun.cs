using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerPun : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _shootaPrefab;
    [SerializeField] private GameObject _bigShootaPrefab;
    [SerializeField] private List<Transform> _spawnPoints;

    private void Start()
    {
        var playerSlotNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        var playerCharacterType = PhotonNetwork.LocalPlayer.CustomProperties[ConstantsForPhoton.CHARACTER_TYPE].ToString();
        Boy character = default;

        if (playerCharacterType == ConstantsForPlayFab.BIG_SHOOTA_ID)
        {
            var boyGameObject = PhotonNetwork.Instantiate(_bigShootaPrefab.name, _spawnPoints[playerSlotNumber].position, _spawnPoints[playerSlotNumber].rotation);
            character = boyGameObject.GetComponent<Boy>();
        } else
        {
            var boyGameObject = PhotonNetwork.Instantiate(_shootaPrefab.name, _spawnPoints[playerSlotNumber].position, _spawnPoints[playerSlotNumber].rotation);
            character = boyGameObject.GetComponent<Boy>();
        }

        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
        {
            CharacterId = PhotonNetwork.LocalPlayer.CustomProperties[ConstantsForPhoton.CHARACTER_ID].ToString()
        }, result => LoadCharacterParameters(result, character), error => Debug.Log(""));
    }

    private void LoadCharacterParameters(GetCharacterStatisticsResult result, Boy character)
    {
        var hp = result.CharacterStatistics[ConstantsForPlayFab.CHARACTER_HP];
        var dmg = result.CharacterStatistics[ConstantsForPlayFab.CHARACTER_DMG];
        
        character.SetCharacterParameters(hp,dmg);

        SendCharacterParameters(character.PhotonView.ViewID, hp, dmg);
    }

    private void SendCharacterParameters(int viewID, float hp, float dmg)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] eventContent = new object[]
        {
            viewID,
            hp,
            dmg
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventsTypes.ParametersSyncEvent, eventContent, options, sendOptions);
        Debug.Log("SendParametersEvent");
    }
}
