using UnityEngine;
using System.Collections.Generic;

public class VehicleSystem : MonoBehaviour
{
    public static VehicleSystem Instance;
    
    [System.Serializable]
    public class Vehicle
    {
        public string vehicleName;
        public int fuel;
        public int maxFuel;
        public int durability;
        public int attackBonus;
        public int defenseBonus;
        public float speed;
        public bool isOwned;
        public bool isEquipped;
    }
    
    public List<Vehicle> availableVehicles = new List<Vehicle>();
    public Vehicle activeVehicle;
    public int totalFuel = 100;
    public int maxFuel = 500;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeVehicles();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeVehicles()
    {
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Motorcycle", 
            fuel = 100, maxFuel = 100, durability = 100, 
            attackBonus = 10, defenseBonus = 5, speed = 1.5f, 
            isOwned = true, isEquipped = true 
        });
        
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Pickup Truck", 
            fuel = 150, maxFuel = 150, durability = 150, 
            attackBonus = 20, defenseBonus = 15, speed = 1.2f, 
            isOwned = false, isEquipped = false 
        });
        
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Armored Truck", 
            fuel = 200, maxFuel = 200, durability = 250, 
            attackBonus = 35, defenseBonus = 30, speed = 1.0f, 
            isOwned = false, isEquipped = false 
        });
        
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Battle Tank", 
            fuel = 300, maxFuel = 300, durability = 500, 
            attackBonus = 60, defenseBonus = 50, speed = 0.7f, 
            isOwned = false, isEquipped = false 
        });
        
        LoadVehicleData();
    }
    
    public bool PurchaseVehicle(string vehicleName)
    {
        Vehicle vehicle = availableVehicles.Find(v => v.vehicleName == vehicleName);
        if (vehicle == null || vehicle.isOwned) return false;
        
        CurrencyManager currency = MasterGameManager.Instance?.Currency;
        int cost = vehicleName == "Pickup Truck" ? 500 : 
                   vehicleName == "Armored Truck" ? 1000 : 2000;
        
        if (currency != null && currency.SpendCoins(cost))
        {
            vehicle.isOwned = true;
            SaveVehicleData();
            DebugLogger.Log($"🚗 Purchased {vehicleName}!");
            return true;
        }
        return false;
    }
    
    public void EquipVehicle(string vehicleName)
    {
        foreach (Vehicle v in availableVehicles)
        {
            v.isEquipped = (v.vehicleName == vehicleName && v.isOwned);
        }
        activeVehicle = availableVehicles.Find(v => v.isEquipped);
        SaveVehicleData();
        DebugLogger.Log($"🔧 Equipped: {activeVehicle?.vehicleName}");
    }
    
    public bool ConsumeFuel(int amount)
    {
        if (totalFuel >= amount)
        {
            totalFuel -= amount;
            SaveVehicleData();
            return true;
        }
        DebugLogger.Log("⛽ Out of fuel!");
        return false;
    }
    
    public void Refuel(int amount)
    {
        totalFuel = Mathf.Min(totalFuel + amount, maxFuel);
        SaveVehicleData();
    }
    
    public int GetAttackBonus()
    {
        return activeVehicle?.attackBonus ?? 0;
    }
    
    public int GetDefenseBonus()
    {
        return activeVehicle?.defenseBonus ?? 0;
    }
    
    void SaveVehicleData()
    {
        PlayerPrefs.SetInt("TotalFuel", totalFuel);
        for (int i = 0; i < availableVehicles.Count; i++)
        {
            PlayerPrefs.SetInt($"Vehicle_{i}_Owned", availableVehicles[i].isOwned ? 1 : 0);
            PlayerPrefs.SetInt($"Vehicle_{i}_Equipped", availableVehicles[i].isEquipped ? 1 : 0);
            PlayerPrefs.SetInt($"Vehicle_{i}_Fuel", availableVehicles[i].fuel);
        }
    }
    
    void LoadVehicleData()
    {
        totalFuel = PlayerPrefs.GetInt("TotalFuel", 100);
        for (int i = 0; i < availableVehicles.Count; i++)
        {
            availableVehicles[i].isOwned = PlayerPrefs.GetInt($"Vehicle_{i}_Owned", 0) == 1;
            availableVehicles[i].isEquipped = PlayerPrefs.GetInt($"Vehicle_{i}_Equipped", 0) == 1;
            availableVehicles[i].fuel = PlayerPrefs.GetInt($"Vehicle_{i}_Fuel", availableVehicles[i].maxFuel);
        }
        activeVehicle = availableVehicles.Find(v => v.isEquipped);
    }
}
