using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CompleteSaveSystem : MonoBehaviour
{
    public static CompleteSaveSystem Instance;
    
    [System.Serializable]
    public class GameSaveData
    {
        public DateTime saveTime;
        
        // Core resources
        public int food, water, scrap, fuel;
        
        // Mental health
        public float mentalHealth;
        
        // Raid data
        public int raidEnergy;
        
        // Buildings
        public int farmLevel, wellLevel, counselingLevel;
        
        // Currency
        public int coins, gems;
        
        // User stats
        public string username;
        public int level, xp;
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();
        
        // Collect all data
        CoreResources resources = GetComponent<CoreResources>();
        if (resources != null)
        {
            data.food = resources.food;
            data.water = resources.water;
            data.scrap = resources.scrap;
            data.fuel = resources.fuel;
        }
        
        MentalHealth mental = GetComponent<MentalHealth>();
        if (mental != null) data.mentalHealth = mental.mentalHealth;
        
        PVERaidSystem raid = GetComponent<PVERaidSystem>();
        if (raid != null) data.raidEnergy = raid.raidEnergy;
        
        CurrencyManager currency = GetComponent<CurrencyManager>();
        if (currency != null)
        {
            data.coins = currency.coins;
            data.gems = currency.gems;
        }
        
        data.saveTime = DateTime.Now;
        
        // Serialize to JSON
        string json = JsonUtility.ToJson(data);
        string path = Application.persistentDataPath + "/gamesave.json";
        File.WriteAllText(path, json);
        
        Debug.Log($"💾 Game saved at {data.saveTime}");
    }
    
    public bool LoadGame()
    {
        string path = Application.persistentDataPath + "/gamesave.json";
        
        if (!File.Exists(path))
        {
            Debug.Log("No save file found. Starting new game.");
            return false;
        }
        
        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        
        if (data == null) return false;
        
        // Restore all data
        CoreResources resources = GetComponent<CoreResources>();
        if (resources != null)
        {
            resources.food = data.food;
            resources.water = data.water;
            resources.scrap = data.scrap;
            resources.fuel = data.fuel;
            resources.UpdateUI();
        }
        
        MentalHealth mental = GetComponent<MentalHealth>();
        if (mental != null)
        {
            mental.mentalHealth = data.mentalHealth;
            mental.UpdateMentalState();
        }
        
        PVERaidSystem raid = GetComponent<PVERaidSystem>();
        if (raid != null) raid.raidEnergy = data.raidEnergy;
        
        CurrencyManager currency = GetComponent<CurrencyManager>();
        if (currency != null)
        {
            currency.coins = data.coins;
            currency.gems = data.gems;
        }
        
        Debug.Log($"💾 Game loaded from {data.saveTime}");
        return true;
    }
    
    public void DeleteSave()
    {
        string path = Application.persistentDataPath + "/gamesave.json";
        if (File.Exists(path))
            File.Delete(path);
        Debug.Log("🗑️ Save file deleted!");
    }
}