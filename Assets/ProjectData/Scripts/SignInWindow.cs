using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInWindow : AccountDataWindowBase
{
    [SerializeField] private Button _signInButton;
    [SerializeField] private Image _statusImage;
    [SerializeField] private Sprite _loadSprite;

    private bool _isLogginInProgress;

    protected override void SubscriptionsElementsUI()
    {
        base.SubscriptionsElementsUI();

        _signInButton.onClick.AddListener(SignIn);
    }

    private void SignIn()
    {
        _isLogginInProgress = true;
        StartConnectionCorutine();

        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _password,
        }, Success, Fail); 
    }

    private void Success(LoginResult result)
    {
        _isLogginInProgress = false;
        Debug.Log($"Success: {_username}, {result.PlayFabId}");
        SceneManager.LoadScene(1);
    }

    private void Fail(PlayFabError error)
    {
        _isLogginInProgress = false;
        Debug.Log($"Fail: {error.ErrorMessage}");
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
