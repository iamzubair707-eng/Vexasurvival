using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Core Systems
    public ResourceManager ResourceManager { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }
    public DataManager DataManager { get; private set; }
    public UIManager UIManager { get; private set; }
    
    // Game Systems
    public BuildingSystem BuildingSystem { get; private set; }
    public RaidSystem RaidSystem { get; private set; }
    public CombatSystem CombatSystem { get; private set; }
    public QuestSystem QuestSystem { get; private set; }
    public ChestSystem ChestSystem { get; private set; }
    public ClanSystem ClanSystem { get; private set; }
    
    // State
    public GameState CurrentState { get; private set; } = GameState.Running;
    public bool IsInitialized { get; private set; } = false;
    
    public enum GameState
    {
        Running,
        Paused,
        GameOver
    }
    
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
        Debug.Log("🚀 Initializing VEXA SURVIVAL...");
        
        // Find or create systems
        ResourceManager = GetComponent<ResourceManager>() ?? gameObject.AddComponent<ResourceManager>();
        CurrencyManager = GetComponent<CurrencyManager>() ?? gameObject.AddComponent<CurrencyManager>();
        DataManager = GetComponent<DataManager>() ?? gameObject.AddComponent<DataManager>();
        UIManager = GetComponent<UIManager>() ?? gameObject.AddComponent<UIManager>();
        
        BuildingSystem = GetComponent<BuildingSystem>() ?? gameObject.AddComponent<BuildingSystem>();
        RaidSystem = GetComponent<RaidSystem>() ?? gameObject.AddComponent<RaidSystem>();
        CombatSystem = GetComponent<CombatSystem>() ?? gameObject.AddComponent<CombatSystem>();
        QuestSystem = GetComponent<QuestSystem>() ?? gameObject.AddComponent<QuestSystem>();
        ChestSystem = GetComponent<ChestSystem>() ?? gameObject.AddComponent<ChestSystem>();
        ClanSystem = GetComponent<ClanSystem>() ?? gameObject.AddComponent<ClanSystem>();
        
        IsInitialized = true;
        Debug.Log("✅ All systems initialized!");
        
        // Load saved data
        DataManager?.LoadGame();
    }
    
    public void PauseGame()
    {
        CurrentState = GameState.Paused;
        Time.timeScale = 0;
        UIManager?.ShowPauseMenu();
    }
    
    public void ResumeGame()
    {
        CurrentState = GameState.Running;
        Time.timeScale = 1;
        UIManager?.HidePauseMenu();
    }
    
    void OnApplicationQuit()
    {
        DataManager?.SaveGame();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            DataManager?.SaveGame();
        else
            DataManager?.LoadGame();
    }
}