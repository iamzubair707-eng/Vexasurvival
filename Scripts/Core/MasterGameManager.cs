using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterGameManager : MonoBehaviour
{
    #region Singleton (Thread-Safe)
    private static MasterGameManager _instance;
    private static readonly object _lock = new object();
    private static bool _quitting = false;
    
    public static MasterGameManager Instance
    {
        get
        {
            if (_quitting)
            {
                Debug.LogWarning("MasterGameManager already destroyed!");
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
    
    #region Cached System References (No FindObjectOfType needed!)
    [Header("⚡ SYSTEM REFERENCES")]
    public CoreResources Resources { get; private set; }
    public CurrencyManager Currency { get; private set; }
    public BuildingSystem BuildingSystem { get; private set; }
    public CombatSystem CombatSystem { get; private set; }
    public PVERaidSystem PVERaid { get; private set; }
    public ChestSystem ChestSystem { get; private set; }
    public QuestManager QuestManager { get; private set; }
    public TutorialSystem TutorialSystem { get; private set; }
    public UIManager UIManager { get; private set; }
    public EnergySystem EnergySystem { get; private set; }
    public AudioManager Audio { get; private set; }
    public VisualManager Visual { get; private set; }
    public VehicleSystem Vehicle { get; private set; }
    public GameBalancer Balancer { get; private set; }
    public AntiCheat AntiCheat { get; private set; }
    public MentalHealthSystem MentalHealth { get; private set; }
    #endregion
    
    #region Game State
    [Header("📊 GAME STATE")]
    [SerializeField] private int _playerLevel = 1;
    [SerializeField] private int _currentXP = 0;
    [SerializeField] private int _troopCount = 0;
    [SerializeField] private int _buildingLevel = 1;
    [SerializeField] private int _coins = 500;
    [SerializeField] private int _gems = 50;
    [SerializeField] private int _wood = 100;
    [SerializeField] private int _stone = 50;
    
    public int PlayerLevel => _playerLevel;
    public int TroopCount => _troopCount;
    public int BuildingLevel => _buildingLevel;
    public int Coins => _coins;
    public int Gems => _gems;
    public int Wood => _wood;
    public int Stone => _stone;
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
        LoadGameData();
    }
    
    void CacheAllSystems()
    {
        // Find all systems ONCE at startup
        Resources = FindFirstObjectByType<CoreResources>();
        Currency = FindFirstObjectByType<CurrencyManager>();
        BuildingSystem = FindFirstObjectByType<BuildingSystem>();
        CombatSystem = FindFirstObjectByType<CombatSystem>();
        PVERaid = FindFirstObjectByType<PVERaidSystem>();
        ChestSystem = FindFirstObjectByType<ChestSystem>();
        QuestManager = FindFirstObjectByType<QuestManager>();
        TutorialSystem = FindFirstObjectByType<TutorialSystem>();
        UIManager = FindFirstObjectByType<UIManager>();
        EnergySystem = FindFirstObjectByType<EnergySystem>();
        Audio = FindFirstObjectByType<AudioManager>();
        Visual = FindFirstObjectByType<VisualManager>();
        Vehicle = FindFirstObjectByType<VehicleSystem>();
        Balancer = FindFirstObjectByType<GameBalancer>();
        AntiCheat = FindFirstObjectByType<AntiCheat>();
        MentalHealth = FindFirstObjectByType<MentalHealthSystem>();
        
        // Auto-create critical systems if missing
        if (Resources == null) Resources = gameObject.AddComponent<CoreResources>();
        if (Currency == null) Currency = gameObject.AddComponent<CurrencyManager>();
        if (UIManager == null) UIManager = gameObject.AddComponent<UIManager>();
    }
    
    void Start()
    {
        _isGameReady = true;
        StartCoroutine(DelayedStart());
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateAllUI();
        
        if (TutorialSystem != null && !TutorialSystem.IsTutorialComplete())
        {
            TutorialSystem.StartTutorial();
        }
    }
    
    #region Core Loop Methods
    public void GatherResource(string type, int amount)
    {
        if (!_isGameReady) return;
        
        switch (type.ToLower())
        {
            case "wood": _wood += amount; break;
            case "stone": _stone += amount; break;
            default: return;
        }
        
        QuestManager?.UpdateProgress(type, amount);
        TutorialSystem?.CheckAction("gather");
        UpdateResourceUI();
        SaveGameData();
    }
    
    public void UpgradeBuilding()
    {
        if (!_isGameReady) return;
        
        int cost = 50 * _buildingLevel;
        
        if (SpendCoins(cost))
        {
            _buildingLevel++;
            UIManager?.ShowNotification($"🏗️ Building level {_buildingLevel}!", Color.green);
            TutorialSystem?.CheckAction("upgrade");
            UpdateBuildingUI();
            SaveGameData();
        }
        else
        {
            UIManager?.ShowNotification($"❌ Need {cost} coins!", Color.red);
        }
    }
    
    public void TrainTroop()
    {
        if (!_isGameReady) return;
        
        int cost = 30;
        
        if (SpendCoins(cost))
        {
            _troopCount++;
            UIManager?.ShowNotification($"⚔️ Troops: {_troopCount}", Color.cyan);
            TutorialSystem?.CheckAction("train");
            UpdateTroopUI();
            SaveGameData();
        }
        else
        {
            UIManager?.ShowNotification($"❌ Need {cost} coins!", Color.red);
        }
    }
    
    public void StartRaid()
    {
        if (_troopCount <= 0)
        {
            UIManager?.ShowNotification("❌ No troops! Train first!", Color.red);
            return;
        }
        
        int playerPower = _troopCount * 20 + _buildingLevel * 10;
        int enemyPower = Random.Range(30, 80);
        bool isVictory = playerPower > enemyPower;
        
        if (isVictory)
        {
            int loot = Random.Range(30, 100);
            _wood += loot;
            AddXP(20);
            UIManager?.ShowNotification($" VICTORY! +{loot} wood!", Color.green);
            TutorialSystem?.CheckAction("raid");
        }
        else
        {
            _troopCount--;
            UIManager?.ShowNotification($"💀 DEFEAT! Lost 1 troop!", Color.red);
        }
        
        UpdateAllUI();
        SaveGameData();
    }
    
    public void OpenChest()
    {
        int reward = Random.Range(20, 100);
        _coins += reward;
        UIManager?.ShowNotification($"🎁 +{reward} coins!", Color.magenta);
        TutorialSystem?.CheckAction("chest");
        UpdateAllUI();
        SaveGameData();
    }
    
    private bool SpendCoins(int amount)
    {
        if (_coins >= amount)
        {
            _coins -= amount;
            return true;
        }
        return false;
    }
    
    private void AddXP(int amount)
    {
        _currentXP += amount;
        int needed = _playerLevel * 100;
        
        if (_currentXP >= needed)
        {
            _currentXP -= needed;
            _playerLevel++;
            UIManager?.ShowNotification($" LEVEL {_playerLevel}!", Color.yellow);
            _coins += 200;
            _gems += 50;
        }
        
        UpdateLevelUI();
    }
    #endregion
    
    #region UI Updates
    private void UpdateAllUI()
    {
        UpdateResourceUI();
        UpdateBuildingUI();
        UpdateTroopUI();
        UpdateLevelUI();
    }
    
    private void UpdateResourceUI() => UIManager?.UpdateResources(_wood, _stone, _coins, _gems);
    private void UpdateBuildingUI() => UIManager?.UpdateStatus(_playerLevel, _troopCount, 0);
    private void UpdateTroopUI() => UIManager?.UpdateTroopCount(_troopCount);
    private void UpdateLevelUI() => UIManager?.UpdateLevel(_playerLevel, _currentXP, _playerLevel * 100);
    #endregion
    
    #region Save/Load
    private void LoadGameData()
    {
        _playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        _currentXP = PlayerPrefs.GetInt("CurrentXP", 0);
        _troopCount = PlayerPrefs.GetInt("TroopCount", 0);
        _buildingLevel = PlayerPrefs.GetInt("BuildingLevel", 1);
        _coins = PlayerPrefs.GetInt("Coins", 500);
        _gems = PlayerPrefs.GetInt("Gems", 50);
        _wood = PlayerPrefs.GetInt("Wood", 100);
        _stone = PlayerPrefs.GetInt("Stone", 50);
    }
    
    private void SaveGameData()
    {
        PlayerPrefs.SetInt("PlayerLevel", _playerLevel);
        PlayerPrefs.SetInt("CurrentXP", _currentXP);
        PlayerPrefs.SetInt("TroopCount", _troopCount);
        PlayerPrefs.SetInt("BuildingLevel", _buildingLevel);
        PlayerPrefs.SetInt("Coins", _coins);
        PlayerPrefs.SetInt("Gems", _gems);
        PlayerPrefs.SetInt("Wood", _wood);
        PlayerPrefs.SetInt("Stone", _stone);
        PlayerPrefs.Save();
    }
    #endregion
    
    void OnApplicationQuit()
    {
        _quitting = true;
        SaveGameData();
    }
    
    void OnApplicationPause(bool pause)
    {
        if (pause) SaveGameData();
    }
    
    public bool IsGameReady() => _isGameReady;
}
