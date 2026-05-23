using UnityEngine;
using System;

public class MentalHealthSystem : MonoBehaviour
{
    public static MentalHealthSystem Instance;
    
    public float mentalHealth = 100f;
    public float maxMentalHealth = 100f;
    public MentalState currentState = MentalState.Stable;
    
    public enum MentalState
    {
        Stable,      // Normal - 100% efficiency
        Stressed,    // -20% production, slower movement
        Depressed,   // -50% production, no raids
        Rebellious,  // May refuse to work or attack
        Insane       // Random actions, may leave base
    }
    
    public event Action<MentalState> OnMentalStateChanged;
    
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
    
    void Start()
    {
        LoadMentalHealth();
        InvokeRepeating("DecayMentalHealth", 60f, 60f); // Every minute
    }
    
    public void AddTrauma(string eventName, int amount)
    {
        mentalHealth -= amount;
        mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMentalHealth);
        
        UpdateMentalState();
        SaveMentalHealth();
        
        string effect = GetCurrentEffect();
        MasterGameManager.Instance?.UIManager?.ShowNotification($"🧠 {eventName}! Mental: {mentalHealth:F0}%", Color.red);
        DebugLogger.Log($"🧠 Trauma: {eventName} -{amount} (Now: {mentalHealth:F0}) - {effect}");
    }
    
    public void HealMentalHealth(int amount)
    {
        mentalHealth += amount;
        mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMentalHealth);
        UpdateMentalState();
        SaveMentalHealth();
        
        MasterGameManager.Instance?.UIManager?.ShowNotification($"💚 Mental health +{amount}%!", Color.green);
    }
    
    void UpdateMentalState()
    {
        MentalState newState;
        
        if (mentalHealth >= 70)
            newState = MentalState.Stable;
        else if (mentalHealth >= 40)
            newState = MentalState.Stressed;
        else if (mentalHealth >= 20)
            newState = MentalState.Depressed;
        else if (mentalHealth >= 5)
            newState = MentalState.Rebellious;
        else
            newState = MentalState.Insane;
        
        if (newState != currentState)
        {
            currentState = newState;
            OnMentalStateChanged?.Invoke(currentState);
            ApplyStateEffects();
            
            MasterGameManager.Instance?.UIManager?.ShowNotification($"⚠️ Survivors are {currentState}!", Color.yellow);
        }
    }
    
    void ApplyStateEffects()
    {
        switch (currentState)
        {
            case MentalState.Stressed:
                DebugLogger.Log("😟 Stressed: -20% production, slower movement");
                break;
            case MentalState.Depressed:
                DebugLogger.Log("😔 Depressed: -50% production, cannot raid");
                break;
            case MentalState.Rebellious:
                DebugLogger.Log("⚠️ Rebellious: May refuse to work!");
                TriggerRebellion();
                break;
            case MentalState.Insane:
                DebugLogger.Log("💀 Insane: Random actions!");
                TriggerInsanity();
                break;
        }
    }
    
    void TriggerRebellion()
    {
        if (UnityEngine.Random.Range(0, 100) < 30)
        {
            int stolen = UnityEngine.Random.Range(10, 50);
            MasterGameManager.Instance?.Resources?.SpendResource("scrap", stolen);
            MasterGameManager.Instance?.UIManager?.ShowNotification($"💔 Survivors stole {stolen} resources!", Color.red);
        }
    }
    
    void TriggerInsanity()
    {
        int randomAction = UnityEngine.Random.Range(0, 100);
        if (randomAction < 20)
        {
            // Random resource loss
            MasterGameManager.Instance?.Resources?.SpendResource("scrap", 20);
            MasterGameManager.Instance?.UIManager?.ShowNotification("🤪 Survivors went crazy! Lost 20 scrap!", Color.red);
        }
    }
    
    void DecayMentalHealth()
    {
        if (currentState != MentalState.Stable)
        {
            mentalHealth -= 2f;
            mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMentalHealth);
            UpdateMentalState();
            SaveMentalHealth();
        }
    }
    
    public string GetCurrentEffect()
    {
        switch (currentState)
        {
            case MentalState.Stressed: return "-20% production";
            case MentalState.Depressed: return "-50% production, no raids";
            case MentalState.Rebellious: return "May steal resources";
            case MentalState.Insane: return "Random actions";
            default: return "Normal operations";
        }
    }
    
    public void SendToCounseling()
    {
        if (MasterGameManager.Instance?.Resources?.SpendResource("scrap", 50) == true)
        {
            HealMentalHealth(30);
            DebugLogger.Log("💚 Counseling helped!");
        }
        else
        {
            MasterGameManager.Instance?.UIManager?.ShowNotification("❌ Need 50 scrap for counseling!", Color.red);
        }
    }
    
    void SaveMentalHealth()
    {
        PlayerPrefs.SetFloat("MentalHealth", mentalHealth);
        PlayerPrefs.SetInt("MentalState", (int)currentState);
    }
    
    void LoadMentalHealth()
    {
        mentalHealth = PlayerPrefs.GetFloat("MentalHealth", 100f);
        currentState = (MentalState)PlayerPrefs.GetInt("MentalState", 0);
    }
    
    void OnGUI()
    {
        if (MasterGameManager.Instance == null) return;
        
        GUI.Box(new Rect(10, Screen.height - 80, 200, 60), "🧠 MENTAL STATUS");
        GUI.Label(new Rect(20, Screen.height - 60, 180, 20), $"Health: {mentalHealth:F0}%");
        GUI.Label(new Rect(20, Screen.height - 40, 180, 20), $"State: {currentState}");
        GUI.Label(new Rect(20, Screen.height - 20, 180, 20), $"Effect: {GetCurrentEffect()}");
    }
}
