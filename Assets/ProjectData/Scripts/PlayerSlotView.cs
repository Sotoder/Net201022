using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerClassText;

    public Player Player { get; private set; }
    public bool IsBusy { get; private set; }

    public void TakeSlot(Player player)
    {
        Player = player;
        _playerNameText.text = "Player: " + player.NickName;
        IsBusy = true;
    }

    public void ClearSlot()
    {
        Player = null;
        _playerNameText.text = "Player: ";
        IsBusy = false;
    }
    
}
