using System.Collections;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;

[Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public int experience;
    public int skillPoints;
    public int fight;
    public int defense;
    public int magic;
    public int item;
}

public class PlayerDataHandler : MonoBehaviour
{
    public static PlayerDataHandler Instance { get; private set; }

    public PlayerData CurrentData { get; private set; }

    private const string PLAYER_DATA_KEY = "PLAYER_DATA";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    public async Task LoadOrCreatePlayerData()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { PLAYER_DATA_KEY });

            if (data.TryGetValue(PLAYER_DATA_KEY, out var savedData))
            {
                CurrentData = JsonUtility.FromJson<PlayerData>(savedData.Value.GetAs<string>());
                Debug.Log(" Datos cargados");
            }
            else
            {
                await CreateDefaultPlayerData();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"No se encontraron datos, creando nuevos: {ex.Message}");
            await CreateDefaultPlayerData();
        }
    }

    private async Task CreateDefaultPlayerData()
    {
        CurrentData = new PlayerData
        {
            playerName = "NuevoJugador",
            level = 1,
            experience = 0,
            skillPoints = 0,
            fight = 1,
            defense = 1,
            magic = 1,
            item = 1
        };

        await SavePlayerData();
    }

    public async Task SavePlayerData()
    {
        string json = JsonUtility.ToJson(CurrentData);
        var data = new Dictionary<string, object> { { PLAYER_DATA_KEY, json } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        Debug.Log("Datos guardados.");
    }
}
