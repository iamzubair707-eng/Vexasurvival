using UnityEngine;

public class MasterGameManager : MonoBehaviour
{
    public static MasterGameManager Instance;
    
    [Header("All Systems - Auto Connect")]
    public CoreResources resources;
    public CurrencyManager currency;
    public BuildingManager buildings;
    public CombatSystem combat;
    public RaidSystem pvpRaid;
    public PVERaidSystem pveRaid;
    public ClanSystem clan;
    public QuestManager quests;
    public ShopManager shop;
    public ChestSystem chests;
    public DailyLoginStreak dailyStreak;
    public BattlePassManager battlePass;
    public EventManager events;
    public DynamicEventManager dynamicEvents;
    public NotificationManager notifications;
    public UIManager ui;
    public AudioManager audio;
    public VisualManager visuals;
    public AntiCheat antiCheat;
    public CompleteSaveSystem saveSystem;
    public TutorialSystem tutorial;
    public UserProfile userProfile;
    public RelationshipManager relationships;
    public MoralChoiceManager moralChoices;
    public DeadlineSystem deadlines;
    public VehicleManager vehicles;
    public DefenseSystem defense;
    public EnergySystem energy;
    public OfflineRewards offlineRewards;
    public MentalHealthSystem mentalHealth;
    public GameBalancer balancer;
    public PerformanceOptimizer optimizer;
    public RevengeSystem revenge;
    
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
        Debug.Log("🎮 Initializing VEXA SURVIVAL - All Systems Connected!");
        Debug.Log($"✅ Total Systems Active: {CountActiveSystems()}");
    }
    
    int CountActiveSystems()
    {
        int count = 0;
        if (resources != null) count++;
        if (currency != null) count++;
        if (buildings != null) count++;
        if (combat != null) count++;
        if (pvpRaid != null) count++;
        if (pveRaid != null) count++;
        if (clan != null) count++;
        if (quests != null) count++;
        if (shop != null) count++;
        if (chests != null) count++;
        if (dailyStreak != null) count++;
        if (battlePass != null) count++;
        if (events != null) count++;
        if (dynamicEvents != null) count++;
        if (notifications != null) count++;
        if (ui != null) count++;
        if (audio != null) count++;
        if (visuals != null) count++;
        if (antiCheat != null) count++;
        if (saveSystem != null) count++;
        if (tutorial != null) count++;
        if (userProfile != null) count++;
        if (relationships != null) count++;
        if (moralChoices != null) count++;
        if (deadlines != null) count++;
        if (vehicles != null) count++;
        if (defense != null) count++;
        if (energy != null) count++;
        if (offlineRewards != null) count++;
        if (mentalHealth != null) count++;
        if (balancer != null) count++;
        if (optimizer != null) count++;
        if (revenge != null) count++;
        return count;
    }
    
    void Start()
    {
        // Start tutorial if first time
        if (tutorial != null && PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            tutorial.StartTutorial();
        }
        
        // Load save data
        if (saveSystem != null)
        {
            saveSystem.LoadGame();
        }
        
        // Start daily streak check
        if (dailyStreak != null)
        {
            dailyStreak.CheckDailyLogin();
        }
        
        Debug.Log("🚀 Game Ready! Total Systems: " + CountActiveSystems());
    }
    
    void OnApplicationQuit()
    {
        if (saveSystem != null)
        {
            saveSystem.SaveGame();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && saveSystem != null)
        {
            saveSystem.SaveGame();
        }
        else if (!pauseStatus && offlineRewards != null)
        {
            offlineRewards.CheckOfflineRewards();
        }
    }
}