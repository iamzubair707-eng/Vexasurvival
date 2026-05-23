using UnityEngine;
using System.Collections.Generic;

public class RelationshipManager : MonoBehaviour
{
    public List<Survivor> survivors = new List<Survivor>();
    
    [System.Serializable]
    public class Survivor
    {
        public string name;
        public float relationshipValue; // -100 to 100
        public RelationshipStatus status;
        public float productionBonus;
        
        public enum RelationshipStatus { Stranger, Friend, Love, Rival, Enemy }
    }
    
    void Start()
    {
        InitializeSurvivors();
        LoadRelationships();
    }
    
    void InitializeSurvivors()
    {
        survivors.Add(new Survivor { name = "Sarah", relationshipValue = 20, status = Survivor.RelationshipStatus.Stranger, productionBonus = 0 });
        survivors.Add(new Survivor { name = "Marcus", relationshipValue = -10, status = Survivor.RelationshipStatus.Stranger, productionBonus = 0 });
        survivors.Add(new Survivor { name = "Elena", relationshipValue = 50, status = Survivor.RelationshipStatus.Friend, productionBonus = 0.1f });
        survivors.Add(new Survivor { name = "John", relationshipValue = -50, status = Survivor.RelationshipStatus.Rival, productionBonus = -0.2f });
    }
    
    public void Interact(string survivorName, string action)
    {
        Survivor s = survivors.Find(sur => sur.name == survivorName);
        if (s == null) return;
        
        switch (action)
        {
            case "gift":
                s.relationshipValue += 15;
                DebugLogger.Log($"🎁 Gave gift to {survivorName}! +15 relationship");
                break;
            case "praise":
                s.relationshipValue += 10;
                DebugLogger.Log($"😊 Praised {survivorName}! +10 relationship");
                break;
            case "insult":
                s.relationshipValue -= 20;
                DebugLogger.Log($"😠 Insulted {survivorName}! -20 relationship");
                break;
            case "betray":
                s.relationshipValue -= 50;
                DebugLogger.Log($"💔 Betrayed {survivorName}! -50 relationship");
                TriggerBetrayal(s);
                break;
        }
        
        s.relationshipValue = Mathf.Clamp(s.relationshipValue, -100, 100);
        UpdateRelationshipStatus(s);
        ApplyRelationshipBonus(s);
        SaveRelationships();
    }
    
    void UpdateRelationshipStatus(Survivor s)
    {
        if (s.relationshipValue >= 80)
            s.status = Survivor.RelationshipStatus.Love;
        else if (s.relationshipValue >= 40)
            s.status = Survivor.RelationshipStatus.Friend;
        else if (s.relationshipValue >= -20)
            s.status = Survivor.RelationshipStatus.Stranger;
        else if (s.relationshipValue >= -60)
            s.status = Survivor.RelationshipStatus.Rival;
        else
            s.status = Survivor.RelationshipStatus.Enemy;
    }
    
    void ApplyRelationshipBonus(Survivor s)
    {
        switch (s.status)
        {
            case Survivor.RelationshipStatus.Love:
                s.productionBonus = 0.5f;
                DebugLogger.Log($"💕 {s.name} is in love! +50% production!");
                break;
            case Survivor.RelationshipStatus.Friend:
                s.productionBonus = 0.2f;
                break;
            case Survivor.RelationshipStatus.Rival:
                s.productionBonus = -0.3f;
                DebugLogger.Log($"⚔️ {s.name} is your rival! -30% production!");
                break;
            case Survivor.RelationshipStatus.Enemy:
                s.productionBonus = -0.5f;
                DebugLogger.Log($"💀 {s.name} is your enemy! -50% production!");
                break;
        }
    }
    
    void TriggerBetrayal(Survivor s)
    {
        CoreResources resources = GetComponent<CoreResources>();
        int stolenAmount = Random.Range(20, 50);
        resources.SpendResource("scrap", stolenAmount);
        DebugLogger.Log($" {s.name} stole {stolenAmount} scrap as revenge!");
    }
    
    void SaveRelationships()
    {
        for (int i = 0; i < survivors.Count; i++)
        {
            PlayerPrefs.SetFloat($"Rel_{survivors[i].name}", survivors[i].relationshipValue);
        }
    }
    
    void LoadRelationships()
    {
        foreach (Survivor s in survivors)
        {
            s.relationshipValue = PlayerPrefs.GetFloat($"Rel_{s.name}", s.relationshipValue);
            UpdateRelationshipStatus(s);
            ApplyRelationshipBonus(s);
        }
    }
}