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

    private const string PLAYER_NAME_KEY = "PLAYER_NAME";
    private const string PLAYER_LEVEL_KEY = "PLAYER_LEVEL";
    private const string PLAYER_EXP_KEY = "PLAYER_EXP";
    private const string PLAYER_SKILL_KEY = "PLAYER_SKILL_POINTS";
    private const string PLAYER_FIGHT_KEY = "PLAYER_FIGHT";
    private const string PLAYER_DEFENSE_KEY = "PLAYER_DEFENSE";
    private const string PLAYER_MAGIC_KEY = "PLAYER_MAGIC";
    private const string PLAYER_ITEM_KEY = "PLAYER_ITEM";



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
            var keys = new HashSet<string>
            {
                PLAYER_NAME_KEY, PLAYER_LEVEL_KEY, PLAYER_EXP_KEY,
                PLAYER_SKILL_KEY, PLAYER_FIGHT_KEY, PLAYER_DEFENSE_KEY,
                PLAYER_MAGIC_KEY, PLAYER_ITEM_KEY
            };
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            CurrentData = new PlayerData
            {
                playerName = data.ContainsKey(PLAYER_NAME_KEY) ? data[PLAYER_NAME_KEY].Value.GetAs<string>() : "NuevoJugador",
                level = data.ContainsKey(PLAYER_LEVEL_KEY) ? data[PLAYER_LEVEL_KEY].Value.GetAs<int>() : 1,
                experience = data.ContainsKey(PLAYER_EXP_KEY) ? data[PLAYER_EXP_KEY].Value.GetAs<int>() : 0,
                skillPoints = data.ContainsKey(PLAYER_SKILL_KEY) ? data[PLAYER_SKILL_KEY].Value.GetAs<int>() : 0,
                fight = data.ContainsKey(PLAYER_FIGHT_KEY) ? data[PLAYER_FIGHT_KEY].Value.GetAs<int>() : 1,
                defense = data.ContainsKey(PLAYER_DEFENSE_KEY) ? data[PLAYER_DEFENSE_KEY].Value.GetAs<int>() : 1,
                magic = data.ContainsKey(PLAYER_MAGIC_KEY) ? data[PLAYER_MAGIC_KEY].Value.GetAs<int>() : 1,
                item = data.ContainsKey(PLAYER_ITEM_KEY) ? data[PLAYER_ITEM_KEY].Value.GetAs<int>() : 1
            };
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
        var data = new Dictionary<string, object>
        {
            { PLAYER_NAME_KEY, CurrentData.playerName },
            { PLAYER_LEVEL_KEY, CurrentData.level },
            { PLAYER_EXP_KEY, CurrentData.experience },
            { PLAYER_SKILL_KEY, CurrentData.skillPoints },
            { PLAYER_FIGHT_KEY, CurrentData.fight },
            { PLAYER_DEFENSE_KEY, CurrentData.defense },
            { PLAYER_MAGIC_KEY, CurrentData.magic },
            { PLAYER_ITEM_KEY, CurrentData.item }
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        Debug.Log("Datos guardados.");
    }
}
