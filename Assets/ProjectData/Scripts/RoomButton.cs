using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour, IPointerClickHandler
{
    public event Action<RoomButton> OnClickRoomMiniView;
    public bool IsSelected { get; private set; }
    public RoomInfo RoomInfo { get; private set; }
    public bool IsCloseOrNotVisible { get; private set; }

    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _roomOwner;
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private Color _selectedColor;

    private Color _idleColor;
    private Color _idleTextColor;
    private string _ownerName;

    public void Init(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;

        if(RoomInfo.CustomProperties.ContainsKey(LobbyManager.OWNER))
        {
            _ownerName = RoomInfo.CustomProperties[LobbyManager.OWNER].ToString();
            _roomOwner.text = $"Owner: {_ownerName}";
        }
        _roomName.text = roomInfo.Name;
        _idleColor = _backGroundImage.color;
        _idleTextColor = _roomName.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsCloseOrNotVisible) return;

        OnClickRoomMiniView?.Invoke(this);
        Debug.Log("OnPointerClick");
    }

    public void SelectView()
    {
        if (IsCloseOrNotVisible) return;

        IsSelected = true;
        _backGroundImage.color = _selectedColor;
    }

    public void DeselectView()
    {
        if (IsCloseOrNotVisible) return;

        IsSelected = false;        
        _backGroundImage.color = _idleColor;
    }

    public void ClearRoomMiniView()
    {
        RoomInfo = null;
        _roomName.text = "";
    }

    public void SetCloseOrInvisibleStatus()
    {
        IsCloseOrNotVisible = true;
        _backGroundImage.color = new Color(_idleColor.r, _idleColor.g, _idleColor.b, _idleColor.a * 0.2f);
        _roomName.color = new Color(_idleTextColor.r, _idleTextColor.g, _idleTextColor.b, _idleTextColor.a * 0.2f);
        _roomOwner.color = new Color(_idleTextColor.r, _idleTextColor.g, _idleTextColor.b, _idleTextColor.a * 0.2f);
    }

    public void TakeOffCloseOrInvisibleStatus()
    {
        IsCloseOrNotVisible = false;
        _backGroundImage.color = new Color(_idleColor.r, _idleColor.g, _idleColor.b, _idleColor.a);
        _roomName.color = new Color(_idleTextColor.r, _idleTextColor.g, _idleTextColor.b, _idleTextColor.a);
        _roomOwner.color = new Color(_idleTextColor.r, _idleTextColor.g, _idleTextColor.b, _idleTextColor.a);
    }
}
