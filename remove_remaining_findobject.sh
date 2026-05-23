#!/bin/bash

echo "🔍 Removing remaining FindObjectOfType calls..."

# Fix CachedReferences.cs
cat > Scripts/Core/CachedReferences.cs << 'EOF'
using UnityEngine;

public class CachedReferences : MonoBehaviour
{
    public static CachedReferences Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // All systems accessed through MasterGameManager singleton
    public static MasterGameManager GameManager => MasterGameManager.Instance;
    public static CoreResources Resources => GameManager?.Resources;
    public static CurrencyManager Currency => GameManager?.Currency;
    public static UIManager UI => GameManager?.UIManager;
    public static BuildingSystem Building => GameManager?.BuildingSystem;
    public static CombatSystem Combat => GameManager?.CombatSystem;
    public static PVERaidSystem PVERaid => GameManager?.PVERaid;
    public static ChestSystem Chest => GameManager?.ChestSystem;
    public static QuestManager Quests => GameManager?.QuestManager;
    public static TutorialSystem Tutorial => GameManager?.TutorialSystem;
    public static EnergySystem Energy => GameManager?.EnergySystem;
    public static AudioManager Audio => GameManager?.Audio;
    public static VisualManager Visual => GameManager?.Visual;
    public static VehicleSystem Vehicle => GameManager?.Vehicle;
}
