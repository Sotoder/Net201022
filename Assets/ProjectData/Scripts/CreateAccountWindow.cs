using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateAccountWindow : AccountDataWindowBase
{
    [SerializeField] private InputField _emailField;
    [SerializeField] private Button _createAccountButton;
    [SerializeField] private Image _statusImage;
    [SerializeField] private Sprite _loadSprite;

    private string _email;
    private bool _isLogginInProgress;

    protected override void SubscriptionsElementsUI()
    {
        base.SubscriptionsElementsUI();

        _emailField.onValueChanged.AddListener(UpdateEmail);
        _createAccountButton.onClick.AddListener(CreateAccount);
    }

    private void UpdateEmail(string email)
    {
        _email = email;
    }

    private void CreateAccount()
    {
        _isLogginInProgress = true;
        StartConnectionCorutine();

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _email,
            Password = _password
        }, Success, Fail);
    }

    private void Fail(PlayFabError error)
    {
        _isLogginInProgress = false;
        Debug.Log($"Fail: {error.ErrorMessage}");
    }

    private void Success(RegisterPlayFabUserResult result)
    {
        _isLogginInProgress = false;
        Debug.Log($"Success: {_username}, {result.PlayFabId}");
        SceneManager.LoadScene(1);
    }

    public void StartConnectionCorutine()
    {
        _statusImage.sprite = _loadSprite;
        _statusImage.color = Color.white;
        StartCoroutine(LoginProgressCoroutine());
    }

    private IEnumerator LoginProgressCoroutine()
    {
        while (_isLogginInProgress)
        {
            _statusImage.transform.Rotate(Vector3.forward * Time.deltaTime * 100);
            yield return new WaitForEndOfFrame();
        }

        _statusImage.transform.rotation = Quaternion.identity;
        _statusImage.sprite = null;
        _statusImage.color = new Color(0, 0, 0, 0);
    }
}
