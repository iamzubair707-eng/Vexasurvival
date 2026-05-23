using UnityEngine;
using System.Collections.Generic;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance;
    
    public List<Vehicle> availableVehicles = new List<Vehicle>();
    public Vehicle activeVehicle;
    public float globalFuel = 100f;
    public float maxFuel = 500f;
    
    [System.Serializable]
    public class Vehicle
    {
        public string vehicleName;
        public VehicleType type;
        public float speed;
        public float durability;
        public float fuelCapacity;
        public float attackBonus;
        public float defenseBonus;
        public int scrapCost;
        public bool isOwned;
        public bool isEquipped;
        
        public enum VehicleType { Bike, Truck, ArmoredTruck, Tank, Helicopter }
    }
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        InitializeVehicles();
        LoadFuelData();
    }
    
    void InitializeVehicles()
    {
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Motorcycle", type = Vehicle.VehicleType.Bike, 
            speed = 1.5f, durability = 50, fuelCapacity = 50, 
            attackBonus = 10, defenseBonus = 5, scrapCost = 100, 
            isOwned = true, isEquipped = true 
        });
        
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Pickup Truck", type = Vehicle.VehicleType.Truck, 
            speed = 1f, durability = 150, fuelCapacity = 150, 
            attackBonus = 25, defenseBonus = 20, scrapCost = 300, 
            isOwned = false, isEquipped = false 
        });
        
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Armored Truck", type = Vehicle.VehicleType.ArmoredTruck, 
            speed = 0.8f, durability = 300, fuelCapacity = 200, 
            attackBonus = 40, defenseBonus = 50, scrapCost = 600, 
            isOwned = false, isEquipped = false 
        });
        
        availableVehicles.Add(new Vehicle 
        { 
            vehicleName = "Battle Tank", type = Vehicle.VehicleType.Tank, 
            speed = 0.5f, durability = 500, fuelCapacity = 300, 
            attackBonus = 100, defenseBonus = 100, scrapCost = 1500, 
            isOwned = false, isEquipped = false 
        });
    }
    
    public bool PurchaseVehicle(string vehicleName)
    {
        Vehicle vehicle = availableVehicles.Find(v => v.vehicleName == vehicleName);
        if (vehicle == null || vehicle.isOwned) return false;
        
        CoreResources resources = GetComponent<CoreResources>();
        if (resources.SpendResource("scrap", vehicle.scrapCost))
        {
            vehicle.isOwned = true;
            Debug.Log($"🚗 Purchased {vehicleName}!");
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
        Debug.Log($"🔧 Equipped: {activeVehicle?.vehicleName}");
    }
    
    public bool ConsumeFuel(float amount)
    {
        if (globalFuel >= amount)
        {
            globalFuel -= amount;
            SaveFuelData();
            return true;
        }
        Debug.Log("⛽ Out of fuel! Refuel at base!");
        return false;
    }
    
    public void Refuel(float amount)
    {
        globalFuel = Mathf.Min(globalFuel + amount, maxFuel);
        SaveFuelData();
        Debug.Log($"⛽ Refueled! Current fuel: {globalFuel}/{maxFuel}");
    }
    
    void SaveFuelData() => PlayerPrefs.SetFloat("GlobalFuel", globalFuel);
    void LoadFuelData() => globalFuel = PlayerPrefs.GetFloat("GlobalFuel", 100f);
}