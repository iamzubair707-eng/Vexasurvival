using UnityEngine;

public class GameTester : MonoBehaviour
{
    private ResourceManager resourceManager;
    private HealthSystem healthSystem;
    private BuildingManager buildingManager;
    private RaidSystem raidSystem;
    private ClanSystem clanSystem;
    private QuestManager questManager;
    private ShopManager shopManager;
    private EventManager eventManager;
    private BattlePassManager battlePassManager;
    private DailyReward dailyReward;
    
    void Start()
    {
        // Get all components
        resourceManager = GetComponent<ResourceManager>();
        healthSystem = GetComponent<HealthSystem>();
        buildingManager = GetComponent<BuildingManager>();
        raidSystem = GetComponent<RaidSystem>();
        clanSystem = GetComponent<ClanSystem>();
        questManager = GetComponent<QuestManager>();
        shopManager = GetComponent<ShopManager>();
        eventManager = GetComponent<EventManager>();
        battlePassManager = GetComponent<BattlePassManager>();
        dailyReward = GetComponent<DailyReward>();
        
        DebugLogger.Log("========== 🧪 GAME TESTER ACTIVATED 🧪 ==========");
    }
    
    void Update()
    {
        // Test shortcuts (only in development)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestResourceSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestHealthSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestRaidSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestQuestSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestShopSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            TestEventSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            TestBattlePass();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TestClanSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ShowAllStatus();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetTestData();
        }
    }
    
    void TestResourceSystem()
    {
        DebugLogger.Log("=== TESTING RESOURCE SYSTEM ===");
        
        // Add resources
        resourceManager.AddResource("wood", 100);
        resourceManager.AddResource("stone", 50);
        resourceManager.AddResource("food", 75);
        resourceManager.AddResource("vexa", 25);
        
        DebugLogger.Log("✅ Added: 100 Wood, 50 Stone, 75 Food, 25 VEXA");
        
        // Spend some
        resourceManager.SpendResource("wood", 30);
        resourceManager.SpendResource("vexa", 5);
        
        DebugLogger.Log("✅ Spent: 30 Wood, 5 VEXA");
        
        // Try to spend more than available
        bool result = resourceManager.SpendResource("vexa", 1000);
        DebugLogger.Log($"Trying to spend 1000 VEXA (should fail): {(result ? "FAILED" : "✅ PASS")}");
    }
    
    void TestHealthSystem()
    {
        DebugLogger.Log("=== TESTING HEALTH SYSTEM ===");
        
        healthSystem.TakeDamage(30);
        DebugLogger.Log("✅ Took 30 damage");
        
        healthSystem.Heal(20);
        DebugLogger.Log("✅ Healed 20 HP");
        
        // Test death
        healthSystem.TakeDamage(200);
        DebugLogger.Log("✅ Took 200 damage (should trigger death)");
    }
    
    void TestRaidSystem()
    {
        DebugLogger.Log("=== TESTING RAID SYSTEM ===");
        raidSystem.StartRaid("test_player_123");
        DebugLogger.Log("✅ Raid executed");
    }
    
    void TestQuestSystem()
    {
        DebugLogger.Log("=== TESTING QUEST SYSTEM ===");
        questManager.UpdateProgress("wood", 50);
        questManager.UpdateProgress("raid", 1);
        questManager.ShowQuests();
        DebugLogger.Log("✅ Quest progress updated");
    }
    
    void TestShopSystem()
    {
        DebugLogger.Log("=== TESTING SHOP SYSTEM ===");
        shopManager.ShowShop();
        shopManager.BuyItem("heal_potion");
        shopManager.BuyItem("resource_pack");
        DebugLogger.Log("✅ Shop tested");
    }
    
    void TestEventSystem()
    {
        DebugLogger.Log("=== TESTING EVENT SYSTEM ===");
        eventManager.StartDoubleRewardEvent();
        DebugLogger.Log("✅ Double reward event started");
        
        int bonus = eventManager.GetBonusReward(100);
        DebugLogger.Log($"Bonus reward test: 100 → {bonus}");
    }
    
    void TestBattlePass()
    {
        DebugLogger.Log("=== TESTING BATTLE PASS ===");
        battlePassManager.AddXP(50);
        battlePassManager.AddXP(80);
        battlePassManager.ShowBattlePassUI();
        DebugLogger.Log("✅ Battle Pass tested");
    }
    
    void TestClanSystem()
    {
        DebugLogger.Log("=== TESTING CLAN SYSTEM ===");
        clanSystem.CreateClan("TestClan", "Player1");
        clanSystem.JoinClan("TestClan", "Player2");
        clanSystem.AddClanVexa(100);
        clanSystem.ShowClanLeaderboard();
        DebugLogger.Log("✅ Clan system tested");
    }
    
    void ShowAllStatus()
    {
        DebugLogger.Log("========== 📊 ALL SYSTEMS STATUS 📊 ==========");
        DebugLogger.Log($"💰 Resources: Wood={resourceManager?.wood}, Stone={resourceManager?.stone}, Food={resourceManager?.food}, VEXA={resourceManager?.vexaTokens}");
        DebugLogger.Log($"❤️ Health: {healthSystem?.maxHealth}");
        DebugLogger.Log($"🏆 Battle Pass: Level {battlePassManager?.currentLevel}, XP={battlePassManager?.currentXP}");
        DebugLogger.Log($"🎯 Active Event: {eventManager?.GetActiveEventMessage()}");
        DebugLogger.Log($"🛡️ Shield Active: {shopManager?.IsShieldActive()}");
        questManager?.ShowQuests();
        shopManager?.ShowShop();
        DebugLogger.Log("===============================================");
    }
    
    void ResetTestData()
    {
        DebugLogger.Log("⚠️ Resetting all test data...");
        PlayerPrefs.DeleteAll();
        DebugLogger.Log("✅ All data reset! Restart the game to see effects.");
    }
    
    void OnGUI()
    {
        // Show test instructions on screen
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        
        GUI.Box(new Rect(10, 10, 300, 250), "🧪 TEST CONTROLS");
        GUI.Label(new Rect(20, 40, 280, 20), "Press 1 → Test Resources", style);
        GUI.Label(new Rect(20, 65, 280, 20), "Press 2 → Test Health", style);
        GUI.Label(new Rect(20, 90, 280, 20), "Press 3 → Test Raid", style);
        GUI.Label(new Rect(20, 115, 280, 20), "Press 4 → Test Quests", style);
        GUI.Label(new Rect(20, 140, 280, 20), "Press 5 → Test Shop", style);
        GUI.Label(new Rect(20, 165, 280, 20), "Press 6 → Test Events", style);
        GUI.Label(new Rect(20, 190, 280, 20), "Press 7 → Test Battle Pass", style);
        GUI.Label(new Rect(20, 215, 280, 20), "Press 8 → Test Clan", style);
        GUI.Label(new Rect(20, 240, 280, 20), "Press 9 → Show All Status", style);
        GUI.Label(new Rect(20, 265, 280, 20), "Press 0 → Reset All Data", style);
    }
}
void OnGUI()
{
    GUI.Box(new Rect(10, 10, 200, 100), "VEXA SURVIVAL\nPress 1-9 to test");
}