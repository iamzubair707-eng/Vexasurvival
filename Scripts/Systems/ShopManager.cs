using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    private ResourceManager resourceManager;
    public List<ShopItem> shopItems = new List<ShopItem>();
    
    void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        InitializeShop();
    }
    
    void InitializeShop()
    {
        shopItems.Clear();
        
        // Items that can be bought with resources
        shopItems.Add(new ShopItem("speed_1h", "Speed Boost (1 hour)", "vexa", 10, "speed", 3600));
        shopItems.Add(new ShopItem("shield_24h", "Protection Shield (24 hours)", "vexa", 25, "shield", 86400));
        shopItems.Add(new ShopItem("resource_pack", "Resource Pack (100 wood + 50 stone)", "vexa", 15, "resources", 100));
        shopItems.Add(new ShopItem("heal_potion", "Full Health Heal", "vexa", 8, "heal", 100));
        shopItems.Add(new ShopItem("rare_blueprint", "Rare Building Blueprint", "vexa", 50, "blueprint", 1));
        
        // Items that can be bought with wood
        shopItems.Add(new ShopItem("wood_to_stone", "Convert 50 Wood to 25 Stone", "wood", 50, "convert_stone", 25));
        shopItems.Add(new ShopItem("wood_to_food", "Convert 30 Wood to 40 Food", "wood", 30, "convert_food", 40));
        
        // Items that can be bought with stone
        shopItems.Add(new ShopItem("stone_to_wood", "Convert 30 Stone to 40 Wood", "stone", 30, "convert_wood", 40));
    }
    
    public bool BuyItem(string itemId)
    {
        ShopItem item = shopItems.Find(i => i.id == itemId);
        
        if (item == null)
        {
            DebugLogger.Log("Item not found!");
            return false;
        }
        
        // Check if enough currency
        bool canBuy = resourceManager.SpendResource(item.currencyType, item.cost);
        
        if (!canBuy)
        {
            DebugLogger.Log($"Not enough {item.currencyType}!");
            return false;
        }
        
        // Apply item effect
        ApplyItemEffect(item);
        
        DebugLogger.Log($"🛒 Purchased: {item.name} for {item.cost} {item.currencyType}");
        
        // Show notification
        NotificationManager notif = GetComponent<NotificationManager>();
        if (notif != null)
        {
            notif.ShowNotification($"🎁 Purchased: {item.name}!", "success");
        }
        
        return true;
    }
    
    void ApplyItemEffect(ShopItem item)
    {
        switch (item.effectType)
        {
            case "speed":
                DebugLogger.Log($"⚡ Speed boost active for {item.effectValue} seconds!");
                // Implement speed boost logic
                break;
                
            case "shield":
                DebugLogger.Log($"🛡️ Shield active for {item.effectValue} seconds!");
                PlayerPrefs.SetInt("ShieldActive", 1);
                PlayerPrefs.SetFloat("ShieldEndTime", Time.time + item.effectValue);
                break;
                
            case "resources":
                resourceManager.AddResource("wood", 100);
                resourceManager.AddResource("stone", 50);
                DebugLogger.Log("📦 +100 Wood, +50 Stone!");
                break;
                
            case "heal":
                HealthSystem hs = GetComponent<HealthSystem>();
                if (hs != null) hs.Heal(100);
                DebugLogger.Log("💚 Health fully restored!");
                break;
                
            case "blueprint":
                DebugLogger.Log("📜 Rare blueprint unlocked!");
                PlayerPrefs.SetInt("HasRareBlueprint", 1);
                break;
                
            case "convert_stone":
                resourceManager.AddResource("stone", (int)item.effectValue);
                DebugLogger.Log($"🔄 Converted! +{item.effectValue} Stone");
                break;
                
            case "convert_food":
                resourceManager.AddResource("food", (int)item.effectValue);
                DebugLogger.Log($"🔄 Converted! +{item.effectValue} Food");
                break;
                
            case "convert_wood":
                resourceManager.AddResource("wood", (int)item.effectValue);
                DebugLogger.Log($"🔄 Converted! +{item.effectValue} Wood");
                break;
        }
    }
    
    public void ShowShop()
    {
        DebugLogger.Log("========== SHOP ==========");
        DebugLogger.Log("--- VEXA ITEMS ---");
        foreach (ShopItem item in shopItems)
        {
            if (item.currencyType == "vexa")
            {
                DebugLogger.Log($"{item.id}: {item.name} - {item.cost} VEXA");
            }
        }
        
        DebugLogger.Log("--- WOOD ITEMS ---");
        foreach (ShopItem item in shopItems)
        {
            if (item.currencyType == "wood")
            {
                DebugLogger.Log($"{item.id}: {item.name} - {item.cost} Wood");
            }
        }
        
        DebugLogger.Log("--- STONE ITEMS ---");
        foreach (ShopItem item in shopItems)
        {
            if (item.currencyType == "stone")
            {
                DebugLogger.Log($"{item.id}: {item.name} - {item.cost} Stone");
            }
        }
        DebugLogger.Log("==================");
    }
    
    public bool IsShieldActive()
    {
        float endTime = PlayerPrefs.GetFloat("ShieldEndTime", 0);
        return endTime > Time.time;
    }
}

[System.Serializable]
public class ShopItem
{
    public string id;
    public string name;
    public string currencyType; // vexa, wood, stone
    public int cost;
    public string effectType;
    public float effectValue;
    
    public ShopItem(string id, string name, string currency, int cost, string effect, float value)
    {
        this.id = id;
        this.name = name;
        this.currencyType = currency;
        this.cost = cost;
        this.effectType = effect;
        this.effectValue = value;
    }
}