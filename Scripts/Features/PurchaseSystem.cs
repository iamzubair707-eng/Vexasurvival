using UnityEngine;
using System.Collections.Generic;

public class PurchaseSystem : MonoBehaviour
{
    private CurrencyManager currency;
    
    public List<ShopItemData> shopItems = new List<ShopItemData>();
    
    [System.Serializable]
    public class ShopItemData
    {
        public string itemName;
        public string category; // costume, slave, boost, special
        public int coinCost;
        public int gemCost;
        public string itemId;
    }
    
    void Start()
    {
        currency = GetComponent<CurrencyManager>();
        InitializeShop();
    }
    
    void InitializeShop()
    {
        // Costumes
        shopItems.Add(new ShopItemData { itemName = "Ninja Costume", category = "costume", coinCost = 500, gemCost = 50, itemId = "costume_ninja" });
        shopItems.Add(new ShopItemData { itemName = "Knight Armor", category = "costume", coinCost = 800, gemCost = 80, itemId = "costume_knight" });
        shopItems.Add(new ShopItemData { itemName = "Dragon Lord", category = "costume", coinCost = 2000, gemCost = 200, itemId = "costume_dragon" });
        
        // Slaves/Workers
        shopItems.Add(new ShopItemData { itemName = "Wood Worker", category = "slave", coinCost = 300, gemCost = 30, itemId = "slave_wood" });
        shopItems.Add(new ShopItemData { itemName = "Stone Worker", category = "slave", coinCost = 300, gemCost = 30, itemId = "slave_stone" });
        shopItems.Add(new ShopItemData { itemName = "Master Builder", category = "slave", coinCost = 1000, gemCost = 100, itemId = "slave_builder" });
        
        // Boosts
        shopItems.Add(new ShopItemData { itemName = "2X Resources (1hr)", category = "boost", coinCost = 150, gemCost = 15, itemId = "boost_resources" });
        shopItems.Add(new ShopItemData { itemName = "Shield (24hrs)", category = "boost", coinCost = 500, gemCost = 50, itemId = "boost_shield" });
        shopItems.Add(new ShopItemData { itemName = "Raid Boost", category = "boost", coinCost = 300, gemCost = 30, itemId = "boost_raid" });
        
        // Special items
        shopItems.Add(new ShopItemData { itemName = "Mystery Chest", category = "special", coinCost = 200, gemCost = 20, itemId = "special_chest" });
        shopItems.Add(new ShopItemData { itemName = "Legendary Pet", category = "special", coinCost = 5000, gemCost = 500, itemId = "special_pet" });
    }
    
    public bool PurchaseItem(string itemId, string currencyType)
    {
        ShopItemData item = shopItems.Find(i => i.itemId == itemId);
        if (item == null) return false;
        
        bool success = false;
        
        if (currencyType == "coins")
        {
            success = currency.SpendCoins(item.coinCost);
        }
        else if (currencyType == "gems")
        {
            success = currency.SpendGems(item.gemCost);
        }
        
        if (success)
        {
            UnlockItem(item);
            DebugLogger.Log($" Purchased: {item.itemName} with {currencyType}!");
        }
        else
        {
            DebugLogger.Log($"❌ Failed to purchase {item.itemName}. Not enough {currencyType}!");
        }
        
        return success;
    }
    
    void UnlockItem(ShopItemData item)
    {
        switch (item.category)
        {
            case "costume":
                PlayerPrefs.SetInt($"Costume_{item.itemName}", 1);
                break;
            case "slave":
                int currentCount = PlayerPrefs.GetInt($"Slave_{item.itemName}", 0);
                PlayerPrefs.SetInt($"Slave_{item.itemName}", currentCount + 1);
                break;
            case "boost":
                PlayerPrefs.SetInt($"Boost_{item.itemId}", 1);
                PlayerPrefs.SetFloat($"Boost_{item.itemId}_EndTime", Time.time + 3600);
                break;
            case "special":
                PlayerPrefs.SetInt($"Special_{item.itemName}", 1);
                break;
        }
        
        // Show notification
        NotificationManager notif = GetComponent<NotificationManager>();
        if (notif != null)
            notif.ShowNotification($" {item.itemName} Unlocked!", "success");
    }
    
    public void ShowShop()
    {
        DebugLogger.Log("");
        DebugLogger.Log("🛒 ITEM SHOP");
        DebugLogger.Log("");
        
        DebugLogger.Log("👔 COSTUMES");
        foreach (var item in shopItems.FindAll(i => i.category == "costume"))
            DebugLogger.Log($"   {item.itemName} — {item.coinCost}🪙 / {item.gemCost}💎");
        
        DebugLogger.Log("\n👥 WORKERS");
        foreach (var item in shopItems.FindAll(i => i.category == "slave"))
            DebugLogger.Log($"   {item.itemName} — {item.coinCost}🪙 / {item.gemCost}💎");
        
        DebugLogger.Log("\n⚡ BOOSTS");
        foreach (var item in shopItems.FindAll(i => i.category == "boost"))
            DebugLogger.Log($"   {item.itemName} — {item.coinCost}🪙 / {item.gemCost}💎");
        
        DebugLogger.Log("\n✨ SPECIAL");
        foreach (var item in shopItems.FindAll(i => i.category == "special"))
            DebugLogger.Log($"   {item.itemName} — {item.coinCost}🪙 / {item.gemCost}💎");
        
        DebugLogger.Log("");
    }
}