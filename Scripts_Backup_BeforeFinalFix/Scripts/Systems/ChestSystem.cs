using UnityEngine;
using System;
using System.Collections.Generic;

public class ChestSystem : MonoBehaviour
{
    public static ChestSystem Instance;
    
    public int chestCount = 0;
    public DateTime nextChestTime;
    public bool isChestReady = true;
    
    public List<ChestReward> rewardHistory = new List<ChestReward>();
    
    [System.Serializable]
    public class ChestReward
    {
        public string rewardType;
        public int amount;
        public DateTime claimTime;
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        LoadChestData();
    }
    
    void Start()
    {
        CheckChestAvailability();
    }
    
    void CheckChestAvailability()
    {
        if (nextChestTime <= DateTime.Now)
        {
            isChestReady = true;
            chestCount++;
            SaveChestData();
            SendMobileNotification("🎁 Chest Ready!", "Your free chest is waiting! Claim now!");
            DebugLogger.Log("🎁 New chest available!");
        }
    }
    
    public ChestReward OpenChest()
    {
        if (chestCount <= 0)
        {
            DebugLogger.Log("No chests available!");
            return null;
        }
        
        chestCount--;
        ChestReward reward = GenerateReward();
        reward.claimTime = DateTime.Now;
        rewardHistory.Add(reward);
        
        // Set next chest time (3 hours from now)
        nextChestTime = DateTime.Now.AddHours(3);
        isChestReady = false;
        
        SaveChestData();
        SendMobileNotification("🎁 New Chest Incoming", "Next chest will be ready in 3 hours!");
        
        DebugLogger.Log($"🎁 Chest opened! Got: {reward.rewardType} x{reward.amount}");
        return reward;
    }
    
    ChestReward GenerateReward()
    {
        ChestReward reward = new ChestReward();
        int random = UnityEngine.Random.Range(0, 100);
        
        if (random < 40)  // 40% chance
        {
            reward.rewardType = "coins";
            reward.amount = UnityEngine.Random.Range(50, 200);
            CurrencyManager.Instance?.AddCoins(reward.amount);
        }
        else if (random < 65)  // 25% chance
        {
            reward.rewardType = "gems";
            reward.amount = UnityEngine.Random.Range(5, 25);
            CurrencyManager.Instance?.AddGems(reward.amount);
        }
        else if (random < 80)  // 15% chance
        {
            reward.rewardType = "costume";
            reward.amount = 1;
            UnlockCostume();
        }
        else if (random < 92)  // 12% chance
        {
            reward.rewardType = "slave";
            reward.amount = 1;
            UnlockSlave();
        }
        else  // 8% chance
        {
            reward.rewardType = "special";
            reward.amount = 1;
            UnlockSpecialItem();
        }
        
        return reward;
    }
    
    void UnlockCostume()
    {
        string[] costumes = { "Warrior Skin", "Shadow Cloak", "Golden Armor", "Ancient Robe" };
        string costume = costumes[UnityEngine.Random.Range(0, costumes.Length)];
        PlayerPrefs.SetInt($"Costume_{costume}", 1);
        DebugLogger.Log($"👔 New Costume Unlocked: {costume}!");
    }
    
    void UnlockSlave()
    {
        string[] slaves = { "Wood Gatherer", "Stone Miner", "Food Harvester", "Defense Guard" };
        string slave = slaves[UnityEngine.Random.Range(0, slaves.Length)];
        PlayerPrefs.SetInt($"Slave_{slave}", PlayerPrefs.GetInt($"Slave_{slave}", 0) + 1);
        DebugLogger.Log($"👥 New Worker Unlocked: {slave}!");
    }
    
    void UnlockSpecialItem()
    {
        string[] specials = { "Dragon Pet", "Magic Amulet", "Infinite Shield", "Double Damage" };
        string special = specials[UnityEngine.Random.Range(0, specials.Length)];
        PlayerPrefs.SetInt($"Special_{special}", 1);
        DebugLogger.Log($"✨ SPECIAL ITEM: {special}!");
    }
    
    public void ShowChestStatus()
    {
        DebugLogger.Log($"🎁 Chests Available: {chestCount}");
        if (!isChestReady)
        {
            TimeSpan remaining = nextChestTime - DateTime.Now;
            DebugLogger.Log($"⏰ Next chest in: {remaining.Hours}h {remaining.Minutes}m");
        }
    }
    
    void SendMobileNotification(string title, string message)
    {
        #if UNITY_ANDROID || UNITY_IOS
        // Mobile notification (will work when on actual device)
        DebugLogger.Log($"📱 NOTIFICATION: {title} - {message}");
        #endif
    }
    
    void SaveChestData()
    {
        PlayerPrefs.SetInt("ChestCount", chestCount);
        PlayerPrefs.SetString("NextChestTime", nextChestTime.ToString());
        PlayerPrefs.SetInt("IsChestReady", isChestReady ? 1 : 0);
    }
    
    void LoadChestData()
    {
        chestCount = PlayerPrefs.GetInt("ChestCount", 1);
        string savedTime = PlayerPrefs.GetString("NextChestTime", DateTime.Now.ToString());
        nextChestTime = DateTime.Parse(savedTime);
        isChestReady = PlayerPrefs.GetInt("IsChestReady", 1) == 1;
    }
}