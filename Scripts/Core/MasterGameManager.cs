using UnityEngine;
using System.Collections;

public class MasterGameManager : MonoBehaviour
{
    public static MasterGameManager Instance { get; private set; }
    
    [Header("Core Systems")]
    public CoreResources resources;
    public CurrencyManager currency;
    public BuildingSystem buildingSystem;
    public CombatSystem combatSystem;
    public PVERaidSystem pveRaid;
    public ChestSystem chestSystem;
    public QuestManager questManager;
    public TutorialSystem tutorialSystem;
    
    [Header("UI")]
    public UIManager uiManager;
    
    [Header("Game State")]
    public int playerLevel = 1;
    public int currentXP = 0;
    public int troopCount = 0;
    public int buildingLevel = 1;
    public int raidsCompleted = 0;
    
    private bool isGameReady = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAllSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAllSystems()
    {
        Debug.Log("🚀 Initializing VEXA SURVIVAL - Master Game Manager");
        
        // Find or create systems
        if (resources == null) resources = FindObjectOfType<CoreResources>();
        if (currency == null) currency = FindObjectOfType<CurrencyManager>();
        if (buildingSystem == null) buildingSystem = FindObjectOfType<BuildingSystem>();
        if (combatSystem == null) combatSystem = FindObjectOfType<CombatSystem>();
        if (pveRaid == null) pveRaid = FindObjectOfType<PVERaidSystem>();
        if (chestSystem == null) chestSystem = FindObjectOfType<ChestSystem>();
        if (questManager == null) questManager = FindObjectOfType<QuestManager>();
        if (tutorialSystem == null) tutorialSystem = FindObjectOfType<TutorialSystem>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        
        LoadGameData();
        isGameReady = true;
        
        StartCoroutine(DelayedStart());
        Debug.Log("✅ All systems initialized! Total Systems: " + CountActiveSystems());
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Start tutorial if not completed
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0 && tutorialSystem != null)
        {
            tutorialSystem.StartTutorial();
        }
        
        // Check daily streak
        CheckDailyLogin();
        
        // Check offline rewards
        CheckOfflineRewards();
        
        UpdateAllUI();
        Debug.Log("🎮 Game Ready! Start playing!");
    }
    
    // ==================== CORE LOOP METHODS ====================
    
    public void GatherResource(string type, int amount)
    {
        if (!isGameReady) return;
        
        switch (type.ToLower())
        {
            case "wood":
                resources?.AddResource("scrap", amount);
                break;
            case "stone":
                resources?.AddResource("fuel", amount);
                break;
            case "food":
                resources?.AddResource("food", amount);
                break;
        }
        
        // Add to quest progress
        questManager?.UpdateProgress(type, amount);
        
        // Check tutorial
        tutorialSystem?.CheckAction("gather");
        
        UpdateResourceUI();
        Debug.Log($"📦 Gathered {amount} {type}!");
    }
    
    public void UpgradeBuilding()
    {
        if (!isGameReady) return;
        
        int cost = 50 * buildingLevel;
        
        if (currency != null && currency.SpendCoins(cost))
        {
            buildingLevel++;
            buildingSystem?.UpgradeBuilding(cost);
            
            // Visual feedback
            uiManager?.ShowNotification($"🏗️ Building upgraded to level {buildingLevel}!", Color.green);
            
            // Check tutorial
            tutorialSystem?.CheckAction("upgrade");
            
            UpdateBuildingUI();
            Debug.Log($"Building upgraded to level {buildingLevel}!");
        }
        else
        {
            uiManager?.ShowNotification($"❌ Not enough coins! Need {cost} coins!", Color.red);
        }
    }
    
    public void TrainTroop()
    {
        if (!isGameReady) return;
        
        int cost = 30;
        
        if (currency != null && currency.SpendCoins(cost))
        {
            troopCount++;
            
            uiManager?.ShowNotification($"⚔️ Troop trained! Total: {troopCount}", Color.cyan);
            
            // Check tutorial
            tutorialSystem?.CheckAction("train");
            
            UpdateTroopUI();
            Debug.Log($"Troop trained! Now have {troopCount} troops.");
        }
        else
        {
            uiManager?.ShowNotification($"❌ Not enough coins! Need {cost} coins!", Color.red);
        }
    }
    
    public void StartRaid()
    {
        if (!isGameReady) return;
        
        if (troopCount <= 0)
        {
            uiManager?.ShowNotification("❌ No troops! Train some soldiers first!", Color.red);
            return;
        }
        
        if (pveRaid == null)
        {
            Debug.LogError("PVERaidSystem not found!");
            return;
        }
        
        // Calculate raid power
        int playerPower = combatSystem?.CalculateRaidPower(troopCount, buildingLevel) ?? 50;
        int enemyPower = Random.Range(30, 80);
        
        uiManager?.ShowNotification($"⚔️ RAID STARTED! Power: {playerPower} vs {enemyPower}", Color.yellow);
        
        // Execute raid
        var result = combatSystem?.ExecuteRaid(playerPower, enemyPower);
        
        if (result != null && result.isVictory)
        {
            // Victory
            raidsCompleted++;
            int loot = result.lootAmount;
            int expGain = result.expGain;
            
            resources?.AddResource("scrap", loot);
            AddXP(expGain);
            
            uiManager?.ShowNotification($"🏆 RAID VICTORY! +{loot} scrap, +{expGain} XP!", Color.green);
            
            // Add to quest progress
            questManager?.UpdateProgress("raid", 1);
            
            // Give free chest chance
            if (Random.Range(0, 100) < 20)
            {
                chestSystem?.AddFreeChest();
                uiManager?.ShowNotification($"🎁 Bonus chest earned!", Color.yellow);
            }
            
            // Check tutorial
            tutorialSystem?.CheckAction("raid");
        }
        else
        {
            // Defeat
            int damage = result?.damageTaken ?? 30;
            troopCount = Mathf.Max(0, troopCount - 1);
            
            uiManager?.ShowNotification($"💀 RAID DEFEATED! Lost {damage} scrap and 1 troop!", Color.red);
        }
        
        UpdateAllUI();
        Debug.Log($"Raid completed! Victory: {result?.isVictory ?? false}");
    }
    
    public void OpenChest()
    {
        if (!isGameReady) return;
        
        if (chestSystem == null)
        {
            Debug.LogError("ChestSystem not found!");
            return;
        }
        
        var reward = chestSystem.OpenChest();
        
        if (reward != null)
        {
            switch (reward.rewardType)
            {
                case "coins":
                    currency?.AddCoins(reward.amount);
                    break;
                case "gems":
                    currency?.AddGems(reward.amount);
                    break;
                case "scrap":
                    resources?.AddResource("scrap", reward.amount);
                    break;
            }
            
            uiManager?.ShowNotification($"🎁 CHEST OPENED! +{reward.amount} {reward.rewardType}!", Color.magenta);
            
            // Check tutorial
            tutorialSystem?.CheckAction("chest");
            
            UpdateAllUI();
            Debug.Log($"Chest opened! Got {reward.amount} {reward.rewardType}!");
        }
        else
        {
            uiManager?.ShowNotification("⏰ No chests available! Come back in 3 hours!", Color.gray);
        }
    }
    
    // ==================== XP & LEVEL SYSTEM ====================
    
    void AddXP(int amount)
    {
        currentXP += amount;
        int xpNeeded = playerLevel * 100;
        
        if (currentXP >= xpNeeded)
        {
            currentXP -= xpNeeded;
            playerLevel++;
            uiManager?.ShowNotification($"🎉 LEVEL UP! Now Level {playerLevel}!", Color.yellow);
            OnLevelUp();
        }
        
        UpdateLevelUI();
    }
    
    void OnLevelUp()
    {
        // Give level up rewards
        currency?.AddGems(50);
        currency?.AddCoins(200);
        
        // Full energy restore
        EnergySystem energy = FindObjectOfType<EnergySystem>();
        energy?.RefillEnergy();
    }
    
    // ==================== DAILY & OFFLINE ====================
    
    void CheckDailyLogin()
    {
        string lastDate = PlayerPrefs.GetString("LastLoginDate", "");
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        if (lastDate != today)
        {
            int streak = PlayerPrefs.GetInt("LoginStreak", 0);
            streak++;
            PlayerPrefs.SetInt("LoginStreak", streak);
            PlayerPrefs.SetString("LastLoginDate", today);
            
            int bonus = 50 + (streak * 10);
            currency?.AddCoins(bonus);
            uiManager?.ShowNotification($"🔥 Daily Login! +{bonus} coins! Streak: {streak}", Color.yellow);
        }
    }
    
    void CheckOfflineRewards()
    {
        string lastTimeStr = PlayerPrefs.GetString("LastOfflineTime", "");
        if (!string.IsNullOrEmpty(lastTimeStr))
        {
            System.DateTime lastTime = System.DateTime.Parse(lastTimeStr);
            System.TimeSpan diff = System.DateTime.Now - lastTime;
            int hoursOffline = Mathf.Min((int)diff.TotalHours, 12);
            
            if (hoursOffline > 0)
            {
                int offlineReward = hoursOffline * 20;
                resources?.AddResource("scrap", offlineReward);
                uiManager?.ShowNotification($"🎁 Welcome back! +{offlineReward} scrap from offline!", Color.green);
            }
        }
        PlayerPrefs.SetString("LastOfflineTime", System.DateTime.Now.ToString());
    }
    
    // ==================== UI UPDATES ====================
    
    void UpdateAllUI()
    {
        UpdateResourceUI();
        UpdateBuildingUI();
        UpdateTroopUI();
        UpdateLevelUI();
    }
    
    void UpdateResourceUI()
    {
        if (uiManager != null && resources != null && currency != null)
        {
            uiManager.UpdateResources(
                resources.scrap,  // wood
                resources.fuel,   // stone
                currency.coins,
                currency.gems
            );
        }
    }
    
    void UpdateBuildingUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateStatus(playerLevel, troopCount, pveRaid?.raidEnergy ?? 3);
        }
    }
    
    void UpdateTroopUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateTroopCount(troopCount);
        }
    }
    
    void UpdateLevelUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateLevel(playerLevel, currentXP, playerLevel * 100);
        }
    }
    
    // ==================== SAVE & LOAD ====================
    
    void LoadGameData()
    {
        playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentXP = PlayerPrefs.GetInt("CurrentXP", 0);
        troopCount = PlayerPrefs.GetInt("TroopCount", 0);
        buildingLevel = PlayerPrefs.GetInt("BuildingLevel", 1);
        raidsCompleted = PlayerPrefs.GetInt("RaidsCompleted", 0);
    }
    
    void SaveGameData()
    {
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("CurrentXP", currentXP);
        PlayerPrefs.SetInt("TroopCount", troopCount);
        PlayerPrefs.SetInt("BuildingLevel", buildingLevel);
        PlayerPrefs.SetInt("RaidsCompleted", raidsCompleted);
        PlayerPrefs.Save();
    }
    
    void OnApplicationQuit()
    {
        SaveGameData();
        PlayerPrefs.SetString("LastOfflineTime", System.DateTime.Now.ToString());
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
            PlayerPrefs.SetString("LastOfflineTime", System.DateTime.Now.ToString());
        }
    }
    
    int CountActiveSystems()
    {
        int count = 0;
        if (resources != null) count++;
        if (currency != null) count++;
        if (buildingSystem != null) count++;
        if (combatSystem != null) count++;
        if (pveRaid != null) count++;
        if (chestSystem != null) count++;
        if (questManager != null) count++;
        if (tutorialSystem != null) count++;
        if (uiManager != null) count++;
        return count;
    }
    
    // ==================== PUBLIC GETTERS ====================
    
    public int GetTroopCount() => troopCount;
    public int GetBuildingLevel() => buildingLevel;
    public int GetPlayerLevel() => playerLevel;
    public bool IsGameReady() => isGameReady;
}
