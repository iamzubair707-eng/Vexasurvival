using UnityEngine;
using System;

public class DynamicWorldEvents : MonoBehaviour
{
    public WorldEvent currentEvent;
    public DateTime nextEventTime;
    
    [System.Serializable]
    public class WorldEvent
    {
        public string eventName;
        public string description;
        public float resourceMultiplier;
        public float raidMultiplier;
        public int durationDays;
        public DateTime endTime;
    }
    
    void Start()
    {
        LoadEventData();
        InvokeRepeating("CheckAndStartEvent", 60f, 3600f);
    }
    
    void CheckAndStartEvent()
    {
        if (currentEvent == null && DateTime.Now >= nextEventTime)
        {
            StartRandomEvent();
        }
        else if (currentEvent != null && DateTime.Now >= currentEvent.endTime)
        {
            EndCurrentEvent();
        }
    }
    
    void StartRandomEvent()
    {
        int random = UnityEngine.Random.Range(0, 5);
        
        switch (random)
        {
            case 0:
                currentEvent = new WorldEvent
                {
                    eventName = "☢️ NUCLEAR STORM",
                    description = "Radiation reduces wood production!",
                    resourceMultiplier = 0.5f,
                    raidMultiplier = 1f,
                    durationDays = 3,
                    endTime = DateTime.Now.AddDays(3)
                };
                break;
            case 1:
                currentEvent = new WorldEvent
                {
                    eventName = "🧟 ZOMBIE HORDE",
                    description = "Raids are more dangerous!",
                    resourceMultiplier = 0.8f,
                    raidMultiplier = 1.5f,
                    durationDays = 2,
                    endTime = DateTime.Now.AddDays(2)
                };
                break;
            case 2:
                currentEvent = new WorldEvent
                {
                    eventName = "🌪️ SANDSTORM",
                    description = "Visibility reduced! Defense down!",
                    resourceMultiplier = 0.6f,
                    raidMultiplier = 1.2f,
                    durationDays = 4,
                    endTime = DateTime.Now.AddDays(4)
                };
                break;
            case 3:
                currentEvent = new WorldEvent
                {
                    eventName = "❄️ NUCLEAR WINTER",
                    description = "Food production halted!",
                    resourceMultiplier = 0.3f,
                    raidMultiplier = 1f,
                    durationDays = 5,
                    endTime = DateTime.Now.AddDays(5)
                };
                break;
            case 4:
                currentEvent = new WorldEvent
                {
                    eventName = "⚡ RADIOACTIVE SURGE",
                    description = "Double resource production!",
                    resourceMultiplier = 2f,
                    raidMultiplier = 1f,
                    durationDays = 2,
                    endTime = DateTime.Now.AddDays(2)
                };
                break;
        }
        
        Debug.Log($"🌍 WORLD EVENT: {currentEvent.eventName}");
        Debug.Log($"📝 {currentEvent.description}");
        Debug.Log($"⏰ Ends: {currentEvent.endTime}");
        
        SendNotification($"🌍 {currentEvent.eventName}!", currentEvent.description);
        
        nextEventTime = DateTime.Now.AddDays(UnityEngine.Random.Range(7, 11));
        SaveEventData();
    }
    
    void EndCurrentEvent()
    {
        Debug.Log($"✅ Event ended: {currentEvent.eventName}");
        SendNotification("✅ Event Ended", "The world returns to normal.");
        currentEvent = null;
        SaveEventData();
    }
    
    void SendNotification(string title, string message)
    {
        NotificationManager notif = GetComponent<NotificationManager>();
        if (notif != null) notif.ShowNotification($"{title} {message}", "urgent");
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
        if (!string.IsNullOrEmpty(savedEvent))
        {
            // Reload current event (simplified)
        }
        string savedNextTime = PlayerPrefs.GetString("NextEventTime", DateTime.Now.ToString());
        nextEventTime = DateTime.Parse(savedNextTime);
    }
}