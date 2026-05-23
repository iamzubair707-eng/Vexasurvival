using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();
    private ResourceManager resourceManager;
    private RaidSystem raidSystem;
    
    void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        raidSystem = GetComponent<RaidSystem>();
        LoadQuests();
        CheckQuestProgress();
    }
    
    void LoadQuests()
    {
        // Daily Quests
        quests.Add(new Quest("collect_wood", "Collect 100 Wood", 100, "wood", 5));
        quests.Add(new Quest("collect_stone", "Collect 50 Stone", 50, "stone", 5));
        quests.Add(new Quest("win_raid", "Win 3 Raids", 3, "raid", 10));
        quests.Add(new Quest("build_building", "Build 2 Buildings", 2, "build", 8));
        quests.Add(new Quest("upgrade_building", "Upgrade Building to Level 3", 3, "upgrade", 15));
        
        // Weekly Quests
        quests.Add(new Quest("weekly_wood", "Collect 1000 Wood (Weekly)", 1000, "wood", 30));
        quests.Add(new Quest("weekly_vexas", "Earn 50 VEXA", 50, "vexa", 50));
        quests.Add(new Quest("clan_war", "Win a Clan War", 1, "clanwar", 40));
        
        // Load completion status
        foreach (Quest q in quests)
        {
            q.isCompleted = PlayerPrefs.GetInt($"Quest_{q.id}", 0) == 1;
            q.progress = PlayerPrefs.GetInt($"Quest_{q.id}_progress", 0);
        }
    }
    
    public void UpdateProgress(string type, int amount)
    {
        foreach (Quest q in quests)
        {
            if (q.isCompleted) continue;
            
            if (q.requiredType == type)
            {
                q.progress += amount;
                DebugLogger.Log($"📋 Quest '{q.title}': {q.progress}/{q.requiredAmount}");
                
                if (q.progress >= q.requiredAmount)
                {
                    CompleteQuest(q);
                }
                
                SaveQuestProgress(q);
            }
        }
    }
    
    void CompleteQuest(Quest q)
    {
        q.isCompleted = true;
        q.progress = q.requiredAmount;
        
        // Give reward
        if (resourceManager != null)
        {
            resourceManager.AddResource("vexa", q.rewardVexa);
        }
        
        DebugLogger.Log($" QUEST COMPLETE: {q.title}! +{q.rewardVexa} VEXA");
        
        // Show notification
        NotificationManager notif = GetComponent<NotificationManager>();
        if (notif != null)
        {
            notif.ShowNotification($" Quest Complete! {q.title} - +{q.rewardVexa} VEXA", "success");
        }
        
        SaveQuestProgress(q);
    }
    
    void CheckQuestProgress()
    {
        // Called when resources change
        if (resourceManager != null)
        {
            UpdateProgress("wood", 0); // Will check actual vs required
            UpdateProgress("stone", 0);
            UpdateProgress("vexa", resourceManager.vexaTokens);
        }
    }
    
    void SaveQuestProgress(Quest q)
    {
        PlayerPrefs.SetInt($"Quest_{q.id}", q.isCompleted ? 1 : 0);
        PlayerPrefs.SetInt($"Quest_{q.id}_progress", q.progress);
    }
    
    public void ShowQuests()
    {
        DebugLogger.Log("========== DAILY QUESTS ==========");
        foreach (Quest q in quests)
        {
            string status = q.isCompleted ? "" : $"📊 {q.progress}/{q.requiredAmount}";
            DebugLogger.Log($"{status} {q.title} → {q.rewardVexa} VEXA");
        }
        DebugLogger.Log("==================================");
    }
    
    void OnEnable()
    {
        // Reset daily quests at midnight (simplified - check on load)
        string lastDate = PlayerPrefs.GetString("QuestLastDate", "");
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        if (lastDate != today)
        {
            foreach (Quest q in quests)
            {
                if (!q.id.Contains("weekly"))
                {
                    q.isCompleted = false;
                    q.progress = 0;
                    SaveQuestProgress(q);
                }
            }
            PlayerPrefs.SetString("QuestLastDate", today);
            DebugLogger.Log("🔄 Daily quests reset!");
        }
    }
}

[System.Serializable]
public class Quest
{
    public string id;
    public string title;
    public int requiredAmount;
    public string requiredType; // wood, stone, raid, build, vexa, clanwar
    public int rewardVexa;
    public int progress;
    public bool isCompleted;
    
    public Quest(string id, string title, int required, string type, int reward)
    {
        this.id = id;
        this.title = title;
        this.requiredAmount = required;
        this.requiredType = type;
        this.rewardVexa = reward;
        this.progress = 0;
        this.isCompleted = false;
    }
}