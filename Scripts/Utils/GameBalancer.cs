using UnityEngine;
using System.Collections.Generic;

public class GameBalancer : MonoBehaviour
{
    public static GameBalancer Instance;
    
    [System.Serializable]
    public class BalanceSettings
    {
        public float resourceGainMultiplier = 1f;
        public float buildTimeMultiplier = 1f;
        public float raidDifficultyMultiplier = 1f;
        public float expGainMultiplier = 1f;
        public float coinGainMultiplier = 1f;
    }
    
    public BalanceSettings currentBalance = new BalanceSettings();
    public BalanceSettings defaultBalance = new BalanceSettings();
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        LoadBalanceSettings();
    }
    
    public int CalculateResourceGain(int baseGain)
    {
        return Mathf.RoundToInt(baseGain * currentBalance.resourceGainMultiplier);
    }
    
    public float CalculateBuildTime(float baseTime)
    {
        return baseTime * currentBalance.buildTimeMultiplier;
    }
    
    public int CalculateRaidReward(int baseReward)
    {
        // Risk vs reward: higher difficulty = higher reward
        float difficultyBonus = 1f + (currentBalance.raidDifficultyMultiplier - 1f) * 0.5f;
        return Mathf.RoundToInt(baseReward * difficultyBonus);
    }
    
    public void AdjustDifficulty(int playerLevel)
    {
        // Dynamic difficulty based on player level
        if (playerLevel < 5)
        {
            currentBalance.raidDifficultyMultiplier = 0.7f;
            currentBalance.resourceGainMultiplier = 1.3f;
        }
        else if (playerLevel < 15)
        {
            currentBalance.raidDifficultyMultiplier = 1f;
            currentBalance.resourceGainMultiplier = 1f;
        }
        else
        {
            currentBalance.raidDifficultyMultiplier = 1.3f;
            currentBalance.resourceGainMultiplier = 0.8f;
        }
        
        DebugLogger.Log($"⚖️ Difficulty adjusted for level {playerLevel}");
        SaveBalanceSettings();
    }
    
    public void ApplyGlobalEventMultiplier(float multiplier)
    {
        currentBalance.resourceGainMultiplier = multiplier;
        SaveBalanceSettings();
    }
    
    void SaveBalanceSettings()
    {
        PlayerPrefs.SetFloat("ResourceMultiplier", currentBalance.resourceGainMultiplier);
        PlayerPrefs.SetFloat("RaidDifficulty", currentBalance.raidDifficultyMultiplier);
    }
    
    void LoadBalanceSettings()
    {
        currentBalance.resourceGainMultiplier = PlayerPrefs.GetFloat("ResourceMultiplier", 1f);
        currentBalance.raidDifficultyMultiplier = PlayerPrefs.GetFloat("RaidDifficulty", 1f);
    }
}