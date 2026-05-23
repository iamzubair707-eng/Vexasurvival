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
    
    // Direct access through MasterGameManager
    public static MasterGameManager GameManager => MasterGameManager.Instance;
    public static CoreResources Resources => GameManager?.Resources;
    public static CurrencyManager Currency => GameManager?.Currency;
    public static UIManager UI => GameManager?.UIManager;
    public static BuildingSystem Building => GameManager?.BuildingSystem;
    public static CombatSystem Combat => GameManager?.CombatSystem;
    public static PVERaidSystem PVERaid => GameManager?.PVERaid;
    public static ChestSystem Chest => GameManager?.ChestSystem;
}
