using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour
{
    public bool IsLogginInProgress;

    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _loginText;
    [SerializeField] private Image _statusImage;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Sprite _connectSprite;
    [SerializeField] private Sprite _disconnectSprite;
    [SerializeField] private Sprite _loadSprite;
    [SerializeField] private Toggle _oldLoginToggle;

    private bool _isWarning;

    public TMP_InputField LoginInputField => _inputField;
    public Button LoginButton => _loginButton;
    public Button ConnectButton => _connectButton;
    public Button DisconnectButton => _disconnectButton;
    public Button ResetButton => _resetButton;
    public Toggle OldLoginToggle => _oldLoginToggle;
    public bool IsWarning => _isWarning;

    public void LoginWarning()
    {
        _inputField.image.color = new Color(1, 0, 0, 0.5f);
        _isWarning = true;
    }

    public void StartLoginCorutine()
    {
        _statusImage.sprite = _loadSprite;
        _statusText.text = "<color=white>Connecting</color>";
        StartCoroutine(LoginProgressCoroutine());
    }

    private IEnumerator LoginProgressCoroutine()
    {
        while (IsLogginInProgress)
        {
            _statusImage.transform.Rotate(Vector3.forward * Time.deltaTime * 100);
            yield return new WaitForEndOfFrame();
        }

        _statusImage.transform.rotation = Quaternion.identity;
    }

    public void ResetWarning()
    {
        _inputField.image.color = Color.white;
        _isWarning = false;
    }

    public void SetLoginText(string messege)
    {
        _loginText.text = messege;
    }

    public void SetOfflineConnectionStatus()
    {
        _statusImage.sprite = _disconnectSprite;
        _statusText.text = "<color=red>Offline</color>";
        _connectButton.gameObject.SetActive(true);
        _disconnectButton.gameObject.SetActive(false);
    }
    public void SetOnlineConnectionStatus()
    {
        _statusImage.sprite = _connectSprite;
        _statusText.text = "<color=green>Online</color>";
        _connectButton.gameObject.SetActive(false);
        _disconnectButton.gameObject.SetActive(true);
    }
}
