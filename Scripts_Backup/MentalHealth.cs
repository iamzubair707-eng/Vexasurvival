using UnityEngine;
using System;

public class MentalHealth : MonoBehaviour
{
    public float mentalHealth = 100f;
    public float maxMentalHealth = 100f;
    public MentalState currentState = MentalState.Stable;
    
    public enum MentalState
    {
        Stable,      // Normal production
        Stressed,    // -20% production
        Depressed,   // -50% production, no raids
        Rebellious,  // Might leave or attack
        Insane       // Random actions
    }
    
    public event Action<MentalState> OnMentalStateChanged;
    
    void Start()
    {
        LoadMentalHealth();
        InvokeRepeating("DecayMentalHealth", 3600f, 3600f); // Hourly
    }
    
    public void AddTrauma(string eventName, int amount)
    {
        mentalHealth -= amount;
        mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMentalHealth);
        
        Debug.Log($"🧠 {eventName}: Mental Health -{amount} (Now: {mentalHealth:F0})");
        UpdateMentalState();
        SaveMentalHealth();
    }
    
    public void HealMentalHealth(int amount)
    {
        mentalHealth += amount;
        mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMentalHealth);
        UpdateMentalState();
        SaveMentalHealth();
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
            
            NotificationManager notif = GetComponent<NotificationManager>();
            if (notif != null)
                notif.ShowNotification($"⚠️ Survivors are {currentState}!", "Build Counseling Center to help!", "warning");
        }
    }
    
    void ApplyStateEffects()
    {
        switch (currentState)
        {
            case MentalState.Stressed:
                Debug.Log("😟 Survivors stressed: -20% production");
                break;
            case MentalState.Depressed:
                Debug.Log("😔 Survivors depressed: -50% production, cannot raid");
                break;
            case MentalState.Rebellious:
                Debug.Log("⚠️ Survivors rebellious! Might leave!");
                break;
            case MentalState.Insane:
                Debug.Log("💀 Survivors insane! Random actions!");
                break;
        }
    }
    
    void DecayMentalHealth()
    {
        if (currentState != MentalState.Stable)
        {
            mentalHealth -= 5f;
            mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMentalHealth);
            UpdateMentalState();
            SaveMentalHealth();
        }
    }
    
    public void SendToCounseling()
    {
        CoreResources resources = GetComponent<CoreResources>();
        if (resources.SpendResource("scrap", 50))
        {
            HealMentalHealth(30);
            Debug.Log("💚 Counseling session completed! Mental health improved!");
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
}