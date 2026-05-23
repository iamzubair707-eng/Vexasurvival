using UnityEngine;
using System;

public class DailyLoginStreak : MonoBehaviour
{
    public int currentStreak = 0;
    public int bestStreak = 0;
    public DateTime lastLoginDate;
    
    void Start()
    {
        LoadStreakData();
        CheckDailyLogin();
    }
    
    void CheckDailyLogin()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string lastDate = PlayerPrefs.GetString("LastLoginDate", "");
        
        if (lastDate != today)
        {
            // New day!
            if (string.IsNullOrEmpty(lastDate))
            {
                // First login
                currentStreak = 1;
            }
            else
            {
                DateTime last = DateTime.Parse(lastDate);
                TimeSpan difference = DateTime.Now.Date - last.Date;
                
                if (difference.Days == 1)
                {
                    // Consecutive day
                    currentStreak++;
                    if (currentStreak > bestStreak)
                        bestStreak = currentStreak;
                }
                else if (difference.Days > 1)
                {
                    // Streak broken
                    currentStreak = 1;
                }
            }
            
            // Give streak reward
            GiveStreakReward();
            
            PlayerPrefs.SetString("LastLoginDate", today);
            SaveStreakData();
            
            DebugLogger.Log($"🔥 Daily Login Streak: {currentStreak} days! Best: {bestStreak}");
            
            // Send notification for next day
            SendStreakNotification();
        }
    }
    
    void GiveStreakReward()
    {
        var currency = GetComponent<CurrencyManager>();
        
        // Base reward
        int coinsReward = 50 + (currentStreak * 10);
        int gemsReward = 5 + (currentStreak / 7);
        
        currency?.AddCoins(coinsReward);
        
        if (gemsReward > 0)
            currency?.AddGems(gemsReward);
        
        // Special rewards for milestones
        if (currentStreak == 7)
        {
            DebugLogger.Log("🎉 7 DAY STREAK! +100 GEMS!");
            currency?.AddGems(100);
        }
        else if (currentStreak == 30)
        {
            DebugLogger.Log("🎉🎉 30 DAY STREAK! LEGENDARY CHEST!");
            var chest = GetComponent<ChestSystem>();
            if (chest != null) chest.chestCount += 3;
        }
        else if (currentStreak == 100)
        {
            DebugLogger.Log("🎉🎉🎉 100 DAY STREAK! ULTIMATE REWARD!");
            currency?.AddGems(1000);
        }
        
        DebugLogger.Log($"💰 Daily reward: +{coinsReward} coins, +{gemsReward} gems");
    }
    
    void SendStreakNotification()
    {
        DebugLogger.Log($"📱 Tomorrow: {currentStreak + 1} day streak awaits!");
    }
    
    void SaveStreakData()
    {
        PlayerPrefs.SetInt("CurrentStreak", currentStreak);
        PlayerPrefs.SetInt("BestStreak", bestStreak);
    }
    
    void LoadStreakData()
    {
        currentStreak = PlayerPrefs.GetInt("CurrentStreak", 0);
        bestStreak = PlayerPrefs.GetInt("BestStreak", 0);
    }
    
    public void ShowStreakInfo()
    {
        DebugLogger.Log($"🔥 Current Streak: {currentStreak} days");
        DebugLogger.Log($"🏆 Best Streak: {bestStreak} days");
        DebugLogger.Log($"⭐ Tomorrow's reward: {50 + ((currentStreak + 1) * 10)} coins");
    }
}