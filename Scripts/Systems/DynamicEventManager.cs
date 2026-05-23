using UnityEngine;
using System;

public class DynamicEventManager : MonoBehaviour
{
    public static DynamicEventManager Instance;
    
    public ActiveEvent currentEvent;
    public DateTime nextEventTime;
    public float globalEventMultiplier = 1f;
    
    [System.Serializable]
    public class ActiveEvent
    {
        public string eventName;
        public string description;
        public float resourceMultiplier;
        public float raidDifficulty;
        public float durationHours;
        public DateTime endTime;
        public string visualEffect;
    }
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        LoadEventData();
        InvokeRepeating("CheckAndTriggerEvent", 60f, 300f);
    }
    
    void CheckAndTriggerEvent()
    {
        if (currentEvent == null && DateTime.Now >= nextEventTime)
        {
            TriggerRandomEvent();
        }
        else if (currentEvent != null && DateTime.Now >= currentEvent.endTime)
        {
            EndCurrentEvent();
        }
    }
    
    void TriggerRandomEvent()
    {
        int random = UnityEngine.Random.Range(0, 100);
        ActiveEvent newEvent = null;
        
        if (random < 20)
        {
            newEvent = new ActiveEvent
            {
                eventName = "☢️ RADIATION STORM",
                description = "Resources decay faster!",
                resourceMultiplier = 0.5f,
                raidDifficulty = 1.2f,
                durationHours = 4,
                endTime = DateTime.Now.AddHours(4),
                visualEffect = "RedFog"
            };
        }
        else if (random < 40)
        {
            newEvent = new ActiveEvent
            {
                eventName = "🧟 ZOMBIE HORDE",
                description = "Base defense required!",
                resourceMultiplier = 0.8f,
                raidDifficulty = 1.5f,
                durationHours = 3,
                endTime = DateTime.Now.AddHours(3),
                visualEffect = "GreenFog"
            };
        }
        else if (random < 60)
        {
            newEvent = new ActiveEvent
            {
                eventName = "💰 TRADER CARAVAN",
                description = "Discounts on all items!",
                resourceMultiplier = 1.5f,
                raidDifficulty = 0.5f,
                durationHours = 6,
                endTime = DateTime.Now.AddHours(6),
                visualEffect = "GoldenGlow"
            };
        }
        else if (random < 80)
        {
            newEvent = new ActiveEvent
            {
                eventName = "🌪️ SANDSTORM",
                description = "Visibility reduced!",
                resourceMultiplier = 0.6f,
                raidDifficulty = 1.3f,
                durationHours = 5,
                endTime = DateTime.Now.AddHours(5),
                visualEffect = "BrownFog"
            };
        }
        else
        {
            newEvent = new ActiveEvent
            {
                eventName = "⚡ DOUBLE RESOURCES",
                description = "All gathering doubled!",
                resourceMultiplier = 2f,
                raidDifficulty = 1f,
                durationHours = 2,
                endTime = DateTime.Now.AddHours(2),
                visualEffect = "YellowGlow"
            };
        }
        
        currentEvent = newEvent;
        globalEventMultiplier = currentEvent.resourceMultiplier;
        
        DebugLogger.Log($"🌍 {currentEvent.eventName} started!");
        DebugLogger.Log($"📝 {currentEvent.description}");
        
        NotificationManager notif = GetComponent<NotificationManager>();
        notif?.ShowNotification($"🌍 {currentEvent.eventName}!", currentEvent.description, "urgent");
        
        nextEventTime = DateTime.Now.AddDays(UnityEngine.Random.Range(5, 10));
        SaveEventData();
    }
    
    void EndCurrentEvent()
    {
        DebugLogger.Log($"✅ Event ended: {currentEvent.eventName}");
        globalEventMultiplier = 1f;
        currentEvent = null;
        SaveEventData();
    }
    
    void SaveEventData()
    {
        if (currentEvent != null)
        {
            PlayerPrefs.SetString("CurrentEvent", currentEvent.eventName);
            PlayerPrefs.SetString("EventEndTime", currentEvent.endTime.ToString());
        }
        PlayerPrefs.SetString("NextEventTime", nextEventTime.ToString());
    }
    
    void LoadEventData()
    {
        string savedEvent = PlayerPrefs.GetString("CurrentEvent", "");
        string savedNextTime = PlayerPrefs.GetString("NextEventTime", DateTime.Now.ToString());
        nextEventTime = DateTime.Parse(savedNextTime);
        
        if (!string.IsNullOrEmpty(savedEvent) && currentEvent == null)
        {
            // Reload event if needed
        }
    }
}