using UnityEngine;
using System.Collections.Generic;

public class MoralChoiceManager : MonoBehaviour
{
    public int moralityPoints = 0; // -100 to 100
    public string moralityTier = "Neutral";
    public List<MoralEvent> eventHistory = new List<MoralEvent>();
    
    [System.Serializable]
    public class MoralEvent
    {
        public string eventName;
        public int moralityChange;
        public string consequence;
        public bool isResolved;
    }
    
    void Start()
    {
        LoadMorality();
    }
    
    public void PresentChoice(MoralChoice choice)
    {
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"⚖️ {choice.question}");
        Debug.Log($"1️⃣ {choice.option1} (+{choice.moralityGain1} morality)");
        Debug.Log($"2️⃣ {choice.option2} ({choice.moralityGain2} morality)");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
    
    public void MakeChoice(int choiceIndex, MoralChoice choice)
    {
        int gain = (choiceIndex == 1) ? choice.moralityGain1 : choice.moralityGain2;
        moralityPoints += gain;
        moralityPoints = Mathf.Clamp(moralityPoints, -100, 100);
        
        UpdateMoralityTier();
        ApplyLongTermEffect(gain);
        SaveMorality();
        
        Debug.Log($"✅ Morality changed by {gain}. Now: {moralityPoints} ({moralityTier})");
    }
    
    void UpdateMoralityTier()
    {
        if (moralityPoints >= 80)
            moralityTier = "Saint";
        else if (moralityPoints >= 50)
            moralityTier = "Hero";
        else if (moralityPoints >= 20)
            moralityTier = "Good";
        else if (moralityPoints >= -20)
            moralityTier = "Neutral";
        else if (moralityPoints >= -50)
            moralityTier = "Suspicious";
        else if (moralityPoints >= -80)
            moralityTier = "Villain";
        else
            moralityTier = "Demon";
    }
    
    void ApplyLongTermEffect(int moralityChange)
    {
        CurrencyManager currency = GetComponent<CurrencyManager>();
        
        if (moralityTier == "Saint")
        {
            Debug.Log("🌟 Survivors worship you! +50% production!");
        }
        else if (moralityTier == "Demon")
        {
            Debug.Log("💀 Everyone fears you! Raid damage +30%, but traders avoid you!");
        }
        
        // Trigger reputation-based events
        if (Mathf.Abs(moralityPoints) > 70 && Random.Range(0, 100) < 30)
        {
            TriggerSpecialEvent();
        }
    }
    
    void TriggerSpecialEvent()
    {
        if (moralityTier == "Saint")
        {
            Debug.Log("🎁 A mysterious merchant offers you rare items!");
        }
        else if (moralityTier == "Demon")
        {
            Debug.Log("⚔️ Bounty hunters are tracking you!");
            // Trigger attack
        }
    }
    
    void SaveMorality()
    {
        PlayerPrefs.SetInt("MoralityPoints", moralityPoints);
        PlayerPrefs.SetString("MoralityTier", moralityTier);
    }
    
    void LoadMorality()
    {
        moralityPoints = PlayerPrefs.GetInt("MoralityPoints", 0);
        moralityTier = PlayerPrefs.GetString("MoralityTier", "Neutral");
    }
}

[System.Serializable]
public class MoralChoice
{
    public string question;
    public string option1;
    public string option2;
    public int moralityGain1;
    public int moralityGain2;
}