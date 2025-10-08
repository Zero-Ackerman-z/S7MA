using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerStatsSystem : MonoBehaviour
{
    private PlayerData player => PlayerDataHandler.Instance.CurrentData;

    public async void GainExperience(int amount)
    {
        player.experience += amount;
        while (player.experience >= GetRequiredExpForNextLevel())
        {
            player.experience -= GetRequiredExpForNextLevel();
            player.level++;
            player.skillPoints += 20; 
            Debug.Log($"Subiste al nivel {player.level}");
        }

        await PlayerDataHandler.Instance.SavePlayerData();
    }

    private int GetRequiredExpForNextLevel()
    {
        int baseExp = 10;
        float growthFactor = 1.25f;
        return Mathf.RoundToInt(baseExp * Mathf.Pow(growthFactor, player.level - 1) * player.level);
    }
}
