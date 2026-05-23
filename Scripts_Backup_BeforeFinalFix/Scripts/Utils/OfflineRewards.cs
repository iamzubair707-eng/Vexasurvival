using UnityEngine;
using System;

public class OfflineRewards : MonoBehaviour
{
    private DateTime lastOnlineTime;
    private ResourceManager resourceManager;
    private CurrencyManager currencyManager;
    
    void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        currencyManager = GetComponent<CurrencyManager>();
        LoadOfflineTime();
    }
    
    void OnApplicationQuit()
    {
        SaveOfflineTime();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveOfflineTime();
        else
            CalculateOfflineRewards();
    }
    
    void CalculateOfflineRewards()
    {
        TimeSpan offlineDuration = DateTime.Now - lastOnlineTime;
        int hoursOffline = Mathf.Min((int)offlineDuration.TotalHours, 12); // Max 12 hours
        
        if (hoursOffline > 0)
        {
            int woodReward = hoursOffline * 50;
            int stoneReward = hoursOffline * 30;
            int foodReward = hoursOffline * 40;
            int coinReward = hoursOffline * 100;
            
            resourceManager.AddResource("wood", woodReward);
            resourceManager.AddResource("stone", stoneReward);
            resourceManager.AddResource("food", foodReward);
            currencyManager.AddCoins(coinReward);
            
            DebugLogger.Log($"🎁 Offline Rewards ({hoursOffline} hours): +{woodReward} Wood, +{stoneReward} Stone, +{foodReward} Food, +{coinReward} Coins");
            
            NotificationManager notif = GetComponent<NotificationManager>();
            if (notif != null)
                notif.ShowNotification($"🎁 Welcome back! You earned offline rewards!", "success");
        }
        
        lastOnlineTime = DateTime.Now;
        SaveOfflineTime();
    }
    
    void SaveOfflineTime()
    {
        PlayerPrefs.SetString("LastOnline", lastOnlineTime.ToString());
    }
    
    void LoadOfflineTime()
    {
        string savedTime = PlayerPrefs.GetString("LastOnline", DateTime.Now.ToString());
        lastOnlineTime = DateTime.Parse(savedTime);
    }
}