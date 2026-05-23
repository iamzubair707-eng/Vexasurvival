using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour
{
    public List<Notification> activeNotifications = new List<Notification>();
    public GameObject notificationPrefab;
    public Transform notificationPanel;
    
    void Start()
    {
        StartCoroutine(CheckUrgentEvents());
    }
    
    public void ShowNotification(string message, string type = "info")
    {
        Notification notif = new Notification(message, type);
        activeNotifications.Add(notif);
        
        DebugLogger.Log($"[{type.ToUpper()}] {message}");
        
        // Show on UI if prefab assigned
        if (notificationPrefab != null && notificationPanel != null)
        {
            GameObject notifGO = Instantiate(notificationPrefab, notificationPanel);
            notifGO.GetComponentInChildren<UnityEngine.UI.Text>().text = message;
            Destroy(notifGO, 3f);
        }
        
        StartCoroutine(RemoveNotification(notif));
    }
    
    IEnumerator CheckUrgentEvents()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            
            // Random event for urgency (FOMO trigger)
            int randomEvent = Random.Range(0, 100);
            
            if (randomEvent > 90)
            {
                ShowNotification("⚠️ LIMITED TIME: Double rewards for next 30 minutes!", "urgent");
            }
            else if (randomEvent > 85)
            {
                ShowNotification("⚔️ Your base is visible to enemies! Upgrade defenses!", "warning");
            }
            else if (randomEvent > 80)
            {
                ShowNotification("🎁 Claim your hourly reward before it resets!", "reminder");
            }
        }
    }
    
    IEnumerator RemoveNotification(Notification notif)
    {
        yield return new WaitForSeconds(4f);
        activeNotifications.Remove(notif);
    }
    
    public void ShowRaidNotification(string attackerName, int damage)
    {
        ShowNotification($"🔥 {attackerName} is raiding your base! Damage: {damage}", "danger");
        
        // Red screen flash effect
        StartCoroutine(RedFlash());
    }
    
    IEnumerator RedFlash()
    {
        // Find or create flash panel
        GameObject flash = GameObject.Find("FlashPanel");
        if (flash != null)
        {
            UnityEngine.UI.Image img = flash.GetComponent<UnityEngine.UI.Image>();
            img.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.3f);
            img.color = new Color(1, 0, 0, 0);
        }
    }
}

[System.Serializable]
public class Notification
{
    public string message;
    public string type;
    public float timestamp;
    
    public Notification(string msg, string t)
    {
        message = msg;
        type = t;
        timestamp = Time.time;
    }
}