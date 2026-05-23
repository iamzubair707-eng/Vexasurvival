using UnityEngine;

public class MoralChoiceSystem : MonoBehaviour
{
    public int reputation = 0; // -100 to 100
    public string reputationTier = "Neutral";
    
    void Start()
    {
        LoadReputation();
    }
    
    public void MakeChoice(string choice)
    {
        switch (choice)
        {
            case "help_weak":
                reputation += 20;
                Debug.Log("🤝 You helped weak survivors. +20 Reputation");
                break;
            case "loot_them":
                reputation -= 30;
                ResourceManager resources = GetComponent<ResourceManager>();
                resources.AddResource("wood", 100);
                resources.AddResource("stone", 50);
                Debug.Log("💰 You looted survivors! +100 Wood, +50 Stone, -30 Reputation");
                break;
            case "ignore":
                reputation -= 10;
                Debug.Log("😐 You ignored them. -10 Reputation");
                break;
        }
        
        reputation = Mathf.Clamp(reputation, -100, 100);
        UpdateReputationTier();
        SaveReputation();
        
        // Trigger story events based on reputation
        TriggerStoryEvent();
    }
    
    void UpdateReputationTier()
    {
        if (reputation >= 80)
            reputationTier = "Hero";
        else if (reputation >= 50)
            reputationTier = "Trusted";
        else if (reputation >= 20)
            reputationTier = "Good";
        else if (reputation >= -20)
            reputationTier = "Neutral";
        else if (reputation >= -50)
            reputationTier = "Suspicious";
        else if (reputation >= -80)
            reputationTier = "Wanted";
        else
            reputationTier = "Outcast";
        
        Debug.Log($"🏷️ Reputation: {reputationTier} ({reputation})");
    }
    
    void TriggerStoryEvent()
    {
        if (reputationTier == "Hero")
        {
            Debug.Log("🌟 STORY: Survivors build a statue in your honor!");
            // Give rewards
        }
        else if (reputationTier == "Outcast")
        {
            Debug.Log("⚠️ STORY: You're banned from trading posts!");
            // Punishment
        }
    }
    
    void SaveReputation()
    {
        PlayerPrefs.SetInt("Reputation", reputation);
        PlayerPrefs.SetString("ReputationTier", reputationTier);
    }
    
    void LoadReputation()
    {
        reputation = PlayerPrefs.GetInt("Reputation", 0);
        reputationTier = PlayerPrefs.GetString("ReputationTier", "Neutral");
    }
}