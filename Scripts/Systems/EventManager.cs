using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
    private ResourceManager resourceManager;
    private NotificationManager notificationManager;
    
    public bool isDoubleRewardActive = false;
    public bool isHalfRaidCooldown = false;
    public bool isDiscountActive = false;
    
    private float eventEndTime = 0f;
    
    void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        notificationManager = GetComponent<NotificationManager>();
        
        StartCoroutine(EventScheduler());
        LoadEventStatus();
    }
    
    IEnumerator EventScheduler()
    {
        while (true)
        {
            // Check if current event expired
            if (eventEndTime > 0 && Time.time > eventEndTime)
            {
                EndAllEvents();
            }
            
            // Randomly start events (FOMO trigger)
            float randomCheck = Random.Range(0f, 100f);
            
            if (!isDoubleRewardActive && !isHalfRaidCooldown && !isDiscountActive)
            {
                if (randomCheck < 2f) // 2% chance every 30 seconds
                {
                    StartRandomEvent();
                }
            }
            
            yield return new WaitForSeconds(30f);
        }
    }
    
    void StartRandomEvent()
    {
        int eventType = Random.Range(0, 3);
        
        switch (eventType)
        {
            case 0:
                StartDoubleRewardEvent();
                break;
            case 1:
                StartHalfRaidCooldownEvent();
                break;
            case 2:
                StartDiscountEvent();
                break;
        }
    }
    
    public void StartDoubleRewardEvent()
    {
        isDoubleRewardActive = true;
        eventEndTime = Time.time + 1800f; // 30 minutes
        
        string msg = "🔥 DOUBLE REWARDS ACTIVE! 🔥\nAll resources doubled for 30 minutes!";
        
        if (notificationManager != null)
            notificationManager.ShowNotification(msg, "urgent");
        
        DebugLogger.Log(" DOUBLE REWARDS EVENT STARTED! (30 minutes)");
        
        PlayerPrefs.SetInt("DoubleReward", 1);
        PlayerPrefs.SetFloat("EventEndTime", eventEndTime);
    }
    
    public void StartHalfRaidCooldownEvent()
    {
        isHalfRaidCooldown = true;
        eventEndTime = Time.time + 1800f;
        
        string msg = "⚔️ RAID BOOST ACTIVE! ⚔️\nRaid cooldown reduced by 50% for 30 minutes!";
        
        if (notificationManager != null)
            notificationManager.ShowNotification(msg, "warning");
        
        DebugLogger.Log("⚔️ HALF RAID COOLDOWN EVENT STARTED! (30 minutes)");
        
        PlayerPrefs.SetInt("HalfRaidCooldown", 1);
        PlayerPrefs.SetFloat("EventEndTime", eventEndTime);
    }
    
    public void StartDiscountEvent()
    {
        isDiscountActive = true;
        eventEndTime = Time.time + 1800f;
        
        string msg = "💰 SHOP DISCOUNT! 💰\nAll shop items 20% off for 30 minutes!";
        
        if (notificationManager != null)
            notificationManager.ShowNotification(msg, "success");
        
        DebugLogger.Log("💰 DISCOUNT EVENT STARTED! (30 minutes)");
        
        PlayerPrefs.SetInt("Discount", 1);
        PlayerPrefs.SetFloat("EventEndTime", eventEndTime);
    }
    
    public void EndAllEvents()
    {
        isDoubleRewardActive = false;
        isHalfRaidCooldown = false;
        isDiscountActive = false;
        eventEndTime = 0f;
        
        PlayerPrefs.SetInt("DoubleReward", 0);
        PlayerPrefs.SetInt("HalfRaidCooldown", 0);
        PlayerPrefs.SetInt("Discount", 0);
        PlayerPrefs.SetFloat("EventEndTime", 0);
        
        if (notificationManager != null)
            notificationManager.ShowNotification("⏰ Events have ended! Wait for next one!", "info");
        
        DebugLogger.Log("All events ended!");
    }
    
    public int GetDiscountedPrice(int originalPrice)
    {
        if (isDiscountActive)
        {
            return Mathf.RoundToInt(originalPrice * 0.8f);
        }
        return originalPrice;
    }
    
    public float GetRaidCooldown(float originalCooldown)
    {
        if (isHalfRaidCooldown)
        {
            return originalCooldown / 2f;
        }
        return originalCooldown;
    }
    
    public int GetBonusReward(int originalReward)
    {
        if (isDoubleRewardActive)
        {
            return originalReward * 2;
        }
        return originalReward;
    }
    
    void LoadEventStatus()
    {
        isDoubleRewardActive = PlayerPrefs.GetInt("DoubleReward", 0) == 1;
        isHalfRaidCooldown = PlayerPrefs.GetInt("HalfRaidCooldown", 0) == 1;
        isDiscountActive = PlayerPrefs.GetInt("Discount", 0) == 1;
        eventEndTime = PlayerPrefs.GetFloat("EventEndTime", 0);
        
        if (eventEndTime > 0 && Time.time > eventEndTime)
        {
            EndAllEvents();
        }
    }
    
    public string GetActiveEventMessage()
    {
        if (isDoubleRewardActive) return "🔥 DOUBLE REWARDS!";
        if (isHalfRaidCooldown) return "⚔️ RAID BOOST!";
        if (isDiscountActive) return "💰 20% OFF!";
        return "No active event";
    }
}