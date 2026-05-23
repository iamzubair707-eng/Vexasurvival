using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    private string savePath;
    public GameData currentData = new GameData();
    
    [System.Serializable]
    public class GameData
    {
        public DateTime lastSaveTime;
        public float playTime;
        
        // Resources
        public int wood, stone, food, fuel;
        public int coins, gems;
        
        // Progression
        public int playerLevel, currentXP;
        public int buildingLevel;
        public int troopCount;
        
        // Stats
        public int totalRaidsWon, totalRaidsLost;
        public int chestsOpened;
        public int dailyStreak;
        public string lastLoginDate;
        
        // Settings
        public bool tutorialComplete;
        public float soundVolume = 1f;
        public float musicVolume = 0.7f;
    }
    
    void Awake()
    {
        savePath = Application.persistentDataPath + "/vexasurvival_save.json";
        LoadGame();
    }
    
    public void SaveGame()
    {
        try
        {
            // Update data from systems
            UpdateDataFromSystems();
            currentData.lastSaveTime = DateTime.Now;
            
            string json = JsonUtility.ToJson(currentData, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"💾 Game saved at {currentData.lastSaveTime}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }
    
    public void LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                currentData = JsonUtility.FromJson<GameData>(json);
                ApplyDataToSystems();
                Debug.Log($"💾 Game loaded from {currentData.lastSaveTime}");
            }
            else
            {
                Debug.Log("No save file found. Starting new game.");
                NewGame();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            NewGame();
        }
    }
    
    public void NewGame()
    {
        currentData = new GameData();
        currentData.wood = 100;
        currentData.stone = 50;
        currentData.food = 80;
        currentData.coins = 200;
        currentData.gems = 10;
        currentData.playerLevel = 1;
        currentData.buildingLevel = 1;
        currentData.troopCount = 0;
        currentData.tutorialComplete = false;
        
        ApplyDataToSystems();
        SaveGame();
        Debug.Log("🆕 New game started!");
    }
    
    void UpdateDataFromSystems()
    {
        var resources = FindObjectOfType<ResourceManager>();
        if (resources != null)
        {
            currentData.wood = resources.GetResource("wood");
            currentData.stone = resources.GetResource("stone");
        }
        
        var currency = FindObjectOfType<CurrencyManager>();
        if (currency != null)
        {
            currentData.coins = currency.coins;
            currentData.gems = currency.gems;
        }
        
        var building = FindObjectOfType<BuildingSystem>();
        if (building != null)
            currentData.buildingLevel = building.GetCurrentLevel();
    }
    
    void ApplyDataToSystems()
    {
        var resources = FindObjectOfType<ResourceManager>();
        if (resources != null)
        {
            resources.SetResource("wood", currentData.wood);
            resources.SetResource("stone", currentData.stone);
        }
        
        var currency = FindObjectOfType<CurrencyManager>();
        if (currency != null)
        {
            currency.coins = currentData.coins;
            currency.gems = currentData.gems;
        }
    }
    
    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
        NewGame();
    }
}