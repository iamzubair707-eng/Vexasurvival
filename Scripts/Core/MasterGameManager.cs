using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterGameManager : MonoBehaviour
{
    // ==================== ULTRA-STRONG SINGLETON ====================
    private static MasterGameManager _instance;
    private static readonly object _lock = new object();
    private static bool _quitting = false;
    
    public static MasterGameManager Instance
    {
        get
        {
            if (_quitting)
            {
                DebugLogger.LogWarning("MasterGameManager already destroyed! Returning null.");
                return null;
            }
            
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<MasterGameManager>();
                    
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
    }
    
    // ==================== CACHED SYSTEM REFERENCES (No FindObjectOfType needed) ====================
    [Header("⚡ CACHED SYSTEMS - Set via Inspector or Auto-Detect")]
    [SerializeField] private CoreResources _resources;
    [SerializeField] private CurrencyManager _currency;
    [SerializeField] private BuildingSystem _buildingSystem;
    [SerializeField] private CombatSystem _combatSystem;
    [SerializeField] private PVERaidSystem _pveRaid;
    [SerializeField] private ChestSystem _chestSystem;
    [SerializeField] private QuestManager _questManager;
    [SerializeField] private TutorialSystem _tutorialSystem;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private EnergySystem _energySystem;
    [SerializeField] private AudioManager _audio;
    [SerializeField] private VisualManager _visual;
    [SerializeField] private ClanSystem _clan;
    [SerializeField] private Leaderboard _leaderboard;
    [SerializeField] private NotificationManager _notification;
    [SerializeField] private VehicleSystem _vehicle;
    [SerializeField] private DefenseSystem _defense;
    [SerializeField] private OfflineRewards _offlineRewards;
    [SerializeField] private GameBalancer _balancer;
    [SerializeField] private AntiCheat _antiCheat;
    [SerializeField] private MentalHealthSystem _mentalHealth;
    
    // Public properties for access
    public CoreResources Resources => _resources;
    public CurrencyManager Currency => _currency;
    public BuildingSystem BuildingSystem => _buildingSystem;
    public CombatSystem CombatSystem => _combatSystem;
    public PVERaidSystem PVERaid => _pveRaid;
    public ChestSystem ChestSystem => _chestSystem;
    public QuestManager QuestManager => _questManager;
    public TutorialSystem TutorialSystem => _tutorialSystem;
    public UIManager UIManager => _uiManager;
    public EnergySystem EnergySystem => _energySystem;
    public AudioManager Audio => _audio;
    public VisualManager Visual => _visual;
    public ClanSystem Clan => _clan;
    public Leaderboard Leaderboard => _leaderboard;
    public NotificationManager Notification => _notification;
    public VehicleSystem Vehicle => _vehicle;
    public DefenseSystem Defense => _defense;
    public OfflineRewards OfflineRewards => _offlineRewards;
    public GameBalancer Balancer => _balancer;
    public AntiCheat AntiCheat => _antiCheat;
    public MentalHealthSystem MentalHealth => _mentalHealth;
    
    // ==================== GAME STATE ====================
    [Header("📊 GAME STATE")]
    [SerializeField] private int _playerLevel = 1;
    [SerializeField] private int _currentXP = 0;
    [SerializeField] private int _troopCount = 0;
    [SerializeField] private int _buildingLevel = 1;
    [SerializeField] private int _raidsCompleted = 0;
    [SerializeField] private int _coins = 500;
    [SerializeField] private int _gems = 50;
    [SerializeField] private int _wood = 100;
    [SerializeField] private int _stone = 50;
    
    public int PlayerLevel => _playerLevel;
    public int CurrentXP => _currentXP;
    public int TroopCount => _troopCount;
    public int BuildingLevel => _buildingLevel;
    public int RaidsCompleted => _raidsCompleted;
    public int Coins => _coins;
    public int Gems => _gems;
    public int Wood => _wood;
    public int Stone => _stone;
    
    private bool _isGameReady = false;
    private bool _isInitialized = false;
    
    // ==================== INITIALIZATION ====================
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeSystems();
    }
    
    void InitializeSystems()
    {
        DebugLogger.Log("🚀 Initializing MasterGameManager...");
        
        // Find all systems ONCE at startup
        _resources = FindFirstObjectByType<CoreResources>();
        _currency = FindFirstObjectByType<CurrencyManager>();
        _buildingSystem = FindFirstObjectByType<BuildingSystem>();
        _combatSystem = FindFirstObjectByType<CombatSystem>();
        _pveRaid = FindFirstObjectByType<PVERaidSystem>();
        _chestSystem = FindFirstObjectByType<ChestSystem>();
        _questManager = FindFirstObjectByType<QuestManager>();
        _tutorialSystem = FindFirstObjectByType<TutorialSystem>();
        _uiManager = FindFirstObjectByType<UIManager>();
        _energySystem = FindFirstObjectByType<EnergySystem>();
        _audio = FindFirstObjectByType<AudioManager>();
        _visual = FindFirstObjectByType<VisualManager>();
        _clan = FindFirstObjectByType<ClanSystem>();
        _leaderboard = FindFirstObjectByType<Leaderboard>();
        _notification = FindFirstObjectByType<NotificationManager>();
        _vehicle = FindFirstObjectByType<VehicleSystem>();
        _defense = FindFirstObjectByType<DefenseSystem>();
        _offlineRewards = FindFirstObjectByType<OfflineRewards>();
        _balancer = FindFirstObjectByType<GameBalancer>();
        _antiCheat = FindFirstObjectByType<AntiCheat>();
        _mentalHealth = FindFirstObjectByType<MentalHealthSystem>();
        
        // Auto-create missing critical systems
        if (_resources == null) _resources = gameObject.AddComponent<CoreResources>();
        if (_currency == null) _currency = gameObject.AddComponent<CurrencyManager>();
        if (_buildingSystem == null) _buildingSystem = gameObject.AddComponent<BuildingSystem>();
        if (_uiManager == null) _uiManager = gameObject.AddComponent<UIManager>();
        
        LoadGameData();
        _isInitialized = true;
        
        DebugLogger.Log($"✅ MasterGameManager initialized with {CountSystems()} systems!");
    }
    
    void Start()
    {
        _isGameReady = true;
        StartCoroutine(DelayedStart());
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (_tutorialSystem != null && PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            _tutorialSystem.StartTutorial();
        }
        
        UpdateAllUI();
        DebugLogger.Log("🎮 VEXA SURVIVAL - Ready to play!");
    }
    
    int CountSystems()
    {
        int count = 0;
        if (_resources != null) count++;
        if (_currency != null) count++;
        if (_buildingSystem != null) count++;
        if (_combatSystem != null) count++;
        if (_pveRaid != null) count++;
        if (_chestSystem != null) count++;
        if (_questManager != null) count++;
        if (_uiManager != null) count++;
        if (_vehicle != null) count++;
        if (_mentalHealth != null) count++;
        return count;
    }
    
    // ==================== CORE LOOP METHODS ====================
    
    public void GatherResource(string type, int amount)
    {
        if (!_isGameReady) return;
        
        switch (type.ToLower())
        {
            case "wood":
                _wood += amount;
                break;
            case "stone":
                _stone += amount;
                break;
        }
        
        _questManager?.UpdateProgress(type, amount);
        _tutorialSystem?.CheckAction("gather");
        UpdateResourceUI();
        SaveGameData();
        
        DebugLogger.Log($"📦 Gathered {amount} {type}!");
    }
    
    public void UpgradeBuilding()
    {
        if (!_isGameReady) return;
        
        int cost = 50 * _buildingLevel;
        
        if (_currency != null && _currency.SpendCoins(cost))
        {
            _buildingLevel++;
            _buildingSystem?.UpgradeBuilding(cost);
            _uiManager?.ShowNotification($"🏗️ Building upgraded to level {_buildingLevel}!", Color.green);
            _tutorialSystem?.CheckAction("upgrade");
            UpdateBuildingUI();
            SaveGameData();
        }
        else
        {
            _uiManager?.ShowNotification($"❌ Need {cost} coins!", Color.red);
        }
    }
    
    public void TrainTroop()
    {
        if (!_isGameReady) return;
        
        int cost = 30;
        
        if (_currency != null && _currency.SpendCoins(cost))
        {
            _troopCount++;
            _uiManager?.ShowNotification($"⚔️ Troop trained! Total: {_troopCount}", Color.cyan);
            _tutorialSystem?.CheckAction("train");
            UpdateTroopUI();
            SaveGameData();
        }
        else
        {
            _uiManager?.ShowNotification($"❌ Need {cost} coins!", Color.red);
        }
    }
    
    public void StartRaid()
    {
        if (_troopCount <= 0)
        {
            _uiManager?.ShowNotification("❌ No troops! Train first!", Color.red);
            return;
        }
        
        int playerPower = (_combatSystem?.CalculateRaidPower(_troopCount, _buildingLevel) ?? 50) + (_vehicle?.GetAttackBonus() ?? 0);
        int enemyPower = _balancer?.CalculateEnemyPower() ?? Random.Range(30, 80);
        
        var result = _combatSystem?.ExecuteRaid(playerPower, enemyPower);
        
        if (result != null && result.isVictory)
        {
            _raidsCompleted++;
            _wood += result.lootAmount;
            AddXP(result.expGain);
            _uiManager?.ShowNotification($"🏆 RAID VICTORY! +{result.lootAmount} wood!", Color.green);
            _questManager?.UpdateProgress("raid", 1);
            _tutorialSystem?.CheckAction("raid");
        }
        else
        {
            _troopCount = Mathf.Max(0, _troopCount - 1);
            _uiManager?.ShowNotification($"💀 RAID DEFEATED! Lost 1 troop!", Color.red);
        }
        
        UpdateAllUI();
        SaveGameData();
    }
    
    public void OpenChest()
    {
        if (_chestSystem == null) return;
        
        var reward = _chestSystem.OpenChest();
        if (reward != null)
        {
            switch (reward.rewardType)
            {
                case "coins": _currency?.AddCoins(reward.amount); break;
                case "gems": _currency?.AddGems(reward.amount); break;
                case "wood": _wood += reward.amount; break;
                case "stone": _stone += reward.amount; break;
            }
            _uiManager?.ShowNotification($"🎁 +{reward.amount} {reward.rewardType}!", Color.magenta);
            _tutorialSystem?.CheckAction("chest");
            UpdateAllUI();
            SaveGameData();
        }
    }
    
    void AddXP(int amount)
    {
        _currentXP += amount;
        int needed = _playerLevel * 100;
        
        if (_currentXP >= needed)
        {
            _currentXP -= needed;
            _playerLevel++;
            _uiManager?.ShowNotification($"🎉 LEVEL {_playerLevel}!", Color.yellow);
            _currency?.AddGems(50);
            _currency?.AddCoins(200);
            _energySystem?.RefillEnergy();
        }
        
        UpdateLevelUI();
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
        _uiManager?.UpdateResources(_wood, _stone, _coins, _gems);
    }
    
    void UpdateBuildingUI()
    {
        _uiManager?.UpdateStatus(_playerLevel, _troopCount, _pveRaid?.raidEnergy ?? 3);
    }
    
    void UpdateTroopUI()
    {
        _uiManager?.UpdateTroopCount(_troopCount);
    }
    
    void UpdateLevelUI()
    {
        _uiManager?.UpdateLevel(_playerLevel, _currentXP, _playerLevel * 100);
    }
    
    // ==================== SAVE & LOAD ====================
    
    void LoadGameData()
    {
        _playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        _currentXP = PlayerPrefs.GetInt("CurrentXP", 0);
        _troopCount = PlayerPrefs.GetInt("TroopCount", 0);
        _buildingLevel = PlayerPrefs.GetInt("BuildingLevel", 1);
        _raidsCompleted = PlayerPrefs.GetInt("RaidsCompleted", 0);
        _coins = PlayerPrefs.GetInt("Coins", 500);
        _gems = PlayerPrefs.GetInt("Gems", 50);
        _wood = PlayerPrefs.GetInt("Wood", 100);
        _stone = PlayerPrefs.GetInt("Stone", 50);
    }
    
    void SaveGameData()
    {
        PlayerPrefs.SetInt("PlayerLevel", _playerLevel);
        PlayerPrefs.SetInt("CurrentXP", _currentXP);
        PlayerPrefs.SetInt("TroopCount", _troopCount);
        PlayerPrefs.SetInt("BuildingLevel", _buildingLevel);
        PlayerPrefs.SetInt("RaidsCompleted", _raidsCompleted);
        PlayerPrefs.SetInt("Coins", _coins);
        PlayerPrefs.SetInt("Gems", _gems);
        PlayerPrefs.SetInt("Wood", _wood);
        PlayerPrefs.SetInt("Stone", _stone);
        PlayerPrefs.Save();
    }
    
    void OnApplicationQuit()
    {
        _quitting = true;
        SaveGameData();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGameData();
    }
    
    // ==================== PUBLIC GETTERS ====================
    
    public bool IsGameReady() => _isGameReady;
    public bool IsInitialized() => _isInitialized;
}
