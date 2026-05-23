using System;
using UnityEngine;

public class DailyReward : MonoBehaviour
{
    private int currentStreak = 0;
    private string lastClaimDate = "";
    
    void Start()
    {
        LoadData();
        CheckAndReward();
    }
    
    void CheckAndReward()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        
        if (lastClaimDate == today)
        {
            Debug.Log("Already claimed today!");
            return;
        }
        
        // Streak logic
        DateTime lastDate = DateTime.Parse(lastClaimDate);
        DateTime nowDate = DateTime.Parse(today);
        
        if ((nowDate - lastDate).Days == 1)
        {
            currentStreak++;
        }
        else if ((nowDate - lastDate).Days > 1)
        {
            currentStreak = 1;
        }
        
        // Reward based on streak
        int reward = GetReward();
        Debug.Log($"Day {currentStreak} Reward: {reward} VEXA");
        
        lastClaimDate = today;
        SaveData();
    }
    
    int GetReward()
    {
        if (currentStreak >= 7) return 10;
        if (currentStreak >= 3) return 5;
        return 1;
    }
    
    void LoadData()
    {
        currentStreak = PlayerPrefs.GetInt("Streak", 1);
        lastClaimDate = PlayerPrefs.GetString("LastDate", "");
    }
    
    void SaveData()
    {
        PlayerPrefs.SetInt("Streak", currentStreak);
        PlayerPrefs.SetString("LastDate", lastClaimDate);
    }
}