using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CharactersCreatorController
{
    private RoomView _roomView;

    public CharactersCreatorController(RoomView roomView) 
    {
        _roomView = roomView;
    }


    public void GetPlayerCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(), OnGetCharactersSuccess, OnError);
    }

    private void OnGetCharactersSuccess(ListUsersCharactersResult result)
    {
        if (result.Characters.Count == 0)
        {
            CreateNewCharacters();
        }
        else
        {
            _roomView.LoadCharacters(result.Characters);
        }
    }

    private void CreateNewCharacters()
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            CatalogVersion = ConstantsForPlayFab.CATALOG_VERSION,
            StoreId = ConstantsForPlayFab.CHARACTER_STORE
        }, result =>
        {
            foreach (var item in result.Store)
            {
                GetCharacterToken(item);
            }
        }, OnError);

    }

    private void GetCharacterToken(StoreItem storeItem)
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            ItemId = storeItem.ItemId,
            Price = (int)storeItem.VirtualCurrencyPrices[ConstantsForPlayFab.EXP],
            VirtualCurrency = ConstantsForPlayFab.EXP
        }, result => CreateCharacterWhithToken(result.Items[0].ItemId), OnError);
    }

    private void CreateCharacterWhithToken(string itemID)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = itemID switch
            {
                ConstantsForPlayFab.SHOOTA_ID => "Shoota Boy",
                ConstantsForPlayFab.BIG_SHOOTA_ID => "Bigshoota Boy"
            },
            ItemId = itemID
        }, result => SetNewCharacterStatistics(result), OnError);
    }

    private void SetNewCharacterStatistics(GrantCharacterToUserResult result)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = result.CharacterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                [ConstantsForPlayFab.CHARACTER_HP] = 100,
                [ConstantsForPlayFab.CHARACTER_DMG] = result.CharacterType switch
                {
                    ConstantsForPlayFab.SHOOTA_ID => 20,
                    ConstantsForPlayFab.BIG_SHOOTA_ID => 50
                }
            }
        }, result => Debug.Log($"Initial stats set"), OnError); ;
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }
}