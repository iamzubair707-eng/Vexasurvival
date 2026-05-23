using UnityEngine;
using System.Collections.Generic;

public class CoreResources : MonoBehaviour
{
    // Core resources
    public int food = 100;
    public int water = 100;
    public int scrap = 50;
    public int fuel = 30;
    
    // Resource limits
    public int maxFood = 500;
    public int maxWater = 500;
    public int maxScrap = 300;
    public int maxFuel = 200;
    
    // UI elements (assign in Unity)
    public UnityEngine.UI.Text foodText;
    public UnityEngine.UI.Text waterText;
    public UnityEngine.UI.Text scrapText;
    public UnityEngine.UI.Text fuelText;
    
    void Start()
    {
        LoadResources();
        UpdateUI();
        
        // Auto-generation every 30 seconds
        InvokeRepeating("GeneratePassiveResources", 30f, 30f);
    }
    
    public bool SpendResource(string type, int amount)
    {
        switch (type.ToLower())
        {
            case "food":
                if (food >= amount) { food -= amount; UpdateUI(); SaveResources(); return true; }
                break;
            case "water":
                if (water >= amount) { water -= amount; UpdateUI(); SaveResources(); return true; }
                break;
            case "scrap":
                if (scrap >= amount) { scrap -= amount; UpdateUI(); SaveResources(); return true; }
                break;
            case "fuel":
                if (fuel >= amount) { fuel -= amount; UpdateUI(); SaveResources(); return true; }
                break;
        }
        DebugLogger.Log($"❌ Not enough {type}!");
        return false;
    }
    
    public void AddResource(string type, int amount)
    {
        switch (type.ToLower())
        {
            case "food":
                food = Mathf.Min(food + amount, maxFood);
                break;
            case "water":
                water = Mathf.Min(water + amount, maxWater);
                break;
            case "scrap":
                scrap = Mathf.Min(scrap + amount, maxScrap);
                break;
            case "fuel":
                fuel = Mathf.Min(fuel + amount, maxFuel);
                break;
        }
        UpdateUI();
        SaveResources();
        DebugLogger.Log($" +{amount} {type}!");
    }
    
    void GeneratePassiveResources()
    {
        // Based on buildings and survivors
        int foodGain = 5 + (PlayerPrefs.GetInt("FarmLevel", 0) * 2);
        int waterGain = 3 + (PlayerPrefs.GetInt("WellLevel", 0) * 1);
        
        food = Mathf.Min(food + foodGain, maxFood);
        water = Mathf.Min(water + waterGain, maxWater);
        
        UpdateUI();
        SaveResources();
    }
    
    void UpdateUI()
    {
        if (foodText != null) foodText.text = $"🍔 {food}/{maxFood}";
        if (waterText != null) waterText.text = $"💧 {water}/{maxWater}";
        if (scrapText != null) scrapText.text = $" {scrap}/{maxScrap}";
        if (fuelText != null) fuelText.text = $"⛽ {fuel}/{maxFuel}";
    }
    
    void SaveResources()
    {
        PlayerPrefs.SetInt("Food", food);
        PlayerPrefs.SetInt("Water", water);
        PlayerPrefs.SetInt("Scrap", scrap);
        PlayerPrefs.SetInt("Fuel", fuel);
    }
    
    void LoadResources()
    {
        food = PlayerPrefs.GetInt("Food", 100);
        water = PlayerPrefs.GetInt("Water", 100);
        scrap = PlayerPrefs.GetInt("Scrap", 50);
        fuel = PlayerPrefs.GetInt("Fuel", 30);
    }
}