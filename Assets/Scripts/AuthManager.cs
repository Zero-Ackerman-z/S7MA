using UnityEngine;

using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudSave;

using System.Collections.Generic;
using Sirenix.OdinInspector;
public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    public event Action<PlayerInfo, string> OnSignedIn;
    //public event Action OnSignedOut;

    public event Action<String> OnUpdateName;
    private PlayerInfo _playerInfo;
    private async void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
        await UnityServices.InitializeAsync();
        SetupEvents();
    }
/*    private async void Start()
    {
        await UnityServices.InitializeAsync();
        SetupEvents();
        PlayerAccountService.Instance.SignedIn += SignIn;
    }*/

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player ID " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Acces Token " + AuthenticationService.Instance.AccessToken);
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.Log(err);
        };
        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player log out");
        };
        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session expired");
        };
    }
    public async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await PostSignIn();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en SignInAnonymouslyAsync: {ex}");
        }
    }
    //->Lo puedes llamar a traves de un boton
    /*
    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }
    private async void SignIn()
    {
        try
        {
            await SignInWithUnityAuth();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }*/
    public async Task SignInWithUnityAccountAsync()
    {
        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
            string accessToken = PlayerAccountService.Instance.AccessToken;
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            await PostSignIn();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en SignInWithUnityAccountAsync: {ex}");
        }
    }
    private async Task PostSignIn()
    {
        //_playerInfo = AuthenticationService.Instance.PlayerInfo;
        //string name = await AuthenticationService.Instance.GetPlayerNameAsync();
        //OnSignedIn?.Invoke(_playerInfo, name);

        // Cargar o crear datos del jugador
        await PlayerDataHandler.Instance.LoadOrCreatePlayerData();

        // Redirigir a la escena de Menú
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
    // === Actualizar nombre ===
    public async Task UpdatePlayerName(string newName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            string updatedName = await AuthenticationService.Instance.GetPlayerNameAsync();
            OnUpdateName?.Invoke(updatedName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al actualizar nombre: {ex}");
        }
    }
    /*
    private async Task SignInWithUnityAuth()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("Login Succ");
            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            OnSingedIn?.Invoke(playerInfo, name);
            Debug.Log("Sign In Successful ");
        }
        catch (AuthenticationException ex)
        {   
            Debug.LogException(ex);
        }
        catch(RequestFailedException ex)
        {
            Debug.Log(ex);
        }
    }

    public async Task UpdateName(string newName)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
        var name = await AuthenticationService.Instance.GetPlayerNameAsync();

        OnUpdateName?.Invoke(name);
    }
    */
    public async Task DeleteAccountUnityAsync()
    {
        try
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }

    //-> Cloud Save


    [Button]
    public async void SaveData(string key , string value)
    {
        var playerData = new Dictionary<string, object>()
        {
            {key, value}
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
    }
    [Button]
    public async void LoadData(string key)
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
           new HashSet<string> { key } 
            );
        if(playerData.TryGetValue(key, out var value))
        {
            Debug.Log(key + " value : " + value.Value.GetAs<String>());
        }

    }
    [Button]
    public async void DeleteData(string key)
    {
        await CloudSaveService.Instance.Data.Player.DeleteAsync(key);
    }


    //
}
