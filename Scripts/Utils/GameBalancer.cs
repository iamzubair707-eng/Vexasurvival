using UnityEngine;

public class GameBalancer : MonoBehaviour
{
    public static GameBalancer Instance;
    
    [Header("Resource Balance")]
    public float resourceGainMultiplier = 1f;
    public int baseGatherAmount = 10;
    public int buildTimeReduction = 0;
    
    [Header("Combat Balance")]
    public float raidDifficultyMultiplier = 1f;
    public int baseTroopPower = 20;
    public int baseEnemyPower = 30;
    
    [Header("Economy Balance")]
    public int upgradeBaseCost = 50;
    public int troopTrainingCost = 30;
    public int raidRewardBase = 30;
    
    [Header("Time Balance")]
    public float chestCooldownHours = 3f;
    public float energyRegenMinutes = 30f;
    public int maxOfflineHours = 12;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadBalanceSettings();
    }
    
    public int CalculateGatherAmount(string resourceType)
    {
        int baseAmount = baseGatherAmount;
        
        // Bonus from building level
        int buildingLevel = MasterGameManager.Instance?.GetBuildingLevel() ?? 1;
        float buildingBonus = 1f + (buildingLevel - 1) * 0.2f;
        
        // Mental health penalty
        float mentalPenalty = GetMentalHealthPenalty();
        
        float finalAmount = baseAmount * resourceGainMultiplier * buildingBonus * mentalPenalty;
        
        return Mathf.Max(1, Mathf.RoundToInt(finalAmount));
    }
    
    public int CalculateUpgradeCost()
    {
        int currentLevel = MasterGameManager.Instance?.GetBuildingLevel() ?? 1;
        int cost = upgradeBaseCost * currentLevel;
        
        // Apply event discounts
        float discount = GetActiveDiscount();
        cost = Mathf.RoundToInt(cost * discount);
        
        return Mathf.Max(10, cost);
    }
    
    public int CalculateRaidPower(int troopCount, int buildingLevel)
    {
        int troopPower = troopCount * baseTroopPower;
        int buildingBonus = buildingLevel * 10;
        int vehicleBonus = MasterGameManager.Instance?.Vehicle?.GetAttackBonus() ?? 0;
        
        int totalPower = troopPower + buildingBonus + vehicleBonus;
        
        // Apply mental health penalty
        float mentalPenalty = GetMentalHealthPenalty();
        totalPower = Mathf.RoundToInt(totalPower * mentalPenalty);
        
        return totalPower;
    }
    
    public int CalculateEnemyPower()
    {
        int playerLevel = MasterGameManager.Instance?.GetPlayerLevel() ?? 1;
        float difficulty = raidDifficultyMultiplier;
        
        int enemyPower = Mathf.RoundToInt((baseEnemyPower + playerLevel * 5) * difficulty);
        
        return enemyPower;
    }
    
    public int CalculateRaidReward(bool isVictory, int playerPower, int enemyPower)
    {
        if (!isVictory) return 5; // Consolation reward
        
        int baseReward = raidRewardBase;
        float powerRatio = Mathf.Clamp((float)playerPower / enemyPower, 0.5f, 2f);
        
        int reward = Mathf.RoundToInt(baseReward * powerRatio);
        
        // Apply event multipliers
        reward = Mathf.RoundToInt(reward * GetActiveRewardMultiplier());
        
        return reward;
    }
    
    public float GetBuildTimeReduction()
    {
        int buildingLevel = MasterGameManager.Instance?.GetBuildingLevel() ?? 1;
        float reduction = 1f - (buildingLevel - 1) * 0.05f;
        return Mathf.Clamp(reduction, 0.5f, 1f);
    }
    
    float GetMentalHealthPenalty()
    {
        var mental = MasterGameManager.Instance?.GetComponent<MentalHealthSystem>();
        if (mental == null) return 1f;
        
        switch (mental.currentState)
        {
            case MentalHealthSystem.MentalState.Stressed: return 0.8f;
            case MentalHealthSystem.MentalState.Depressed: return 0.5f;
            case MentalHealthSystem.MentalState.Rebellious: return 0.6f;
            case MentalHealthSystem.MentalState.Insane: return 0.3f;
            default: return 1f;
        }
    }
    
    float GetActiveDiscount()
    {
        // Check for active events
        if (PlayerPrefs.GetInt("DiscountActive", 0) == 1)
            return 0.8f; // 20% discount
        return 1f;
    }
    
    float GetActiveRewardMultiplier()
    {
        if (PlayerPrefs.GetInt("DoubleRewardActive", 0) == 1)
            return 2f;
        return 1f;
    }
    
    public void SetDifficulty( float multiplier)
    {
        raidDifficultyMultiplier = multiplier;
        SaveBalanceSettings();
        DebugLogger.Log($"⚖️ Difficulty set to: {multiplier}x");
    }
    
    void SaveBalanceSettings()
    {
        PlayerPrefs.SetFloat("RaidDifficulty", raidDifficultyMultiplier);
        PlayerPrefs.SetFloat("ResourceMultiplier", resourceGainMultiplier);
    }
    
    void LoadBalanceSettings()
    {
        raidDifficultyMultiplier = PlayerPrefs.GetFloat("RaidDifficulty", 1f);
        resourceGainMultiplier = PlayerPrefs.GetFloat("ResourceMultiplier", 1f);
    }
    
    // Difficulty presets
    public void SetEasyMode()
    {
        resourceGainMultiplier = 1.5f;
        raidDifficultyMultiplier = 0.7f;
        upgradeBaseCost = 40;
        SaveBalanceSettings();
        DebugLogger.Log("⭐ Easy mode activated!");
    }
    
    public void SetNormalMode()
    {
        resourceGainMultiplier = 1f;
        raidDifficultyMultiplier = 1f;
        upgradeBaseCost = 50;
        SaveBalanceSettings();
        DebugLogger.Log("⭐ Normal mode activated!");
    }
    
    public void SetHardMode()
    {
        resourceGainMultiplier = 0.7f;
        raidDifficultyMultiplier = 1.5f;
        upgradeBaseCost = 70;
        SaveBalanceSettings();
        DebugLogger.Log("⭐ Hard mode activated!");
    }
}
