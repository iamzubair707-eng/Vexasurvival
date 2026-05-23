using UnityEngine;
using System.Collections.Generic;

public class BattlePassManager : MonoBehaviour
{
    private ResourceManager resourceManager;
    private NotificationManager notificationManager;
    
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    
    public List<BattlePassReward> freeRewards = new List<BattlePassReward>();
    public List<BattlePassReward> premiumRewards = new List<BattlePassReward>();
    
    public bool hasPremiumPass = false;
    
    void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        notificationManager = GetComponent<NotificationManager>();
        
        LoadBattlePassData();
        InitializeRewards();
    }
    
    void InitializeRewards()
    {
        freeRewards.Clear();
        premiumRewards.Clear();
        
        for (int i = 1; i <= 50; i++)
        {
            // Free rewards every level
            freeRewards.Add(new BattlePassReward(i, "vexa", i * 2));
            
            // Premium rewards every level (better items)
            if (i % 5 == 0)
            {
                premiumRewards.Add(new BattlePassReward(i, "rare_blueprint", 1));
            }
            else
            {
                premiumRewards.Add(new BattlePassReward(i, "vexa", i * 5));
            }
        }
    }
    
    public void AddXP(int amount)
    {
        if (currentLevel >= 50)
        {
            DebugLogger.Log("Battle Pass already at max level!");
            return;
        }
        
        currentXP += amount;
        
        if (notificationManager != null)
        {
            notificationManager.ShowNotification($"🎖️ +{amount} Battle Pass XP!", "info");
        }
        
        while (currentXP >= xpToNextLevel && currentLevel < 50)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            
            // Claim rewards for new level
            ClaimLevelRewards(currentLevel);
            
            // Show level up notification
            if (notificationManager != null)
            {
                notificationManager.ShowNotification($"🎉 BATTLE PASS LEVEL {currentLevel} REACHED! 🎉", "success");
            }
            
            DebugLogger.Log($"🏆 Battle Pass Level Up! Now Level {currentLevel}");
            
            xpToNextLevel = CalculateNextLevelXP();
        }
        
        SaveBattlePassData();
    }
    
    int CalculateNextLevelXP()
    {
        // Increase XP needed each level
        return 100 + (currentLevel * 10);
    }
    
    void ClaimLevelRewards(int level)
    {
        // Claim free reward
        BattlePassReward freeReward = freeRewards.Find(r => r.level == level);
        if (freeReward != null && !freeReward.isClaimed)
        {
            GiveReward(freeReward);
            freeReward.isClaimed = true;
            DebugLogger.Log($"🎁 Free reward claimed for level {level}!");
        }
        
        // Claim premium reward (if player has premium pass)
        if (hasPremiumPass)
        {
            BattlePassReward premiumReward = premiumRewards.Find(r => r.level == level);
            if (premiumReward != null && !premiumReward.isClaimed)
            {
                GiveReward(premiumReward);
                premiumReward.isClaimed = true;
                DebugLogger.Log($"💎 Premium reward claimed for level {level}!");
            }
        }
    }
    
    void GiveReward(BattlePassReward reward)
    {
        switch (reward.rewardType)
        {
            case "vexa":
                resourceManager.AddResource("vexa", reward.amount);
                DebugLogger.Log($"💰 +{reward.amount} VEXA from Battle Pass!");
                break;
                
            case "wood":
                resourceManager.AddResource("wood", reward.amount);
                break;
                
            case "stone":
                resourceManager.AddResource("stone", reward.amount);
                break;
                
            case "rare_blueprint":
                PlayerPrefs.SetInt("HasRareBlueprint", 1);
                DebugLogger.Log("📜 RARE BLUEPRINT UNLOCKED!");
                break;
                
            case "shield":
                PlayerPrefs.SetInt("ShieldActive", 1);
                PlayerPrefs.SetFloat("ShieldEndTime", Time.time + 86400);
                DebugLogger.Log("🛡️ 24-HOUR SHIELD ACTIVATED!");
                break;
        }
    }
    
    public void PurchasePremiumPass()
    {
        if (hasPremiumPass)
        {
            DebugLogger.Log("Already have premium pass!");
            return;
        }
        
        // Cost 100 VEXA for premium pass
        if (resourceManager.SpendResource("vexa", 100))
        {
            hasPremiumPass = true;
            
            // Claim all previous level premium rewards
            for (int i = 1; i <= currentLevel; i++)
            {
                BattlePassReward premiumReward = premiumRewards.Find(r => r.level == i);
                if (premiumReward != null && !premiumReward.isClaimed)
                {
                    GiveReward(premiumReward);
                    premiumReward.isClaimed = true;
                }
            }
            
            if (notificationManager != null)
            {
                notificationManager.ShowNotification("💎 PREMIUM BATTLE PASS UNLOCKED! 💎", "success");
            }
            
            DebugLogger.Log("Premium Battle Pass purchased! All past rewards claimed!");
        }
        else
        {
            DebugLogger.Log("Not enough VEXA to purchase premium pass! Need 100 VEXA");
        }
        
        SaveBattlePassData();
    }
    
    public void ShowBattlePassUI()
    {
        DebugLogger.Log("========== BATTLE PASS ==========");
        DebugLogger.Log($"Level: {currentLevel}/50");
        DebugLogger.Log($"XP: {currentXP}/{xpToNextLevel}");
        DebugLogger.Log($"Premium: {(hasPremiumPass ? "✅ ACTIVE" : "❌ INACTIVE (100 VEXA to unlock)")}");
        DebugLogger.Log("================================");
        
        // Show next 5 rewards
        DebugLogger.Log("--- UPCOMING REWARDS ---");
        for (int i = currentLevel; i <= Mathf.Min(currentLevel + 5, 50); i++)
        {
            BattlePassReward free = freeRewards.Find(r => r.level == i);
            if (free != null)
            {
                DebugLogger.Log($"Level {i}: 🆓 +{free.amount} VEXA");
                
                if (hasPremiumPass || i <= currentLevel)
                {
                    BattlePassReward premium = premiumRewards.Find(r => r.level == i);
                    if (premium != null)
                    {
                        DebugLogger.Log($"        💎 +{premium.amount} VEXA");
                    }
                }
            }
        }
        DebugLogger.Log("================================");
    }
    
    void SaveBattlePassData()
    {
        PlayerPrefs.SetInt("BattlePassLevel", currentLevel);
        PlayerPrefs.SetInt("BattlePassXP", currentXP);
        PlayerPrefs.SetInt("BattlePassPremium", hasPremiumPass ? 1 : 0);
        
        // Save claimed rewards
        for (int i = 1; i <= 50; i++)
        {
            BattlePassReward free = freeRewards.Find(r => r.level == i);
            if (free != null)
                PlayerPrefs.SetInt($"BP_Free_{i}", free.isClaimed ? 1 : 0);
            
            BattlePassReward premium = premiumRewards.Find(r => r.level == i);
            if (premium != null)
                PlayerPrefs.SetInt($"BP_Premium_{i}", premium.isClaimed ? 1 : 0);
        }
    }
    
    void LoadBattlePassData()
    {
        currentLevel = PlayerPrefs.GetInt("BattlePassLevel", 1);
        currentXP = PlayerPrefs.GetInt("BattlePassXP", 0);
        hasPremiumPass = PlayerPrefs.GetInt("BattlePassPremium", 0) == 1;
        
        // Load claimed rewards
        foreach (BattlePassReward reward in freeRewards)
        {
            reward.isClaimed = PlayerPrefs.GetInt($"BP_Free_{reward.level}", 0) == 1;
        }
        
        foreach (BattlePassReward reward in premiumRewards)
        {
            reward.isClaimed = PlayerPrefs.GetInt($"BP_Premium_{reward.level}", 0) == 1;
        }
    }
}

[System.Serializable]
public class BattlePassReward
{
    public int level;
    public string rewardType;
    public int amount;
    public bool isClaimed;
    
    public BattlePassReward(int level, string type, int amount)
    {
        this.level = level;
        this.rewardType = type;
        this.amount = amount;
        this.isClaimed = false;
    }
}