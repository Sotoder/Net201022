using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour, IPointerClickHandler
{
    public event Action<CharacterView> OnClickCharacterView;
    public bool IsSelected { get; private set; }
    public CharacterResult CharacterResult => _characterResult;

    [SerializeField] private TMP_Text _classText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _dmgText;
    [SerializeField] private Image _classImage;
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Sprite _shootaSprite;
    [SerializeField] private Sprite _bigShootaSprite;

    private Color _idleColor;
    private CharacterResult _characterResult;
    

    private void Start()
    {
        _idleColor = _backGroundImage.color;
    }

    private void OnGetCharacterData(GetCharacterStatisticsResult result)
    {
        _hpText.text = "HP: " + result.CharacterStatistics[ConstantsForPlayFab.CHARACTER_HP].ToString();
        _dmgText.text = "DMG: " + result.CharacterStatistics[ConstantsForPlayFab.CHARACTER_DMG].ToString();
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    public void Init(CharacterResult characterResult)
    {
        _characterResult = characterResult;
        _classText.text = characterResult.CharacterName;

        _classImage.sprite = characterResult.CharacterType switch
        {
            ConstantsForPlayFab.SHOOTA_ID => _shootaSprite,
            ConstantsForPlayFab.BIG_SHOOTA_ID => _bigShootaSprite
        };

        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
        {
            CharacterId = characterResult.CharacterId
        }, OnGetCharacterData, OnError);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickCharacterView?.Invoke(this);
    }
    public void SelectView()
    {
        IsSelected = true;
        _backGroundImage.color = _selectedColor;
    }

    public void DeselectView()
    {
        IsSelected = false;
        _backGroundImage.color = _idleColor;
    }
}
