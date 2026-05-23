using UnityEngine;
using System.Collections;

public class PVERaidSystem : MonoBehaviour
{
    public enum RaidType { ZombieHorde, ScavengeRuins, BanditCamp }
    
    public int raidEnergy = 3;
    public int maxRaidEnergy = 5;
    
    void Start()
    {
        LoadRaidData();
        InvokeRepeating("RegenerateEnergy", 1800f, 1800f); // 30 minutes
    }
    
    public void StartRaid(RaidType type)
    {
        if (raidEnergy <= 0)
        {
            DebugLogger.Log("❌ No raid energy left! Wait 30 minutes.");
            return;
        }
        
        raidEnergy--;
        StartCoroutine(ExecuteRaid(type));
        SaveRaidData();
    }
    
    IEnumerator ExecuteRaid(RaidType type)
    {
        DebugLogger.Log($"⚔️ Starting {type} raid...");
        yield return new WaitForSeconds(2f);
        
        CoreResources resources = GetComponent<CoreResources>();
        CurrencyManager currency = GetComponent<CurrencyManager>();
        int rewardAmount = 0;
        
        switch (type)
        {
            case RaidType.ZombieHorde:
                rewardAmount = Random.Range(20, 50);
                resources.AddResource("scrap", rewardAmount);
                DebugLogger.Log($"🧟 Zombies defeated! +{rewardAmount} Scrap!");
                break;
                
            case RaidType.ScavengeRuins:
                rewardAmount = Random.Range(30, 80);
                resources.AddResource("food", rewardAmount / 2);
                resources.AddResource("water", rewardAmount / 2);
                DebugLogger.Log($"🏚️ Ruins scavenged! +{rewardAmount/2} Food, +{rewardAmount/2} Water!");
                break;
                
            case RaidType.BanditCamp:
                rewardAmount = Random.Range(10, 30);
                resources.AddResource("fuel", rewardAmount);
                currency.AddCoins(rewardAmount * 5);
                DebugLogger.Log($"💰 Bandits defeated! +{rewardAmount} Fuel, +{rewardAmount*5} Coins!");
                break;
        }
        
        // Update mental health
        MentalHealthSystem mental = GetComponent<MentalHealthSystem>();
        if (mental != null)
            mental.TriggerEvent("Won Raid");
    }
    
    void RegenerateEnergy()
    {
        if (raidEnergy < maxRaidEnergy)
        {
            raidEnergy++;
            DebugLogger.Log($"⚡ Raid energy regenerated! Now: {raidEnergy}/{maxRaidEnergy}");
        }
    }
    
    void SaveRaidData()
    {
        PlayerPrefs.SetInt("RaidEnergy", raidEnergy);
    }
    
    void LoadRaidData()
    {
        raidEnergy = PlayerPrefs.GetInt("RaidEnergy", maxRaidEnergy);
    }
}