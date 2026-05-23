using UnityEngine;

public class CachedReferences : MonoBehaviour
{
    public static CachedReferences Instance { get; private set; }
    
    public MasterGameManager GameManager;
    public CoreResources Resources;
    public CurrencyManager Currency;
    public UIManager UI;
    public BuildingSystem Building;
    public CombatSystem Combat;
    public VehicleSystem Vehicle;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Assign all references via Inspector or find once
            GameManager = MasterGameManager.Instance;
            Resources = FindObjectOfType<CoreResources>();
            Currency = FindObjectOfType<CurrencyManager>();
            UI = FindObjectOfType<UIManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
