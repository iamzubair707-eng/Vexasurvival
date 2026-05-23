using UnityEngine;

public class CachedReferences : MonoBehaviour
{
    public static CachedReferences Instance { get; private set; }
    
    [Header("Cached System References")]
    public MasterGameManager GameManager;
    public CoreResources Resources;
    public CurrencyManager Currency;
    public UIManager UI;
    public BuildingSystem BuildingSystem;
    public CombatSystem CombatSystem;
    public PVERaidSystem PVERaid;
    public ChestSystem ChestSystem;
    public QuestManager QuestManager;
    public TutorialSystem TutorialSystem;
    public EnergySystem EnergySystem;
    public AudioManager Audio;
    public VisualManager Visual;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CacheAllSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CacheAllSystems()
    {
        GameManager = FindObjectOfType<MasterGameManager>();
        Resources = FindObjectOfType<CoreResources>();
        Currency = FindObjectOfType<CurrencyManager>();
        UI = FindObjectOfType<UIManager>();
        BuildingSystem = FindObjectOfType<BuildingSystem>();
        CombatSystem = FindObjectOfType<CombatSystem>();
        PVERaid = FindObjectOfType<PVERaidSystem>();
        ChestSystem = FindObjectOfType<ChestSystem>();
        QuestManager = FindObjectOfType<QuestManager>();
        TutorialSystem = FindObjectOfType<TutorialSystem>();
        EnergySystem = FindObjectOfType<EnergySystem>();
        Audio = FindObjectOfType<AudioManager>();
        Visual = FindObjectOfType<VisualManager>();
        
        DebugLogger.Log("✅ All systems cached!");
    }
    
    public T GetSystem<T>() where T : Component
    {
        switch (typeof(T).Name)
        {
            case nameof(MasterGameManager): return GameManager as T;
            case nameof(CoreResources): return Resources as T;
            case nameof(CurrencyManager): return Currency as T;
            case nameof(UIManager): return UI as T;
            case nameof(BuildingSystem): return BuildingSystem as T;
            case nameof(CombatSystem): return CombatSystem as T;
            case nameof(PVERaidSystem): return PVERaid as T;
            case nameof(ChestSystem): return ChestSystem as T;
            default: return FindObjectOfType<T>();
        }
    }
}
