using UnityEngine;
using System.Collections.Generic;

public class RevengeSystem : MonoBehaviour
{
    public List<string> revengeList = new List<string>(); // Players who attacked you
    public List<string> attackedByList = new List<string>();
    
    public void AddRevengeTarget(string attackerName)
    {
        if (!revengeList.Contains(attackerName))
        {
            revengeList.Add(attackerName);
            DebugLogger.Log($"⚔️ {attackerName} added to revenge list!");
            
            NotificationManager notif = GetComponent<NotificationManager>();
            if (notif != null)
                notif.ShowNotification($"⚔️ {attackerName} attacked you! Revenge?", "danger");
        }
    }
    
    public void RevengeAttack(string targetName)
    {
        if (revengeList.Contains(targetName))
        {
            DebugLogger.Log($"💀 Taking revenge on {targetName}!");
            // Trigger raid system
            RaidSystem raid = GetComponent<RaidSystem>();
            if (raid != null)
                raid.StartRaid(targetName);
            
            revengeList.Remove(targetName);
        }
    }
    
    public void ShowRevengeList()
    {
        DebugLogger.Log("");
        DebugLogger.Log("⚔️ REVENGE LIST");
        DebugLogger.Log("");
        foreach (string name in revengeList)
            DebugLogger.Log($"🔪 {name} - Attack to take revenge!");
        
        if (revengeList.Count == 0)
            DebugLogger.Log("No enemies yet. Stay safe!");
    }
}