using UnityEngine;
using System.Collections.Generic;

public class Leaderboard : MonoBehaviour
{
    public List<PlayerScore> topPlayers = new List<PlayerScore>();
    
    void Start()
    {
        LoadScores();
    }
    
    public void AddScore(string playerName, int vexaAmount)
    {
        PlayerScore existing = topPlayers.Find(p => p.playerName == playerName);
        
        if (existing != null)
        {
            existing.vexaAmount += vexaAmount;
        }
        else
        {
            topPlayers.Add(new PlayerScore(playerName, vexaAmount));
        }
        
        SortLeaderboard();
        SaveScores();
        DisplayLeaderboard();
    }
    
    void SortLeaderboard()
    {
        topPlayers.Sort((a, b) => b.vexaAmount.CompareTo(a.vexaAmount));
        
        // Keep only top 100
        if (topPlayers.Count > 100)
            topPlayers.RemoveRange(100, topPlayers.Count - 100);
    }
    
    void DisplayLeaderboard()
    {
        Debug.Log("========== LEADERBOARD ==========");
        for (int i = 0; i < Mathf.Min(10, topPlayers.Count); i++)
        {
            Debug.Log($"{i+1}. {topPlayers[i].playerName} - {topPlayers[i].vexaAmount} VEXA");
        }
        Debug.Log("=================================");
    }
    
    void SaveScores()
    {
        for (int i = 0; i < topPlayers.Count; i++)
        {
            PlayerPrefs.SetString($"PlayerName_{i}", topPlayers[i].playerName);
            PlayerPrefs.SetInt($"PlayerVexa_{i}", topPlayers[i].vexaAmount);
        }
        PlayerPrefs.SetInt("PlayerCount", topPlayers.Count);
    }
    
    void LoadScores()
    {
        topPlayers.Clear();
        int count = PlayerPrefs.GetInt("PlayerCount", 0);
        
        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"PlayerName_{i}", "");
            int vexa = PlayerPrefs.GetInt($"PlayerVexa_{i}", 0);
            
            if (!string.IsNullOrEmpty(name))
            {
                topPlayers.Add(new PlayerScore(name, vexa));
            }
        }
        
        SortLeaderboard();
    }
}

[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public int vexaAmount;
    
    public PlayerScore(string name, int vexa)
    {
        playerName = name;
        vexaAmount = vexa;
    }
}