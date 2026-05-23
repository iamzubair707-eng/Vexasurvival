using UnityEngine;
using System;
using System.Collections.Generic;

public class DeadlineSystem : MonoBehaviour
{
    public static DeadlineSystem Instance;
    
    public List<TimeLimitedEvent> activeEvents = new List<TimeLimitedEvent>();
    public List<TimeLimitedEvent> completedEvents = new List<TimeLimitedEvent>();
    
    [System.Serializable]
    public class TimeLimitedEvent
    {
        public string eventId;
        public string eventName;
        public string description;
        public DateTime startTime;
        public DateTime endTime;
        public bool isActive;
        public bool isCompleted;
        public RewardData reward;
        
        public TimeSpan GetRemainingTime()
        {
            return endTime - DateTime.Now;
        }
        
        public float GetProgressPercentage()
        {
            double totalDuration = (endTime - startTime).TotalSeconds;
            double elapsed = (DateTime.Now - startTime).TotalSeconds;
            return Mathf.Clamp01((float)(elapsed / totalDuration)) * 100f;
        }
    }
    
    [System.Serializable]
    public class RewardData
    {
        public string rewardType; // coins, gems, costume, slave, special
        public int amount;
        public string itemName;
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        InitializeDeadlines();
    }
    
    void Start()
    {
        InvokeRepeating("CheckDeadlines", 0f, 60f); // Check every minute
    }
    
    void InitializeDeadlines()
    {
        // Daily events
        activeEvents.Add(new TimeLimitedEvent
        {
            eventId = "daily_login",
            eventName = "🔥 DAILY LOGIN BONUS 🔥",
            description = "Log in every day for massive rewards!",
            startTime = DateTime.Now.Date,
            endTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
            isActive = true,
            reward = new RewardData { rewardType = "coins", amount = 500, itemName = "" }
        });
        
        // 3-hour chest reminder
        activeEvents.Add(new TimeLimitedEvent
        {
            eventId = "chest_reminder",
            eventName = "🎁 FREE CHEST READY! 🎁",
            description = "Your free chest is waiting! Claim in next 30 minutes!",
            startTime = DateTime.Now,
            endTime = DateTime.Now.AddMinutes(30),
            isActive = true,
            reward = new RewardData { rewardType = "chest", amount = 1, itemName = "free_chest" }
        });
        
        // Weekly special
        DateTime weeklyEnd = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek);
        activeEvents.Add(new TimeLimitedEvent
        {
            eventId = "weekly_bonus",
            eventName = "⭐ WEEKLY CHALLENGE ⭐",
            description = "Complete 10 raids this week!",
            startTime = DateTime.Now,
            endTime = weeklyEnd,
            isActive = true,
            reward = new RewardData { rewardType = "gems", amount = 100, itemName = "" }
        });
        
        // Flash sale (4 hours)
        activeEvents.Add(new TimeLimitedEvent
        {
            eventId = "flash_sale",
            eventName = "⚡ FLASH SALE! ⚡",
            description = "50% OFF on all costumes! Limited time!",
            startTime = DateTime.Now,
            endTime = DateTime.Now.AddHours(4),
            isActive = true,
            reward = new RewardData { rewardType = "discount", amount = 50, itemName = "costumes" }
        });
        
        // Special event (48 hours)
        activeEvents.Add(new TimeLimitedEvent
        {
            eventId = "double_xp",
            eventName = "✨ DOUBLE XP WEEKEND ✨",
            description = "Earn 2X XP from all activities!",
            startTime = DateTime.Now,
            endTime = DateTime.Now.AddHours(48),
            isActive = true,
            reward = new RewardData { rewardType = "boost", amount = 2, itemName = "xp_boost" }
        });
    }
    
    void CheckDeadlines()
    {
        for (int i = 0; i < activeEvents.Count; i++)
        {
            var evt = activeEvents[i];
            
            if (DateTime.Now >= evt.endTime)
            {
                DebugLogger.Log($"⏰ DEADLINE PASSED: {evt.eventName}");
                
                if (!evt.isCompleted)
                {
                    // Send notification that deadline passed
                    SendNotification($"⏰ {evt.eventName} has ended!", "Don't miss next one!");
                }
                
                evt.isActive = false;
                evt.isCompleted = true;
                completedEvents.Add(evt);
                activeEvents.RemoveAt(i);
                i--;
            }
            else if (evt.endTime - DateTime.Now <= TimeSpan.FromHours(1))
            {
                // 1 hour remaining - send urgent notification
                if (!PlayerPrefs.HasKey($"Notified_{evt.eventId}"))
                {
                    SendNotification($" URGENT: {evt.eventName}", $"Ends in {(evt.endTime - DateTime.Now).Hours}h {(evt.endTime - DateTime.Now).Minutes}m! Claim now!");
                    PlayerPrefs.SetInt($"Notified_{evt.eventId}", 1);
                }
            }
        }
        
        SaveDeadlineStatus();
    }
    
    public bool ClaimEventReward(string eventId)
    {
        TimeLimitedEvent evt = activeEvents.Find(e => e.eventId == eventId);
        
        if (evt == null)
        {
            DebugLogger.Log("❌ Event not found!");
            return false;
        }
        
        if (DateTime.Now > evt.endTime)
        {
            DebugLogger.Log($"❌ Event '{evt.eventName}' has expired!");
            return false;
        }
        
        if (evt.isCompleted)
        {
            DebugLogger.Log($"❌ Event '{evt.eventName}' already claimed!");
            return false;
        }
        
        // Claim reward
        ClaimReward(evt.reward);
        evt.isCompleted = true;
        evt.isActive = false;
        
        SendNotification($" CLAIMED: {evt.eventName}!", $"You got {GetRewardDescription(evt.reward)}!");
        
        activeEvents.Remove(evt);
        completedEvents.Add(evt);
        SaveDeadlineStatus();
        
        return true;
    }
    
    void ClaimReward(RewardData reward)
    {
        var currency = GetComponent<CurrencyManager>();
        
        switch (reward.rewardType)
        {
            case "coins":
                currency?.AddCoins(reward.amount);
                DebugLogger.Log($"💰 +{reward.amount} Coins from event!");
                break;
                
            case "gems":
                currency?.AddGems(reward.amount);
                DebugLogger.Log($"💎 +{reward.amount} Gems from event!");
                break;
                
            case "chest":
                var chest = GetComponent<ChestSystem>();
                if (chest != null)
                {
                    chest.chestCount++;
                    DebugLogger.Log($"🎁 +1 Free Chest!");
                }
                break;
                
            case "boost":
                DebugLogger.Log($"⚡ Boost activated: {reward.amount}X for 24 hours!");
                PlayerPrefs.SetInt($"Boost_active", 1);
                PlayerPrefs.SetFloat($"Boost_endTime", Time.time + 86400);
                break;
                
            case "discount":
                DebugLogger.Log($"💰 {reward.amount}% discount on {reward.itemName}!");
                PlayerPrefs.SetInt($"Discount_{reward.itemName}", reward.amount);
                PlayerPrefs.SetFloat($"Discount_endTime", Time.time + 86400);
                break;
        }
    }
    
    string GetRewardDescription(RewardData reward)
    {
        switch (reward.rewardType)
        {
            case "coins": return $"{reward.amount} Coins";
            case "gems": return $"{reward.amount} Gems";
            case "chest": return "Free Chest";
            case "boost": return $"{reward.amount}X Boost";
            case "discount": return $"{reward.amount}% Discount on {reward.itemName}";
            default: return "Special Reward";
        }
    }
    
    public void ShowActiveDeadlines()
    {
        DebugLogger.Log("");
        DebugLogger.Log("⏰ ACTIVE TIME-LIMITED EVENTS");
        DebugLogger.Log("");
        
        foreach (var evt in activeEvents)
        {
            TimeSpan remaining = evt.GetRemainingTime();
            DebugLogger.Log($"🔥 {evt.eventName}");
            DebugLogger.Log($"   📝 {evt.description}");
            DebugLogger.Log($"   ⏱️ Remaining: {remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s");
            DebugLogger.Log($"   🎁 Reward: {GetRewardDescription(evt.reward)}");
            DebugLogger.Log($"   📊 Progress: {evt.GetProgressPercentage():F0}%");
            DebugLogger.Log("");
        }
        
        if (activeEvents.Count == 0)
            DebugLogger.Log("No active events right now. Check back soon!");
    }
    
    void SendNotification(string title, string message)
    {
        DebugLogger.Log($"📱🔔 {title} - {message}");
        
        var notif = GetComponent<NotificationManager>();
        if (notif != null)
            notif.ShowNotification($"{title}: {message}", "urgent");
    }
    
    void SaveDeadlineStatus()
    {
        PlayerPrefs.SetInt("ActiveEventCount", activeEvents.Count);
        for (int i = 0; i < activeEvents.Count; i++)
        {
            PlayerPrefs.SetString($"Event_{i}_Id", activeEvents[i].eventId);
            PlayerPrefs.SetString($"Event_{i}_EndTime", activeEvents[i].endTime.ToString());
            PlayerPrefs.SetInt($"Event_{i}_Completed", activeEvents[i].isCompleted ? 1 : 0);
        }
    }
}