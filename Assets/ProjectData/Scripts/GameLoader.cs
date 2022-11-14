using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text _accauntInfo;
    [SerializeField] private TMP_Text _catalog;

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);
    }


    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        var accountInfo = result.AccountInfo;
        _accauntInfo.text = $"Hi! {accountInfo.Username} \n" +
                           $"{accountInfo.PlayFabId} \n";
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        ShowCatalog(result.Catalog);
        Debug.Log("Catalog is loaded");
    }

    private void ShowCatalog(List<CatalogItem> catalog)
    {
        _catalog.text = "Items catalog:";

        foreach (var item in catalog)
        {
            _catalog.text += $"\n{item.DisplayName}";                                 
        }
    }
}
