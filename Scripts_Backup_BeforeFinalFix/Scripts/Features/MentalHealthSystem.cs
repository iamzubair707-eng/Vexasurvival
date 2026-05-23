using UnityEngine;
using System.Collections.Generic;

public class MentalHealthSystem : MonoBehaviour
{
    public static MentalHealthSystem Instance;
    
    public float mentalHealth = 100f; // 0-100
    public float maxMentalHealth = 100f;
    public TraumaState currentTrauma = TraumaState.None;
    
    public enum TraumaState
    {
        None,
        Depression,    // -50% resource production
        PTSD,          // Random panic attacks
        Aggression,    // +30% raid damage but -50% defense
        Paranoia       // Sees fake attacks
    }
    
    public List<TraumaEvent> traumaEvents = new List<TraumaEvent>();
    
    [System.Serializable]
    public class TraumaEvent
    {
        public string eventName;
        public float mentalHealthImpact;
        public TraumaState triggersTrauma;
        public string description;
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    void Start()
    {
        InitializeTraumaEvents();
        InvokeRepeating("DecayMentalHealth", 3600f, 3600f); // Every hour
    }
    
    void InitializeTraumaEvents()
    {
        traumaEvents.Add(new TraumaEvent { eventName = "Lost Raid", mentalHealthImpact = -15f, triggersTrauma = TraumaState.Depression, description = "Your survivors feel defeated." });
        traumaEvents.Add(new TraumaEvent { eventName = "Base Attacked", mentalHealthImpact = -20f, triggersTrauma = TraumaState.PTSD, description = "Survivors traumatized by the attack." });
        traumaEvents.Add(new TraumaEvent { eventName = "Ally Betrayed", mentalHealthImpact = -30f, triggersTrauma = TraumaState.Paranoia, description = "Trust is broken." });
        traumaEvents.Add(new TraumaEvent { eventName = "Killed Enemy", mentalHealthImpact = -10f, triggersTrauma = TraumaState.Aggression, description = "Violence takes a toll." });
        traumaEvents.Add(new TraumaEvent { eventName = "Found Supplies", mentalHealthImpact = +10f, triggersTrauma = TraumaState.None, description = "Hope restored!" });
        traumaEvents.Add(new TraumaEvent { eventName = "Won Raid", mentalHealthImpact = +5f, triggersTrauma = TraumaState.None, description = "Confidence boost." });
    }
    
    public void TriggerEvent(string eventName)
    {
        TraumaEvent evt = traumaEvents.Find(e => e.eventName == eventName);
        if (evt != null)
        {
            mentalHealth += evt.mentalHealthImpact;
            mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMentalHealth);
            
            DebugLogger.Log($"🧠 {evt.description} Mental Health: {mentalHealth:F0}/100");
            
            // Check for trauma state
            if (mentalHealth < 30 && currentTrauma == TraumaState.None)
            {
                currentTrauma = evt.triggersTrauma;
                ApplyTraumaEffects();
                SendNotification($"⚠️ TRAUMA: {currentTrauma}!", evt.description);
            }
            else if (mentalHealth > 70 && currentTrauma != TraumaState.None)
            {
                HealTrauma();
            }
            
            SaveMentalState();
        }
    }
    
    void ApplyTraumaEffects()
    {
        ResourceManager resources = GetComponent<ResourceManager>();
        
        switch (currentTrauma)
        {
            case TraumaState.Depression:
                DebugLogger.Log("😔 Depression: Resource production reduced by 50%");
                break;
            case TraumaState.PTSD:
                DebugLogger.Log("😨 PTSD: Survivors panic randomly!");
                break;
            case TraumaState.Aggression:
                DebugLogger.Log("😤 Aggression: +30% raid damage, -50% defense!");
                break;
            case TraumaState.Paranoia:
                DebugLogger.Log("👀 Paranoia: Seeing fake attacks!");
                break;
        }
    }
    
    void HealTrauma()
    {
        currentTrauma = TraumaState.None;
        DebugLogger.Log("💚 Trauma healed! Mental health restored.");
        SendNotification("💚 Healing Complete!", "Survivors are recovering.");
    }
    
    public void TherapyMiniGame(int cost)
    {
        CurrencyManager currency = GetComponent<CurrencyManager>();
        if (currency.SpendCoins(cost))
        {
            mentalHealth += 20f;
            mentalHealth = Mathf.Min(mentalHealth, maxMentalHealth);
            DebugLogger.Log($"🎮 Therapy session complete! Mental health improved.");
        }
    }
    
    void DecayMentalHealth()
    {
        if (currentTrauma != TraumaState.None)
        {
            mentalHealth -= 5f;
            DebugLogger.Log($"⚠️ Mental health decaying: {mentalHealth:F0}/100");
        }
    }
    
    void SendNotification(string title, string message)
    {
        NotificationManager notif = GetComponent<NotificationManager>();
        if (notif != null) notif.ShowNotification($"{title}: {message}", "warning");
    }
    
    void SaveMentalState()
    {
        PlayerPrefs.SetFloat("MentalHealth", mentalHealth);
        PlayerPrefs.SetInt("CurrentTrauma", (int)currentTrauma);
    }
}