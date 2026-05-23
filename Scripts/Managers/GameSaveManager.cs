using UnityEngine;
using System.Collections.Generic;

public class GameSaveManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public BuildingManager buildingManager;
    public HealthSystem healthSystem;
    public ClanSystem clanSystem;
    public Leaderboard leaderboard;
    
    void Start()
    {
        LoadAllData();
    }
    
    void OnApplicationQuit()
    {
        SaveAllData();
    }
    
    public void SaveAllData()
    {
        PlayerPrefs.SetInt("LastSaveTime", (int)System.DateTime.Now.Ticks);
        PlayerPrefs.Save();
        Debug.Log("💾 Game Saved!");
    }
    
    public void LoadAllData()
    {
        Debug.Log("📀 Loading Game Data...");
        // All individual systems load their own data via PlayerPrefs
        // This just ensures everything is ready
    }
    
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        
        // Reset all systems
        if (resourceManager != null)
        {
            resourceManager.wood = 50;
            resourceManager.stone = 30;
            resourceManager.food = 40;
            resourceManager.vexaTokens = 5;
        }
        
        if (healthSystem != null)
        {
            healthSystem.Heal(100);
        }
        
        Debug.Log("🆕 New Game Started!");
        SaveAllData();
    }
    
    public void ExportSaveData()
    {
        string saveData = System.Json.JsonConvert.SerializeObject(new
        {
            wood = resourceManager?.wood ?? 0,
            stone = resourceManager?.stone ?? 0,
            food = resourceManager?.food ?? 0,
            vexa = resourceManager?.vexaTokens ?? 0,
            health = healthSystem != null ? healthSystem.maxHealth : 100
        });
        
        Debug.Log("📤 Save Data: " + saveData);
        PlayerPrefs.SetString("ExportData", saveData);
    }
}