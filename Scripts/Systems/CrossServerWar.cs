using UnityEngine;
using System.Collections.Generic;

public class CrossServerWar : MonoBehaviour
{
    public bool isWarActive = false;
    public string enemyServer;
    public DateTime warEndTime;
    public int warPoints = 0;
    
    public void DeclareWarOnServer(string serverName)
    {
        if (!isWarActive)
        {
            isWarActive = true;
            enemyServer = serverName;
            warEndTime = DateTime.Now.AddDays(3);
            warPoints = 0;
            
            DebugLogger.Log($"⚔️ CROSS-SERVER WAR DECLARED on {serverName}!");
            DebugLogger.Log($"⏰ War ends in 3 days");
            
            SendNotification("⚔️ CROSS-SERVER WAR!", $"Attack {serverName} for glory!");
        }
    }
    
    public void ContributeWarPoints(int points)
    {
        if (isWarActive)
        {
            warPoints += points;
            DebugLogger.Log($" War contribution: {warPoints} points");
            
            if (warPoints >= 1000)
            {
                EndWar(true);
            }
        }
    }
    
    void EndWar(bool victory)
    {
        if (victory)
        {
            DebugLogger.Log(" CROSS-SERVER WAR VICTORY!");
            // Give all players rewards
            CurrencyManager currency = GetComponent<CurrencyManager>();
            currency.AddGems(500);
        }
        else
        {
            DebugLogger.Log("💀 Cross-server war lost...");
        }
        
        isWarActive = false;
        SendNotification("War Ended!", victory ? "Victory! Rewards claimed!" : "Defeat... Train harder!");
    }
    
    void SendNotification(string title, string message)
    {
        NotificationManager notif = GetComponent<NotificationManager>();
        if (notif != null) notif.ShowNotification($"{title} {message}", "urgent");
    }
}