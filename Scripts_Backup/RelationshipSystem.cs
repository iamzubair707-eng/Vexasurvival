using UnityEngine;
using System.Collections.Generic;

public class RelationshipSystem : MonoBehaviour
{
    public List<SurvivorRelationship> survivors = new List<SurvivorRelationship>();
    
    [System.Serializable]
    public class SurvivorRelationship
    {
        public string name;
        public float relationshipValue; // -100 to 100
        public RelationshipType type;
        public float productivityBonus;
    }
    
    public enum RelationshipType
    {
        Neutral,
        Love,      // +50% productivity
        Rivalry,   // -30% productivity
        Betrayal   // Can steal resources
    }
    
    void Start()
    {
        InitializeSurvivors();
        InvokeRepeating("UpdateRelationships", 300f, 300f);
    }
    
    void InitializeSurvivors()
    {
        survivors.Add(new SurvivorRelationship { name = "Sarah", relationshipValue = 50, type = RelationshipType.Neutral, productivityBonus = 0 });
        survivors.Add(new SurvivorRelationship { name = "Marcus", relationshipValue = 20, type = RelationshipType.Neutral, productivityBonus = 0 });
        survivors.Add(new SurvivorRelationship { name = "Elena", relationshipValue = -30, type = RelationshipType.Rivalry, productivityBonus = -0.3f });
    }
    
    public void Interact(string survivorName, string action)
    {
        SurvivorRelationship survivor = survivors.Find(s => s.name == survivorName);
        
        switch (action)
        {
            case "help":
                survivor.relationshipValue += 10;
                Debug.Log($"🤝 You helped {survivorName}. Relationship: {survivor.relationshipValue}");
                break;
            case "betray":
                survivor.relationshipValue -= 30;
                if (survivor.relationshipValue < -70)
                {
                    survivor.type = RelationshipType.Betrayal;
                    Debug.Log($"⚠️ {survivorName} BETRAYED YOU! They stole resources!");
                    StealResources();
                }
                break;
            case "gift":
                survivor.relationshipValue += 20;
                if (survivor.relationshipValue > 80 && survivor.type != RelationshipType.Love)
                {
                    survivor.type = RelationshipType.Love;
                    survivor.productivityBonus = 0.5f;
                    Debug.Log($"💕 {survivorName} fell in love! +50% productivity!");
                }
                break;
        }
        
        survivor.relationshipValue = Mathf.Clamp(survivor.relationshipValue, -100, 100);
        UpdateProductivity(survivor);
    }
    
    void UpdateRelationships()
    {
        foreach (var survivor in survivors)
        {
            if (survivor.type == RelationshipType.Rivalry && survivor.relationshipValue > 0)
            {
                survivor.type = RelationshipType.Neutral;
                Debug.Log($"🤝 {survivor.name} rivalry ended!");
            }
        }
    }
    
    void UpdateProductivity(SurvivorRelationship survivor)
    {
        ResourceManager resources = GetComponent<ResourceManager>();
        if (survivor.productivityBonus > 0)
        {
            Debug.Log($"📈 {survivor.name} productivity: +{survivor.productivityBonus * 100}%");
        }
    }
    
    void StealResources()
    {
        ResourceManager resources = GetComponent<ResourceManager>();
        resources.SpendResource("wood", 50);
        resources.SpendResource("stone", 30);
        Debug.Log("💔 Resources stolen by traitor!");
    }
}