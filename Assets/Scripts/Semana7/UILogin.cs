using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [Header("Panels")]

    [Header("Panels")]
    [SerializeField] private GameObject panelSelectLogin;  
    [SerializeField] private GameObject loginPanel;   
    //[SerializeField] private Transform userPanel;

    [Header("Buttons")]
    [SerializeField] private Button anonLoginButton;
    [SerializeField] private Button unityLoginButton;

    [Header("UI Texts")]
    [SerializeField] private TMP_Text statusText;
    //[Header("Texts / Inputs")]

    //[SerializeField] private Button loginButton;
    //[SerializeField] private TMP_Text playerIDTxt;
    //[SerializeField] private TMP_Text playerNameTxt;

    //[SerializeField] private TMP_InputField UpdateNameIF;
    //[SerializeField] private Button updateNameBtn;


    //[SerializeField] private UnityPlayerAuth unityPlayerAuth;
    /*void Start()
    {
        loginPanel.gameObject.SetActive(true);
        userPanel.gameObject.SetActive(false);
    }*/
    private void OnEnable()
    {
        /* loginButton?.onClick.AddListener(LoginButton);
         unityPlayerAuth.OnSingedIn += UnityPlayerOnSignedIn;

         updateNameBtn.onClick.AddListener(UpdateName);
         unityPlayerAuth.OnUpdateName += UpdateNameVisual;*/
        anonLoginButton.onClick.AddListener(() => OnSelectLogin(true));
        unityLoginButton.onClick.AddListener(() => OnSelectLogin(false));

    }

    private void Start()
    {
        panelSelectLogin.SetActive(true);
        loginPanel.SetActive(false);
        AuthManager.Instance.OnSignedIn += OnPlayerSignedIn;
    }
    private async void OnSelectLogin(bool isAnonymous)
    {
        panelSelectLogin.SetActive(false);
        loginPanel.SetActive(true);

        statusText.text = "Iniciando sesión...";

        try
        {
            if (isAnonymous)
                await AuthManager.Instance.SignInAnonymouslyAsync();
            else
                await AuthManager.Instance.SignInWithUnityAccountAsync();

            statusText.text = "Sesión iniciada";
        }
        catch (Exception e)
        {
            statusText.text = "Error al iniciar sesión";
            Debug.LogError(e);
        }
    }
    /*
    private async void SignIn(bool isAnonymous)
    {
        loginPanel.gameObject.SetActive(false);
       // userPanel.gameObject.SetActive(true);

        if (isAnonymous)
            await AuthManager.Instance.SignInAnonymouslyAsync();
        else
            await AuthManager.Instance.SignInWithUnityAccountAsync();
    }

   /* private void OnSignedIn(PlayerInfo playerInfo, string playerName)
    {
        playerIDTxt.text = $"ID: {playerInfo.Id}";
        playerNameTxt.text = playerName;
    }/*
    private async void UpdateName()
    {
        await unityPlayerAuth.UpdateName(UpdateNameIF.text);
    }
    private void UpdateNameVisual(string newName)
    {
        playerNameTxt.text = newName;
    }
    private void UnityPlayerOnSignedIn(PlayerInfo playerInfo, string PlayerName)
    {
        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);

        playerIDTxt.text = "ID: " + playerInfo.Id;
        playerNameTxt.text = PlayerName;
    }

    private async void LoginButton()
    {
        await unityPlayerAuth.InitSignIn();
    }*/
    
    private void OnPlayerSignedIn(Unity.Services.Authentication.PlayerInfo info, string name)
    {
    }
    private void OnDisable()
    {
        anonLoginButton.onClick.RemoveAllListeners();
        unityLoginButton.onClick.RemoveAllListeners();
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnSignedIn -= OnPlayerSignedIn;
        }
        //AuthManager.Instance.OnSignedIn -= OnSignedIn;
        /*
        loginButton?.onClick.RemoveListener(LoginButton);
        unityPlayerAuth.OnSingedIn -= UnityPlayerOnSignedIn;*/
    }
}
