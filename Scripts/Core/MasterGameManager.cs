using UnityEngine;
using System.Collections.Generic;

public class MasterGameManager : MonoBehaviour
{
    private static MasterGameManager _instance;
    public static MasterGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MasterGameManager>();
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
    
    [Header("Cached Systems - Set once at startup")]
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
    public VehicleManager Vehicle;
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
        CacheAllSystems();
    }
    
    void CacheAllSystems()
    {
        DebugLogger.Log("🚀 Caching all systems...");
        
        Resources = FindObjectOfType<CoreResources>();
        Currency = FindObjectOfType<CurrencyManager>();
        BuildingSystem = FindObjectOfType<BuildingSystem>();
        CombatSystem = FindObjectOfType<CombatSystem>();
        PVERaid = FindObjectOfType<PVERaidSystem>();
        ChestSystem = FindObjectOfType<ChestSystem>();
        QuestManager = FindObjectOfType<QuestManager>();
        TutorialSystem = FindObjectOfType<TutorialSystem>();
        UIManager = FindObjectOfType<UIManager>();
        EnergySystem = FindObjectOfType<EnergySystem>();
        Audio = FindObjectOfType<AudioManager>();
        Visual = FindObjectOfType<VisualManager>();
        Clan = FindObjectOfType<ClanSystem>();
        Leaderboard = FindObjectOfType<Leaderboard>();
        Notification = FindObjectOfType<NotificationManager>();
        Vehicle = FindObjectOfType<VehicleManager>();
        Defense = FindObjectOfType<DefenseSystem>();
        OfflineRewards = FindObjectOfType<OfflineRewards>();
        Balancer = FindObjectOfType<GameBalancer>();
        AntiCheat = FindObjectOfType<AntiCheat>();
        
        // Create missing systems if needed
        if (Resources == null) Resources = gameObject.AddComponent<CoreResources>();
        if (Currency == null) Currency = gameObject.AddComponent<CurrencyManager>();
        if (BuildingSystem == null) BuildingSystem = gameObject.AddComponent<BuildingSystem>();
        
        LoadGameData();
        isGameReady = true;
        
        DebugLogger.Log($"✅ All systems cached! Total: {CountActiveSystems()}");
    }
    
    int CountActiveSystems()
    {
        int count = 0;
        if (Resources != null) count++;
        if (Currency != null) count++;
        if (BuildingSystem != null) count++;
        if (CombatSystem != null) count++;
        if (PVERaid != null) count++;
        if (ChestSystem != null) count++;
        if (QuestManager != null) count++;
        if (UIManager != null) count++;
        return count;
    }
    
    void Start()
    {
        StartCoroutine(DelayedStart());
    }
    
    System.Collections.IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (TutorialSystem != null && PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            TutorialSystem.StartTutorial();
        }
        
        CheckDailyLogin();
        if (OfflineRewards != null) OfflineRewards.CheckOfflineRewards();
        
        UpdateAllUI();
        DebugLogger.Log("🎮 Game Ready!");
    }
    
    // ==================== CORE LOOP METHODS ====================
    
    public void GatherResource(string type, int amount)
    {
        if (!isGameReady) return;
        
        if (Resources != null)
        {
            switch (type.ToLower())
            {
                case "wood": Resources.AddResource("scrap", amount); break;
                case "stone": Resources.AddResource("fuel", amount); break;
                case "food": Resources.AddResource("food", amount); break;
            }
        }
        
        QuestManager?.UpdateProgress(type, amount);
        TutorialSystem?.CheckAction("gather");
        UpdateResourceUI();
        DebugLogger.Log($"📦 Gathered {amount} {type}!");
    }
    
    public void UpgradeBuilding()
    {
        if (!isGameReady) return;
        
        int cost = 50 * buildingLevel;
        
        if (Currency != null && Currency.SpendCoins(cost))
        {
            buildingLevel++;
            BuildingSystem?.UpgradeBuilding(cost);
            UIManager?.ShowNotification($"🏗️ Building upgraded to level {buildingLevel}!", Color.green);
            TutorialSystem?.CheckAction("upgrade");
            UpdateBuildingUI();
        }
        else
        {
            UIManager?.ShowNotification($"❌ Need {cost} coins!", Color.red);
        }
    }
    
    public void TrainTroop()
    {
        if (!isGameReady) return;
        
        int cost = 30;
        
        if (Currency != null && Currency.SpendCoins(cost))
        {
            troopCount++;
            UIManager?.ShowNotification($"⚔️ Troop trained! Total: {troopCount}", Color.cyan);
            TutorialSystem?.CheckAction("train");
            UpdateTroopUI();
        }
        else
        {
            UIManager?.ShowNotification($"❌ Need {cost} coins!", Color.red);
        }
    }
    
    public void StartRaid()
    {
        if (!isGameReady) return;
        
        if (troopCount <= 0)
        {
            UIManager?.ShowNotification("❌ No troops! Train first!", Color.red);
            return;
        }
        
        int playerPower = CombatSystem?.CalculateRaidPower(troopCount, buildingLevel) ?? 50;
        int enemyPower = Random.Range(30, 80);
        
        var result = CombatSystem?.ExecuteRaid(playerPower, enemyPower);
        
        if (result != null && result.isVictory)
        {
            raidsCompleted++;
            Resources?.AddResource("scrap", result.lootAmount);
            AddXP(result.expGain);
            UIManager?.ShowNotification($"🏆 RAID VICTORY! +{result.lootAmount} scrap!", Color.green);
            QuestManager?.UpdateProgress("raid", 1);
            TutorialSystem?.CheckAction("raid");
        }
        else
        {
            troopCount = Mathf.Max(0, troopCount - 1);
            UIManager?.ShowNotification($"💀 RAID DEFEATED! Lost 1 troop!", Color.red);
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
        else
        {
            UIManager?.ShowNotification("⏰ No chests! Come back in 3 hours!", Color.gray);
        }
    }
    
    void AddXP(int amount)
    {
        currentXP += amount;
        int xpNeeded = playerLevel * 100;
        
        if (currentXP >= xpNeeded)
        {
            currentXP -= xpNeeded;
            playerLevel++;
            UIManager?.ShowNotification($"🎉 LEVEL {playerLevel}!", Color.yellow);
            Currency?.AddGems(50);
            Currency?.AddCoins(200);
            EnergySystem?.RefillEnergy();
        }
        
        UpdateLevelUI();
    }
    
    // ==================== DAILY & OFFLINE ====================
    
    void CheckDailyLogin()
    {
        string lastDate = PlayerPrefs.GetString("LastLoginDate", "");
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        if (lastDate != today)
        {
            int streak = PlayerPrefs.GetInt("LoginStreak", 0) + 1;
            PlayerPrefs.SetInt("LoginStreak", streak);
            PlayerPrefs.SetString("LastLoginDate", today);
            
            int bonus = 50 + (streak * 10);
            Currency?.AddCoins(bonus);
            UIManager?.ShowNotification($"🔥 Daily +{bonus} coins! Streak: {streak}", Color.yellow);
        }
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
        if (UIManager != null && Resources != null && Currency != null)
        {
            UIManager.UpdateResources(Resources.scrap, Resources.fuel, Currency.coins, Currency.gems);
        }
    }
    
    void UpdateBuildingUI()
    {
        if (UIManager != null)
        {
            UIManager.UpdateStatus(playerLevel, troopCount, PVERaid?.raidEnergy ?? 3);
        }
    }
    
    void UpdateTroopUI()
    {
        if (UIManager != null) UIManager.UpdateTroopCount(troopCount);
    }
    
    void UpdateLevelUI()
    {
        if (UIManager != null) UIManager.UpdateLevel(playerLevel, currentXP, playerLevel * 100);
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
    
    void OnApplicationQuit() => SaveGameData();
    void OnApplicationPause(bool pause) { if (pause) SaveGameData(); }
    
    // ==================== PUBLIC GETTERS ====================
    
    public int GetTroopCount() => troopCount;
    public int GetBuildingLevel() => buildingLevel;
    public int GetPlayerLevel() => playerLevel;
    public bool IsGameReady() => isGameReady;
    
    // Generic getter for any system
    public T GetSystem<T>() where T : Component
    {
        var typeName = typeof(T).Name;
        return typeName switch
        {
            nameof(CoreResources) => Resources as T,
            nameof(CurrencyManager) => Currency as T,
            nameof(BuildingSystem) => BuildingSystem as T,
            nameof(CombatSystem) => CombatSystem as T,
            nameof(PVERaidSystem) => PVERaid as T,
            nameof(ChestSystem) => ChestSystem as T,
            nameof(UIManager) => UIManager as T,
            nameof(AudioManager) => Audio as T,
            nameof(VisualManager) => Visual as T,
            _ => FindObjectOfType<T>()
        };
    }
}
