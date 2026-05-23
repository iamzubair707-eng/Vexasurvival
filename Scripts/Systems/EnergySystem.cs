using UnityEngine;
using System;

public class EnergySystem : MonoBehaviour
{
    public int currentEnergy = 100;
    public int maxEnergy = 100;
    public int energyRegenRate = 1; // per minute
    public DateTime lastRegenTime;
    
    void Start()
    {
        LoadEnergyData();
        InvokeRepeating("RegenerateEnergy", 60f, 60f); // Every minute
    }
    
    public bool ConsumeEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            SaveEnergyData();
            return true;
        }
        else
        {
            DebugLogger.Log($"❌ Not enough energy! Need {amount}, have {currentEnergy}");
            NotificationManager notif = GetComponent<NotificationManager>();
            if (notif != null)
                notif.ShowNotification("❌ Not enough energy! Wait or buy energy!", "warning");
            return false;
        }
    }
    
    void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            DebugLogger.Log($"⚡ Energy regenerated: {currentEnergy}/{maxEnergy}");
            SaveEnergyData();
        }
    }
    
    public void RefillEnergy()
    {
        currentEnergy = maxEnergy;
        SaveEnergyData();
    }
    
    public void BuyEnergy(int amount)
    {
        CurrencyManager currency = GetComponent<CurrencyManager>();
        int cost = amount * 10; // 10 gems per 10 energy
        
        if (currency.SpendGems(cost))
        {
            currentEnergy += amount;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            DebugLogger.Log($"⚡ Bought {amount} energy!");
            SaveEnergyData();
        }
    }
    
    void SaveEnergyData()
    {
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        PlayerPrefs.SetString("LastRegenTime", DateTime.Now.ToString());
    }
    
    void LoadEnergyData()
    {
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy", maxEnergy);
        string savedTime = PlayerPrefs.GetString("LastRegenTime", DateTime.Now.ToString());
        lastRegenTime = DateTime.Parse(savedTime);
        
        // Calculate energy regenerated while offline
        TimeSpan offline = DateTime.Now - lastRegenTime;
        int minutesOffline = (int)offline.TotalMinutes;
        int energyGain = minutesOffline * energyRegenRate;
        
        currentEnergy += energyGain;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
    }
}