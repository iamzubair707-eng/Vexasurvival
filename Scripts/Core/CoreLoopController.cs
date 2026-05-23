using UnityEngine;
using UnityEngine.UI;

public class CoreLoopController : MonoBehaviour
{
    [Header("UI References - Assign in Inspector")]
    public Button gatherWoodButton;
    public Button gatherStoneButton;
    public Button upgradeBuildingButton;
    public Button trainTroopButton;
    public Button startRaidButton;
    public Button openChestButton;
    
    [Header("Status Texts")]
    public Text resourcesText;
    public Text buildingsText;
    public Text troopsText;
    public Text raidStatusText;
    
    // Direct references - no searching!
    private BuildingManager buildingManager;
    private CoreResources resources;
    private CurrencyManager currency;
    private PVERaidSystem pveRaid;
    private ChestSystem chests;
    
    private int troopCount = 0;
    private int buildingLevel = 1;
    
    void Start()
    {
        // Use Singleton pattern - no FindObjectOfType!
        buildingManager = BuildingManager.Instance;
        resources = MasterGameManager.Instance.Resources;
        currency = MasterGameManager.Instance.Currency;
        pveRaid = PVERaidSystem.Instance;
        chests = ChestSystem.Instance;
        
        // Setup button listeners
        if (gatherWoodButton != null)
            gatherWoodButton.onClick.AddListener(() => GatherResource("wood", 10));
        
        if (gatherStoneButton != null)
            gatherStoneButton.onClick.AddListener(() => GatherResource("stone", 5));
        
        if (upgradeBuildingButton != null)
            upgradeBuildingButton.onClick.AddListener(UpgradeBuilding);
        
        if (trainTroopButton != null)
            trainTroopButton.onClick.AddListener(TrainTroop);
        
        if (startRaidButton != null)
            startRaidButton.onClick.AddListener(StartRaid);
        
        if (openChestButton != null)
            openChestButton.onClick.AddListener(OpenChest);
        
        UpdateUI();
        InvokeRepeating("UpdateUI", 1f, 1f);
    }
    
    void GatherResource(string type, int amount)
    {
        if (resources != null)
        {
            resources.AddResource(type, amount);
            UpdateUI();
        }
    }
    
    void UpgradeBuilding()
    {
        int cost = 50 * buildingLevel;
        
        if (currency != null && currency.SpendCoins(cost))
        {
            buildingLevel++;
            UpdateUI();
        }
    }
    
    void TrainTroop()
    {
        int cost = 30;
        
        if (currency != null && currency.SpendCoins(cost))
        {
            troopCount++;
            UpdateUI();
        }
    }
    
    void StartRaid()
    {
        if (troopCount <= 0) return;
        
        if (pveRaid != null)
        {
            pveRaid.StartRaid(PVERaidSystem.RaidType.ZombieHorde);
            UpdateUI();
        }
    }
    
    void OpenChest()
    {
        if (chests != null)
        {
            chests.OpenChest();
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        if (resourcesText != null && resources != null)
            resourcesText.text = $"Wood:{resources.scrap} Stone:{resources.fuel}";
        
        if (buildingsText != null)
            buildingsText.text = $"Building Level: {buildingLevel}";
        
        if (troopsText != null)
            troopsText.text = $"Troops: {troopCount}";
        
        if (raidStatusText != null && pveRaid != null)
            raidStatusText.text = $"Raid Energy: {pveRaid.raidEnergy}/{pveRaid.maxRaidEnergy}";
    }
}
