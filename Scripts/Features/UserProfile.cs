using UnityEngine;
using System.Collections.Generic;

public class UserProfile : MonoBehaviour
{
    public static UserProfile Instance;
    
    [Header("User Info")]
    public string username;
    public int userId;
    public int level = 1;
    public int xp = 0;
    public int totalPlayTime = 0;
    public string joinDate;
    
    [Header("Privacy Settings")]
    public bool showOnLeaderboard = true;
    public bool showOnlineStatus = true;
    public bool showResources = false;
    public bool allowFriendRequests = true;
    public bool allowClanInvites = true;
    
    [Header("Game Stats")]
    public int totalRaidsWon = 0;
    public int totalRaidsLost = 0;
    public int totalResourcesCollected = 0;
    public int totalVexaEarned = 0;
    public int currentStreak = 0;
    public int bestStreak = 0;
    
    [Header("Achievements")]
    public List<string> unlockedAchievements = new List<string>();
    public List<string> unlockedBadges = new List<string>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProfile();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (string.IsNullOrEmpty(username))
        {
            CreateNewProfile();
        }
    }
    
    void CreateNewProfile()
    {
        userId = Random.Range(10000, 99999);
        username = "Player_" + userId;
        joinDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        SaveProfile();
        Debug.Log($"🆕 New profile created: {username} (ID: {userId})");
    }
    
    public void UpdateStats(string statName, int amount)
    {
        switch (statName)
        {
            case "raidWin":
                totalRaidsWon += amount;
                CheckAchievements("raidWin");
                break;
            case "raidLoss":
                totalRaidsLost += amount;
                break;
            case "resources":
                totalResourcesCollected += amount;
                CheckAchievements("resources");
                break;
            case "vexa":
                totalVexaEarned += amount;
                CheckAchievements("vexa");
                break;
        }
        SaveProfile();
    }
    
    public void AddXP(int amount)
    {
        xp += amount;
        if (xp >= level * 100)
        {
            xp -= level * 100;
            level++;
            Debug.Log($"🎉 Level Up! Now Level {level}");
            
            // Check level achievement
            CheckAchievements("level");
        }
        SaveProfile();
    }
    
    void CheckAchievements(string type)
    {
        // Raid achievements
        if (type == "raidWin" && totalRaidsWon >= 10 && !unlockedAchievements.Contains("RAIDER"))
        {
            unlockedAchievements.Add("RAIDER");
            Debug.Log("🏆 Achievement Unlocked: RAIDER (10 raid wins)");
        }
        
        if (type == "raidWin" && totalRaidsWon >= 100 && !unlockedAchievements.Contains("WARLORD"))
        {
            unlockedAchievements.Add("WARLORD");
            Debug.Log("🏆 Achievement Unlocked: WARLORD (100 raid wins)");
        }
        
        // Resource achievements
        if (type == "resources" && totalResourcesCollected >= 1000 && !unlockedAchievements.Contains("RESOURCEFUL"))
        {
            unlockedAchievements.Add("RESOURCEFUL");
            Debug.Log("🏆 Achievement Unlocked: RESOURCEFUL (1000 resources)");
        }
        
        // VEXA achievements
        if (type == "vexa" && totalVexaEarned >= 100 && !unlockedAchievements.Contains("WEALTHY"))
        {
            unlockedAchievements.Add("WEALTHY");
            Debug.Log("🏆 Achievement Unlocked: WEALTHY (100 VEXA earned)");
        }
        
        // Level achievements
        if (type == "level")
        {
            if (level >= 10 && !unlockedAchievements.Contains("VETERAN"))
            {
                unlockedAchievements.Add("VETERAN");
                Debug.Log("🏆 Achievement Unlocked: VETERAN (Level 10)");
            }
            if (level >= 50 && !unlockedAchievements.Contains("LEGEND"))
            {
                unlockedAchievements.Add("LEGEND");
                Debug.Log("🏆 Achievement Unlocked: LEGEND (Level 50)");
            }
        }
        
        SaveProfile();
    }
    
    public void UpdateStreak()
    {
        string lastDate = PlayerPrefs.GetString("LastLogin", "");
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        if (lastDate != today)
        {
            System.DateTime last = System.DateTime.Parse(lastDate);
            System.DateTime now = System.DateTime.Parse(today);
            
            if ((now - last).Days == 1)
            {
                currentStreak++;
                if (currentStreak > bestStreak)
                    bestStreak = currentStreak;
            }
            else if ((now - last).Days > 1)
            {
                currentStreak = 1;
            }
            
            PlayerPrefs.SetString("LastLogin", today);
            SaveProfile();
        }
    }
    
    public string GetPrivacySettings()
    {
        return $"Leaderboard: {(showOnLeaderboard ? "Visible" : "Hidden")}\n" +
               $"Online Status: {(showOnlineStatus ? "Visible" : "Hidden")}\n" +
               $"Resources: {(showResources ? "Visible" : "Hidden")}\n" +
               $"Friend Requests: {(allowFriendRequests ? "Enabled" : "Disabled")}\n" +
               $"Clan Invites: {(allowClanInvites ? "Enabled" : "Disabled")}";
    }
    
    public void TogglePrivacy(string setting)
    {
        switch (setting)
        {
            case "leaderboard":
                showOnLeaderboard = !showOnLeaderboard;
                break;
            case "online":
                showOnlineStatus = !showOnlineStatus;
                break;
            case "resources":
                showResources = !showResources;
                break;
            case "friends":
                allowFriendRequests = !allowFriendRequests;
                break;
            case "clan":
                allowClanInvites = !allowClanInvites;
                break;
        }
        SaveProfile();
        Debug.Log($"Privacy setting changed: {setting} = {GetPrivacySettingValue(setting)}");
    }
    
    string GetPrivacySettingValue(string setting)
    {
        switch (setting)
        {
            case "leaderboard": return showOnLeaderboard.ToString();
            case "online": return showOnlineStatus.ToString();
            case "resources": return showResources.ToString();
            case "friends": return allowFriendRequests.ToString();
            case "clan": return allowClanInvites.ToString();
            default: return "Unknown";
        }
    }
    
    public void ShowProfile()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"👤 USER PROFILE");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"Name: {username} (ID: {userId})");
        Debug.Log($"Level: {level} | XP: {xp}/{level * 100}");
        Debug.Log($"Play Time: {totalPlayTime} minutes");
        Debug.Log($"Joined: {joinDate}");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"📊 STATS");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"Raids: {totalRaidsWon}W / {totalRaidsLost}L");
        Debug.Log($"Resources Collected: {totalResourcesCollected}");
        Debug.Log($"VEXA Earned: {totalVexaEarned}");
        Debug.Log($"Current Streak: {currentStreak} days");
        Debug.Log($"Best Streak: {bestStreak} days");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"🏆 ACHIEVEMENTS");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        if (unlockedAchievements.Count == 0)
            Debug.Log("No achievements yet!");
        else
            foreach (string ach in unlockedAchievements)
                Debug.Log($"⭐ {ach}");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"🔒 PRIVACY SETTINGS");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log(GetPrivacySettings());
    }
    
    void SaveProfile()
    {
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.SetInt("UserID", userId);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("XP", xp);
        PlayerPrefs.SetInt("TotalPlayTime", totalPlayTime);
        PlayerPrefs.SetString("JoinDate", joinDate);
        
        PlayerPrefs.SetInt("TotalRaidsWon", totalRaidsWon);
        PlayerPrefs.SetInt("TotalRaidsLost", totalRaidsLost);
        PlayerPrefs.SetInt("TotalResources", totalResourcesCollected);
        PlayerPrefs.SetInt("TotalVexa", totalVexaEarned);
        PlayerPrefs.SetInt("CurrentStreak", currentStreak);
        PlayerPrefs.SetInt("BestStreak", bestStreak);
        
        PlayerPrefs.SetInt("ShowLeaderboard", showOnLeaderboard ? 1 : 0);
        PlayerPrefs.SetInt("ShowOnline", showOnlineStatus ? 1 : 0);
        PlayerPrefs.SetInt("ShowResources", showResources ? 1 : 0);
        PlayerPrefs.SetInt("AllowFriends", allowFriendRequests ? 1 : 0);
        PlayerPrefs.SetInt("AllowClan", allowClanInvites ? 1 : 0);
        
        // Save achievements as comma separated
        string achievements = string.Join(",", unlockedAchievements.ToArray());
        PlayerPrefs.SetString("Achievements", achievements);
        
        PlayerPrefs.Save();
    }
    
    void LoadProfile()
    {
        username = PlayerPrefs.GetString("Username", "");
        userId = PlayerPrefs.GetInt("UserID", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        xp = PlayerPrefs.GetInt("XP", 0);
        totalPlayTime = PlayerPrefs.GetInt("TotalPlayTime", 0);
        joinDate = PlayerPrefs.GetString("JoinDate", "");
        
        totalRaidsWon = PlayerPrefs.GetInt("TotalRaidsWon", 0);
        totalRaidsLost = PlayerPrefs.GetInt("TotalRaidsLost", 0);
        totalResourcesCollected = PlayerPrefs.GetInt("TotalResources", 0);
        totalVexaEarned = PlayerPrefs.GetInt("TotalVexa", 0);
        currentStreak = PlayerPrefs.GetInt("CurrentStreak", 0);
        bestStreak = PlayerPrefs.GetInt("BestStreak", 0);
        
        showOnLeaderboard = PlayerPrefs.GetInt("ShowLeaderboard", 1) == 1;
        showOnlineStatus = PlayerPrefs.GetInt("ShowOnline", 1) == 1;
        showResources = PlayerPrefs.GetInt("ShowResources", 0) == 1;
        allowFriendRequests = PlayerPrefs.GetInt("AllowFriends", 1) == 1;
        allowClanInvites = PlayerPrefs.GetInt("AllowClan", 1) == 1;
        
        string achievements = PlayerPrefs.GetString("Achievements", "");
        if (!string.IsNullOrEmpty(achievements))
        {
            unlockedAchievements = new List<string>(achievements.Split(','));
        }
    }
}