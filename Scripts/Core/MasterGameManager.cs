using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterGameManager : MonoBehaviour
{
    #region Singleton Pattern
    private static MasterGameManager _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;
    
    public static MasterGameManager Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning("[MasterGameManager] Already destroyed! Returning null.");
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
    #endregion
    
    #region Cached System References
    [Header("⚡ SYSTEM REFERENCES (Auto-Cached)")]
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
    public VehicleSystem Vehicle => _vehicle;
    public GameBalancer Balancer => _balancer;
    public AntiCheat AntiCheat => _antiCheat;
    public MentalHealthSystem MentalHealth => _mentalHealth;
    #endregion
    
    #region Game State
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
    public int TroopCount => _troopCount;
    public int BuildingLevel => _buildingLevel;
    #endregion
    
    private bool _isGameReady = false;
    
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
        _vehicle = FindFirstObjectByType<VehicleSystem>();
        _balancer = FindFirstObjectByType<GameBalancer>();
        _antiCheat = FindFirstObjectByType<AntiCheat>();
        _mentalHealth = FindFirstObjectByType<MentalHealthSystem>();
        
        LoadGameData();
    }
    
    void Start()
    {
        _isGameReady = true;
        StartCoroutine(DelayedStart());
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (_tutorialSystem != null && !_tutorialSystem.IsTutorialComplete())
        {
            _tutorialSystem.StartTutorial();
        }
        
        UpdateAllUI();
    }
    
    #region Core Loop Methods
    public void GatherResource(string type, int amount)
    {
        if (!_isGameReady) return;
        
        switch (type.ToLower())
        {
            case "wood": _wood += amount; break;
            case "stone": _stone += amount; break;
        }
        
        _questManager?.UpdateProgress(type, amount);
        _tutorialSystem?.CheckAction("gather");
        UpdateResourceUI();
        SaveGameData();
    }
    
    public void UpgradeBuilding()
    {
        if (!_isGameReady) return;
        
        int cost = 50 * _buildingLevel;
        
        if (_currency != null && _currency.SpendCoins(cost))
        {
            _buildingLevel++;
            _uiManager?.ShowNotification($"🏗️ Building level {_buildingLevel}!", Color.green);
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
            _uiManager?.ShowNotification($"⚔️ Troops: {_troopCount}", Color.cyan);
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
            _uiManager?.ShowNotification("❌ No troops!", Color.red);
            return;
        }
        
        int playerPower = _troopCount * 20 + _buildingLevel * 10;
        int enemyPower = Random.Range(30, 80);
        
        bool isVictory = playerPower > enemyPower;
        
        if (isVictory)
        {
            int loot = Random.Range(30, 100);
            _wood += loot;
            _raidsCompleted++;
            AddXP(20);
            _uiManager?.ShowNotification($"🏆 VICTORY! +{loot} wood!", Color.green);
            _tutorialSystem?.CheckAction("raid");
        }
        else
        {
            _troopCount = Mathf.Max(0, _troopCount - 1);
            _uiManager?.ShowNotification($"💀 DEFEAT! Lost 1 troop!", Color.red);
        }
        
        UpdateAllUI();
        SaveGameData();
    }
    
    public void OpenChest()
    {
        int reward = Random.Range(20, 100);
        _coins += reward;
        _uiManager?.ShowNotification($"🎁 +{reward} coins from chest!", Color.magenta);
        _tutorialSystem?.CheckAction("chest");
        UpdateAllUI();
        SaveGameData();
    }
    #endregion
    
    void AddXP(int amount)
    {
        _currentXP += amount;
        int needed = _playerLevel * 100;
        
        if (_currentXP >= needed)
        {
            _currentXP -= needed;
            _playerLevel++;
            _uiManager?.ShowNotification($"🎉 LEVEL {_playerLevel}!", Color.yellow);
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
    
    void UpdateResourceUI() => _uiManager?.UpdateResources(_wood, _stone, _coins, _gems);
    void UpdateBuildingUI() => _uiManager?.UpdateStatus(_playerLevel, _troopCount, 0);
    void UpdateTroopUI() => _uiManager?.UpdateTroopCount(_troopCount);
    void UpdateLevelUI() => _uiManager?.UpdateLevel(_playerLevel, _currentXP, _playerLevel * 100);
    
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
        _applicationIsQuitting = true;
        SaveGameData();
    }
    
    void OnApplicationPause(bool pause)
    {
        if (pause) SaveGameData();
    }
    
    public bool IsGameReady() => _isGameReady;
}
