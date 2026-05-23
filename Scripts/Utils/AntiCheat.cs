using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class AntiCheat : MonoBehaviour
{
    public static AntiCheat Instance;
    
    private string secretKey = "VexaSurvival2024";
    private int currentSessionId;
    
    // Speed hack detection
    private float lastTime;
    private float lastRealTime;
    private int speedHackWarnings = 0;
    
    // Memory tampering detection
    private int expectedCoins;
    private int expectedGems;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentSessionId = Random.Range(10000, 99999);
            lastTime = Time.realtimeSinceStartup;
            lastRealTime = Time.realtimeSinceStartup;
        }
    }
    
    void Update()
    {
        DetectSpeedHack();
    }
    
    void DetectSpeedHack()
    {
        float currentTime = Time.realtimeSinceStartup;
        float deltaTime = currentTime - lastTime;
        float realDeltaTime = Time.unscaledDeltaTime;
        
        if (realDeltaTime > 0.2f && deltaTime < 0.01f)
        {
            speedHackWarnings++;
            DebugLogger.LogWarning($"⚠️ Possible speed hack detected! Warning {speedHackWarnings}/3");
            
            if (speedHackWarnings >= 3)
            {
                DebugLogger.LogError("🔒 ANTI-CHEAT: Speed hack detected! Applying penalty.");
                ApplyPenalty();
            }
        }
        
        lastTime = currentTime;
    }
    
    public void ValidateResourceIntegrity(int currentCoins, int currentGems)
    {
        // Check for impossible resource amounts
        if (currentCoins > 999999 || currentGems > 99999)
        {
            DebugLogger.LogError("🔒 ANTI-CHEAT: Impossible resource amount detected!");
            ApplyPenalty();
        }
        
        // Check for negative values
        if (currentCoins < 0 || currentGems < 0)
        {
            DebugLogger.LogError("🔒 ANTI-CHEAT: Negative resources detected!");
            ResetToSafeValues();
        }
    }
    
    public bool ValidateTransaction(int amount, string type)
    {
        // Prevent impossible transactions
        if (amount > 10000)
        {
            DebugLogger.LogWarning($"⚠️ Suspicious transaction: {amount} {type}");
            return false;
        }
        return true;
    }
    
    void ApplyPenalty()
    {
        CurrencyManager currency = GetComponent<CurrencyManager>();
        if (currency != null)
        {
            currency.coins = Mathf.FloorToInt(currency.coins * 0.5f);
            currency.gems = Mathf.FloorToInt(currency.gems * 0.5f);
            DebugLogger.Log("⚠️ Penalty applied: Resources reduced by 50%");
        }
        
        // Add cooldown period
        PlayerPrefs.SetFloat("AntiCheatPenaltyEnd", Time.time + 3600);
        
        // Log for server-side tracking (if you add server later)
        LogCheatAttempt();
    }
    
    void ResetToSafeValues()
    {
        CurrencyManager currency = GetComponent<CurrencyManager>();
        if (currency != null)
        {
            currency.coins = 100;
            currency.gems = 10;
        }
    }
    
    void LogCheatAttempt()
    {
        string log = $"[{System.DateTime.Now}] Cheat attempt detected! Session: {currentSessionId}\n";
        System.IO.File.AppendAllText(Application.persistentDataPath + "/cheat_log.txt", log);
    }
    
    public bool IsPenaltyActive()
    {
        float penaltyEnd = PlayerPrefs.GetFloat("AntiCheatPenaltyEnd", 0);
        return penaltyEnd > Time.time;
    }
}