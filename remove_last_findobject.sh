#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🔧 REMOVING LAST 4 FINDOBJECTOFTYPE CALLS"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Fix MasterGameManager.cs - remove FindObjectOfType from Instance getter
cat > Scripts/Core/MasterGameManager.cs << 'EOF'
using UnityEngine;
using System.Collections;

public class MasterGameManager : MonoBehaviour
{
    private static MasterGameManager _instance;
    public static MasterGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Use GameObject.Find instead of FindObjectOfType (faster)
                GameObject gm = GameObject.Find("MasterGameManager");
                if (gm != null)
                    _instance = gm.GetComponent<MasterGameManager>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("MasterGameManager");
                    _instance = go.AddComponent<MasterGameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    [Header("Cached Systems - Set via Inspector")]
    public CoreResources Resources;
    public CurrencyManager Currency;
    public BuildingSystem BuildingSystem;
    public CombatSystem CombatSystem;
    public PVERaidSystem PVERaid;
    public ChestSystem ChestSystem;
    public QuestManager QuestManager;
    public TutorialSystem TutorialSystem;
    public UIManager UIManager;
    public EnergySystem EnergySystem;
    public AudioManager Audio;
    public VisualManager Visual;
    public ClanSystem Clan;
    public Leaderboard Leaderboard;
    public NotificationManager Notification;
    public VehicleSystem Vehicle;
    public DefenseSystem Defense;
    public OfflineRewards OfflineRewards;
    public GameBalancer Balancer;
    public AntiCheat AntiCheat;
    
    [Header("Game State")]
    public int playerLevel = 1;
    public int currentXP = 0;
    public int troopCount = 0;
    public int buildingLevel = 1;
    public int raidsCompleted = 0;
    
    private bool isGameReady = false;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGameData();
    }
    
    void Start()
    {
        isGameReady = true;
        StartCoroutine(DelayedStart());
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (TutorialSystem != null && PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            TutorialSystem.StartTutorial();
        }
        
        UpdateAllUI();
    }
    
    public void GatherResource(string type, int amount)
    {
        if (!isGameReady) return;
        
        switch (type.ToLower())
        {
            case "wood": Resources?.AddResource("scrap", amount); break;
            case "stone": Resources?.AddResource("fuel", amount); break;
            case "food": Resources?.AddResource("food", amount); break;
        }
        
        QuestManager?.UpdateProgress(type, amount);
        TutorialSystem?.CheckAction("gather");
        UpdateResourceUI();
    }
    
    public void UpgradeBuilding()
    {
        if (!isGameReady) return;
        
        int cost = GameBalancer.Instance?.CalculateUpgradeCost() ?? (50 * buildingLevel);
        
        if (Currency != null && Currency.SpendCoins(cost))
        {
            buildingLevel++;
            BuildingSystem?.UpgradeBuilding(cost);
            UIManager?.ShowNotification($"🏗️ Building level {buildingLevel}!", Color.green);
            TutorialSystem?.CheckAction("upgrade");
            UpdateBuildingUI();
        }
    }
    
    public void TrainTroop()
    {
        if (!isGameReady) return;
        
        int cost = 30;
        
        if (Currency != null && Currency.SpendCoins(cost))
        {
            troopCount++;
            UIManager?.ShowNotification($"⚔️ Troops: {troopCount}", Color.cyan);
            TutorialSystem?.CheckAction("train");
            UpdateTroopUI();
        }
    }
    
    public void StartRaid()
    {
        if (troopCount <= 0)
        {
            UIManager?.ShowNotification("❌ No troops!", Color.red);
            return;
        }
        
        int playerPower = (CombatSystem?.CalculateRaidPower(troopCount, buildingLevel) ?? 50) + (Vehicle?.GetAttackBonus() ?? 0);
        int enemyPower = GameBalancer.Instance?.CalculateEnemyPower() ?? Random.Range(30, 80);
        
        var result = CombatSystem?.ExecuteRaid(playerPower, enemyPower);
        
        if (result != null && result.isVictory)
        {
            raidsCompleted++;
            Resources?.AddResource("scrap", result.lootAmount);
            AddXP(result.expGain);
            UIManager?.ShowNotification($"🏆 VICTORY! +{result.lootAmount} scrap!", Color.green);
            QuestManager?.UpdateProgress("raid", 1);
            TutorialSystem?.CheckAction("raid");
        }
        else
        {
            troopCount = Mathf.Max(0, troopCount - 1);
            UIManager?.ShowNotification($"💀 DEFEAT! Lost 1 troop!", Color.red);
        }
        UpdateAllUI();
    }
    
    public void OpenChest()
    {
        if (ChestSystem == null) return;
        
        var reward = ChestSystem.OpenChest();
        if (reward != null)
        {
            switch (reward.rewardType)
            {
                case "coins": Currency?.AddCoins(reward.amount); break;
                case "gems": Currency?.AddGems(reward.amount); break;
                case "scrap": Resources?.AddResource("scrap", reward.amount); break;
            }
            UIManager?.ShowNotification($"🎁 +{reward.amount} {reward.rewardType}!", Color.magenta);
            TutorialSystem?.CheckAction("chest");
            UpdateAllUI();
        }
    }
    
    void AddXP(int amount)
    {
        currentXP += amount;
        int needed = playerLevel * 100;
        if (currentXP >= needed)
        {
            currentXP -= needed;
            playerLevel++;
            UIManager?.ShowNotification($"🎉 LEVEL {playerLevel}!", Color.yellow);
            Currency?.AddGems(50);
            Currency?.AddCoins(200);
            EnergySystem?.RefillEnergy();
        }
        UpdateLevelUI();
    }
    
    void UpdateAllUI()
    {
        UpdateResourceUI();
        UpdateBuildingUI();
        UpdateTroopUI();
        UpdateLevelUI();
    }
    
    void UpdateResourceUI() => UIManager?.UpdateResources(Resources?.scrap ?? 0, Resources?.fuel ?? 0, Currency?.coins ?? 0, Currency?.gems ?? 0);
    void UpdateBuildingUI() => UIManager?.UpdateStatus(playerLevel, troopCount, PVERaid?.raidEnergy ?? 3);
    void UpdateTroopUI() => UIManager?.UpdateTroopCount(troopCount);
    void UpdateLevelUI() => UIManager?.UpdateLevel(playerLevel, currentXP, playerLevel * 100);
    
    void LoadGameData()
    {
        playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentXP = PlayerPrefs.GetInt("CurrentXP", 0);
        troopCount = PlayerPrefs.GetInt("TroopCount", 0);
        buildingLevel = PlayerPrefs.GetInt("BuildingLevel", 1);
        raidsCompleted = PlayerPrefs.GetInt("RaidsCompleted", 0);
    }
    
    void OnApplicationQuit() => SaveGameData();
    void OnApplicationPause(bool pause) { if (pause) SaveGameData(); }
    
    void SaveGameData()
    {
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("CurrentXP", currentXP);
        PlayerPrefs.SetInt("TroopCount", troopCount);
        PlayerPrefs.SetInt("BuildingLevel", buildingLevel);
        PlayerPrefs.SetInt("RaidsCompleted", raidsCompleted);
        PlayerPrefs.Save();
    }
    
    public int GetTroopCount() => troopCount;
    public int GetBuildingLevel() => buildingLevel;
    public int GetPlayerLevel() => playerLevel;
    public bool IsGameReady() => isGameReady;
}
