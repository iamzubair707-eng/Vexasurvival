using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public List<Building> buildings = new List<Building>();
    public GameObject buildingPreview;
    private ResourceManager resourceManager;
    
    void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        LoadBuildings();
    }
    
    public bool TryBuild(string buildingName)
    {
        Building building = buildings.Find(b => b.name == buildingName);
        
        if (building == null)
        {
            Debug.Log("Building not found!");
            return false;
        }
        
        // Check resources
        if (resourceManager.SpendResource("wood", building.woodCost) &&
            resourceManager.SpendResource("stone", building.stoneCost))
        {
            building.isBuilt = true;
            building.level = 1;
            SaveBuildings();
            Debug.Log($"{buildingName} built!");
            
            // Apply building effect
            ApplyBuildingEffect(building);
            return true;
        }
        
        Debug.Log($"Not enough resources for {buildingName}!");
        return false;
    }
    
    public bool UpgradeBuilding(string buildingName)
    {
        Building building = buildings.Find(b => b.name == buildingName);
        
        if (building == null || !building.isBuilt)
            return false;
        
        int upgradeWood = building.woodCost * building.level;
        int upgradeStone = building.stoneCost * building.level;
        
        if (resourceManager.SpendResource("wood", upgradeWood) &&
            resourceManager.SpendResource("stone", upgradeStone))
        {
            building.level++;
            SaveBuildings();
            Debug.Log($"{buildingName} upgraded to level {building.level}!");
            ApplyBuildingEffect(building);
            return true;
        }
        
        return false;
    }
    
    void ApplyBuildingEffect(Building building)
    {
        switch (building.name)
        {
            case "Farm":
                Debug.Log("Food production +" + (5 * building.level));
                break;
            case "Mine":
                Debug.Log("Stone production +" + (3 * building.level));
                break;
            case "LumberMill":
                Debug.Log("Wood production +" + (5 * building.level));
                break;
            case "Turret":
                Debug.Log("Defense +" + (10 * building.level));
                break;
        }
    }
    
    void SaveBuildings()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            PlayerPrefs.SetInt($"Building_{buildings[i].name}_Built", buildings[i].isBuilt ? 1 : 0);
            PlayerPrefs.SetInt($"Building_{buildings[i].name}_Level", buildings[i].level);
        }
    }
    
    void LoadBuildings()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].isBuilt = PlayerPrefs.GetInt($"Building_{buildings[i].name}_Built", 0) == 1;
            buildings[i].level = PlayerPrefs.GetInt($"Building_{buildings[i].name}_Level", 0);
        }
    }
}

[System.Serializable]
public class Building
{
    public string name;
    public int woodCost;
    public int stoneCost;
    public int level;
    public bool isBuilt;
}