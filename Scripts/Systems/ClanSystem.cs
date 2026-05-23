using UnityEngine;
using System.Collections.Generic;

public class ClanSystem : MonoBehaviour
{
    public Clan currentClan;
    public List<Clan> allClans = new List<Clan>();
    
    void Start()
    {
        LoadClans();
    }
    
    public bool CreateClan(string clanName, string leaderName)
    {
        // Check if clan already exists
        if (allClans.Exists(c => c.clanName == clanName))
        {
            Debug.Log("Clan name already exists!");
            return false;
        }
        
        Clan newClan = new Clan();
        newClan.clanName = clanName;
        newClan.leader = leaderName;
        newClan.members.Add(leaderName);
        newClan.totalVexa = 0;
        newClan.clanLevel = 1;
        
        allClans.Add(newClan);
        currentClan = newClan;
        
        SaveClans();
        Debug.Log($"🏰 Clan '{clanName}' created by {leaderName}!");
        return true;
    }
    
    public bool JoinClan(string clanName, string playerName)
    {
        Clan targetClan = allClans.Find(c => c.clanName == clanName);
        
        if (targetClan == null)
        {
            Debug.Log("Clan not found!");
            return false;
        }
        
        if (targetClan.members.Count >= 50)
        {
            Debug.Log("Clan is full!");
            return false;
        }
        
        targetClan.members.Add(playerName);
        currentClan = targetClan;
        
        SaveClans();
        Debug.Log($"✅ {playerName} joined clan '{clanName}'!");
        return true;
    }
    
    public void AddClanVexa(int amount)
    {
        if (currentClan != null)
        {
            currentClan.totalVexa += amount;
            SaveClans();
        }
    }
    
    public void StartClanWar(string enemyClanName)
    {
        Clan enemyClan = allClans.Find(c => c.clanName == enemyClanName);
        
        if (enemyClan == null || currentClan == null)
        {
            Debug.Log("Cannot start clan war!");
            return;
        }
        
        Debug.Log($"⚔️ CLAN WAR: {currentClan.clanName} vs {enemyClan.clanName} ⚔️");
        
        // Calculate war result based on total VEXA
        if (currentClan.totalVexa > enemyClan.totalVexa)
        {
            int reward = currentClan.totalVexa / 10;
            Debug.Log($"🏆 {currentClan.clanName} WINS! Reward: {reward} VEXA each!");
        }
        else
        {
            Debug.Log($"💀 {currentClan.clanName} LOST! Train more!");
        }
    }
    
    public void ShowClanLeaderboard()
    {
        allClans.Sort((a, b) => b.totalVexa.CompareTo(a.totalVexa));
        
        Debug.Log("========== TOP CLANS ==========");
        for (int i = 0; i < Mathf.Min(10, allClans.Count); i++)
        {
            Debug.Log($"{i+1}. {allClans[i].clanName} - {allClans[i].totalVexa} VEXA (Lvl {allClans[i].clanLevel})");
        }
        Debug.Log("===============================");
    }
    
    void SaveClans()
    {
        for (int i = 0; i < allClans.Count; i++)
        {
            PlayerPrefs.SetString($"Clan_{i}_Name", allClans[i].clanName);
            PlayerPrefs.SetString($"Clan_{i}_Leader", allClans[i].leader);
            PlayerPrefs.SetInt($"Clan_{i}_TotalVexa", allClans[i].totalVexa);
            PlayerPrefs.SetInt($"Clan_{i}_Level", allClans[i].clanLevel);
            
            // Save members as comma separated string
            string members = string.Join(",", allClans[i].members.ToArray());
            PlayerPrefs.SetString($"Clan_{i}_Members", members);
        }
        PlayerPrefs.SetInt("TotalClans", allClans.Count);
    }
    
    void LoadClans()
    {
        allClans.Clear();
        int total = PlayerPrefs.GetInt("TotalClans", 0);
        
        for (int i = 0; i < total; i++)
        {
            Clan clan = new Clan();
            clan.clanName = PlayerPrefs.GetString($"Clan_{i}_Name", "");
            clan.leader = PlayerPrefs.GetString($"Clan_{i}_Leader", "");
            clan.totalVexa = PlayerPrefs.GetInt($"Clan_{i}_TotalVexa", 0);
            clan.clanLevel = PlayerPrefs.GetInt($"Clan_{i}_Level", 1);
            
            string membersStr = PlayerPrefs.GetString($"Clan_{i}_Members", "");
            if (!string.IsNullOrEmpty(membersStr))
            {
                clan.members = new List<string>(membersStr.Split(','));
            }
            
            allClans.Add(clan);
        }
    }
}

[System.Serializable]
public class Clan
{
    public string clanName;
    public string leader;
    public List<string> members = new List<string>();
    public int totalVexa;
    public int clanLevel;
}