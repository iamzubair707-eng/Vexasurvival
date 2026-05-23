using UnityEngine;
using UnityEngine.UI;

public class CoreLoopController : MonoBehaviour
{
    [Header("UI References")]
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
    
    private CoreResources resources;
    private CurrencyManager currency;
    private BuildingManager buildings;
    private PVERaidSystem pveRaid;
    private ChestSystem chests;
    
    int troopCount = 0;
    int buildingLevel = 1;
    
    void Start()
    {
        resources = FindObjectOfType<CoreResources>();
        currency = FindObjectOfType<CurrencyManager>();
        buildings = FindObjectOfType<BuildingManager>();
        pveRaid = FindObjectOfType<PVERaidSystem>();
        chests = FindObjectOfType<ChestSystem>();
        
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
        InvokeRepeating("UpdateUI", 0f, 1f);
    }
    
    void GatherResource(string type, int amount)
    {
        if (resources != null)
        {
            resources.AddResource(type, amount);
            Debug.Log($"📦 Gathered {amount} {type}!");
            
            // Visual feedback
            if (AudioManager.Instance != null)
                AudioManager.Instance.CoinCollect();
            
            UpdateUI();
        }
    }
    
    void UpgradeBuilding()
    {
        int cost = 50 * buildingLevel;
        
        if (currency != null && currency.SpendCoins(cost))
        {
            buildingLevel++;
            Debug.Log($"🏗️ Building upgraded to level {buildingLevel}!");
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.BuildComplete();
            
            UpdateUI();
        }
        else
        {
            Debug.Log("❌ Not enough coins to upgrade!");
        }
    }
    
    void TrainTroop()
    {
        int cost = 30;
        
        if (currency != null && currency.SpendCoins(cost))
        {
            troopCount++;
            Debug.Log($"⚔️ Troop trained! Total troops: {troopCount}");
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.ButtonClick();
            
            UpdateUI();
        }
        else
        {
            Debug.Log("❌ Not enough coins to train troop!");
        }
    }
    
    void StartRaid()
    {
        if (troopCount <= 0)
        {
            Debug.Log("❌ No troops to send on raid! Train some first!");
            return;
        }
        
        if (pveRaid != null)
        {
            pveRaid.StartRaid(PVERaidSystem.RaidType.ZombieHorde);
            Debug.Log($"⚔️ Raid started with {troopCount} troops!");
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.RaidStart();
        }
    }
    
    void OpenChest()
    {
        if (chests != null)
        {
            var reward = chests.OpenChest();
            if (reward != null)
            {
                Debug.Log($"🎁 Chest opened! Got: {reward.rewardType} x{reward.amount}");
                
                if (AudioManager.Instance != null)
                    AudioManager.Instance.CoinCollect();
                
                UpdateUI();
            }
        }
    }
    
    void UpdateUI()
    {
        if (resourcesText != null && resources != null)
        {
            resourcesText.text = $"Wood:{resources.scrap} Stone:{resources.fuel}";
        }
        
        if (buildingsText != null)
        {
            buildingsText.text = $"Building Level: {buildingLevel}";
        }
        
        if (troopsText != null)
        {
            troopsText.text = $"Troops: {troopCount}";
        }
        
        if (raidStatusText != null && pveRaid != null)
        {
            raidStatusText.text = $"Raid Energy: {pveRaid.raidEnergy}/{pveRaid.maxRaidEnergy}";
        }
    }
}