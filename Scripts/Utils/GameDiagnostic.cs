using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class GameDiagnostic : MonoBehaviour
{
    private static GameDiagnostic Instance;
    private StringBuilder report = new StringBuilder();
    
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    void Update()
    {
        // Press F12 to run full diagnostic
        if (Input.GetKeyDown(KeyCode.F12))
        {
            RunFullDiagnostic();
        }
        
        // Press F11 for quick status
        if (Input.GetKeyDown(KeyCode.F11))
        {
            QuickStatus();
        }
    }
    
    [ContextMenu("Run Full Diagnostic")]
    public void RunFullDiagnostic()
    {
        report.Clear();
        report.AppendLine("═══════════════════════════════════════");
        report.AppendLine("🔍 VEXA SURVIVAL - FULL DIAGNOSTIC REPORT");
        report.AppendLine($"📅 Time: {System.DateTime.Now}");
        report.AppendLine("═══════════════════════════════════════");
        
        CheckCoreSystems();
        CheckEconomySystems();
        CheckCombatSystems();
        CheckSocialSystems();
        CheckVisualAudioSystems();
        CheckPerformanceAntiCheat();
        CheckSaveSystem();
        
        report.AppendLine("═══════════════════════════════════════");
        report.AppendLine("✅ DIAGNOSTIC COMPLETE");
        
        DebugLogger.Log(report.ToString());
        
        // Save to file
        SaveReportToFile();
    }
    
    void QuickStatus()
    {
        DebugLogger.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        DebugLogger.Log("⚡ QUICK STATUS");
        DebugLogger.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        DebugLogger.Log($"🎯 Total Systems: {CountActiveSystems()}");
        DebugLogger.Log($"✅ Active Features: {CountWorkingFeatures()}");
        DebugLogger.Log($"⚠️ Warnings: {CountWarnings()}");
        DebugLogger.Log($"💾 Save Data: {(SaveFileExists() ? "✅ YES" : "❌ NO")}");
        DebugLogger.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
    
    void CheckCoreSystems()
    {
        report.AppendLine("\n📦 CORE SYSTEMS");
        report.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        // Resource System
        CoreResources resources = GetComponent<CoreResources>();
        if (resources != null)
        {
            report.AppendLine($"✅ CoreResources: Food={resources.food}, Water={resources.water}, Scrap={resources.scrap}, Fuel={resources.fuel}");
        }
        else report.AppendLine("❌ CoreResources: MISSING");
        
        // Mental Health
        MentalHealth mental = GetComponent<MentalHealth>();
        if (mental != null)
        {
            report.AppendLine($"✅ MentalHealth: {mental.mentalHealth:F0}/100, State={mental.currentState}");
        }
        else report.AppendLine("❌ MentalHealth: MISSING");
        
        // Player Movement
        PlayerMovement movement = GetComponent<PlayerMovement>();
        report.AppendLine(movement != null ? "✅ PlayerMovement: ACTIVE" : "❌ PlayerMovement: MISSING");
    }
    
    void CheckEconomySystems()
    {
        report.AppendLine("\n💰 ECONOMY SYSTEMS");
        report.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        CurrencyManager currency = GetComponent<CurrencyManager>();
        if (currency != null)
        {
            report.AppendLine($"✅ CurrencyManager: Coins={currency.coins}, Gems={currency.gems}");
        }
        else report.AppendLine("❌ CurrencyManager: MISSING");
        
        // Shop System
        ShopManager shop = GetComponent<ShopManager>();
        report.AppendLine(shop != null ? "✅ ShopManager: ACTIVE" : "❌ ShopManager: MISSING");
        
        // Battle Pass
        BattlePassManager battlePass = GetComponent<BattlePassManager>();
        if (battlePass != null)
        {
            report.AppendLine($"✅ BattlePass: Level={battlePass.currentLevel}, Premium={battlePass.hasPremiumPass}");
        }
        else report.AppendLine("❌ BattlePass: MISSING");
    }
    
    void CheckCombatSystems()
    {
        report.AppendLine("\n⚔️ COMBAT SYSTEMS");
        report.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        PVERaidSystem pveRaid = GetComponent<PVERaidSystem>();
        if (pveRaid != null)
        {
            report.AppendLine($"✅ PVERaidSystem: Energy={pveRaid.raidEnergy}/{pveRaid.maxRaidEnergy}");
        }
        else report.AppendLine("❌ PVERaidSystem: MISSING");
        
        CombatSystem combat = GetComponent<CombatSystem>();
        if (combat != null)
        {
            report.AppendLine($"✅ CombatSystem: RaidPower={combat.CalculateRaidPower()}");
        }
        else report.AppendLine("❌ CombatSystem: MISSING");
        
        RaidSystem raid = GetComponent<RaidSystem>();
        report.AppendLine(raid != null ? "✅ RaidSystem: ACTIVE" : "❌ RaidSystem: MISSING");
        
        DefenseSystem defense = GetComponent<DefenseSystem>();
        report.AppendLine(defense != null ? "✅ DefenseSystem: ACTIVE" : "❌ DefenseSystem: MISSING");
    }
    
    void CheckSocialSystems()
    {
        report.AppendLine("\n👥 SOCIAL SYSTEMS");
        report.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        ClanSystem clan = GetComponent<ClanSystem>();
        report.AppendLine(clan != null ? "✅ ClanSystem: ACTIVE" : "❌ ClanSystem: MISSING");
        
        RelationshipManager relationships = GetComponent<RelationshipManager>();
        report.AppendLine(relationships != null ? "✅ RelationshipManager: ACTIVE" : "❌ RelationshipManager: MISSING");
        
        Leaderboard leaderboard = GetComponent<Leaderboard>();
        report.AppendLine(leaderboard != null ? "✅ Leaderboard: ACTIVE" : "❌ Leaderboard: MISSING");
    }
    
    void CheckVisualAudioSystems()
    {
        report.AppendLine("\n🎨 VISUAL & AUDIO SYSTEMS");
        report.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        VisualManager visual = GetComponent<VisualManager>();
        report.AppendLine(visual != null ? "✅ VisualManager: ACTIVE" : "❌ VisualManager: MISSING");
        
        AudioManager audio = GetComponent<AudioManager>();
        report.AppendLine(audio != null ? "✅ AudioManager: ACTIVE" : "❌ AudioManager: MISSING");
        
        BackgroundScaler bgScaler = GetComponent<BackgroundScaler>();
        report.AppendLine(bgScaler != null ? "✅ BackgroundScaler: ACTIVE" : "❌ BackgroundScaler: MISSING");
    }
    
    void CheckPerformanceAntiCheat()
    {
        report.AppendLine("\n🔒 PERFORMANCE & ANTI-CHEAT");
        report.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        PerformanceOptimizer perf = GetComponent<PerformanceOptimizer>();
        report.AppendLine(perf != null ? "✅ PerformanceOptimizer: ACTIVE" : "❌ PerformanceOptimizer: MISSING");
        
        AntiCheat antiCheat = GetComponent<AntiCheat>();
        report.AppendLine(antiCheat != null ? "✅ AntiCheat: ACTIVE" : "❌ AntiCheat: MISSING");
        
        GameBalancer balancer = GetComponent<GameBalancer>();
        report.AppendLine(balancer != null ? "✅ GameBalancer: ACTIVE" : "❌ GameBalancer: MISSING");
    }
    
    void CheckSaveSystem()
    {
        report.AppendLine("\n💾 SAVE SYSTEM");
        report.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        CompleteSaveSystem save = GetComponent<CompleteSaveSystem>();
        if (save != null)
        {
            report.AppendLine("✅ CompleteSaveSystem: ACTIVE");
            string savePath = Application.persistentDataPath + "/gamesave.json";
            if (System.IO.File.Exists(savePath))
                report.AppendLine($"   📁 Save File: {savePath}");
            else
                report.AppendLine("   📁 Save File: NOT FOUND (New Game)");
        }
        else report.AppendLine("❌ CompleteSaveSystem: MISSING");
    }
    
    int CountActiveSystems()
    {
        int count = 0;
        count += GetComponent<CoreResources>() != null ? 1 : 0;
        count += GetComponent<MentalHealth>() != null ? 1 : 0;
        count += GetComponent<CurrencyManager>() != null ? 1 : 0;
        count += GetComponent<PVERaidSystem>() != null ? 1 : 0;
        count += GetComponent<CombatSystem>() != null ? 1 : 0;
        count += GetComponent<ClanSystem>() != null ? 1 : 0;
        count += GetComponent<RelationshipManager>() != null ? 1 : 0;
        count += GetComponent<VisualManager>() != null ? 1 : 0;
        count += GetComponent<AudioManager>() != null ? 1 : 0;
        count += GetComponent<AntiCheat>() != null ? 1 : 0;
        count += GetComponent<GameBalancer>() != null ? 1 : 0;
        count += GetComponent<PerformanceOptimizer>() != null ? 1 : 0;
        count += GetComponent<CompleteSaveSystem>() != null ? 1 : 0;
        count += GetComponent<UserProfile>() != null ? 1 : 0;
        count += GetComponent<VehicleManager>() != null ? 1 : 0;
        count += GetComponent<DynamicEventManager>() != null ? 1 : 0;
        count += GetComponent<MoralChoiceManager>() != null ? 1 : 0;
        count += GetComponent<TutorialSystem>() != null ? 1 : 0;
        count += GetComponent<DailyLoginStreak>() != null ? 1 : 0;
        count += GetComponent<QuestManager>() != null ? 1 : 0;
        count += GetComponent<BuildingManager>() != null ? 1 : 0;
        count += GetComponent<ShopManager>() != null ? 1 : 0;
        count += GetComponent<NotificationManager>() != null ? 1 : 0;
        count += GetComponent<ChestSystem>() != null ? 1 : 0;
        return count;
    }
    
    int CountWorkingFeatures()
    {
        // Quick check of critical features
        int working = 0;
        if (GetComponent<CoreResources>() != null) working++;
        if (GetComponent<CurrencyManager>() != null) working++;
        if (GetComponent<PVERaidSystem>() != null) working++;
        if (GetComponent<CompleteSaveSystem>() != null) working++;
        if (GetComponent<PlayerMovement>() != null) working++;
        return working;
    }
    
    int CountWarnings()
    {
        int warnings = 0;
        CoreResources res = GetComponent<CoreResources>();
        if (res != null && (res.food < 0 || res.water < 0)) warnings++;
        
        CurrencyManager cur = GetComponent<CurrencyManager>();
        if (cur != null && (cur.coins < 0 || cur.gems < 0)) warnings++;
        
        return warnings;
    }
    
    bool SaveFileExists()
    {
        string path = Application.persistentDataPath + "/gamesave.json";
        return System.IO.File.Exists(path);
    }
    
    void SaveReportToFile()
    {
        string path = Application.persistentDataPath + "/diagnostic_report.txt";
        System.IO.File.WriteAllText(path, report.ToString());
        DebugLogger.Log($"📄 Report saved to: {path}");
    }
    
    // GUI for on-screen diagnostic
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 40), "🔍 RUN DIAGNOSTIC (F12)"))
        {
            RunFullDiagnostic();
        }
        
        if (GUI.Button(new Rect(10, 60, 200, 40), "⚡ QUICK STATUS (F11)"))
        {
            QuickStatus();
        }
        
        // Show basic stats on screen
        GUI.Box(new Rect(Screen.width - 250, 10, 240, 100), "📊 GAME STATUS");
        GUI.Label(new Rect(Screen.width - 240, 35, 230, 20), $"Active Systems: {CountActiveSystems()}");
        GUI.Label(new Rect(Screen.width - 240, 55, 230, 20), $"FPS: {1.0f / Time.deltaTime:F0}");
        GUI.Label(new Rect(Screen.width - 240, 75, 230, 20), $"Save: {(SaveFileExists() ? "✅ YES" : "❌ NO")}");
    }
}