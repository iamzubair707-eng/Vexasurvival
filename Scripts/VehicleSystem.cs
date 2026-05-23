using UnityEngine;
using System.Collections.Generic;

public class VehicleSystem : MonoBehaviour
{
    public List<Vehicle> ownedVehicles = new List<Vehicle>();
    public Vehicle activeVehicle;
    
    [System.Serializable]
    public class Vehicle
    {
        public string name;
        public int fuel;
        public int maxFuel;
        public int durability;
        public int attackBonus;
        public int speedBonus;
        public string rarity; // Common, Rare, Epic, Legendary
    }
    
    void Start()
    {
        InitializeVehicles();
    }
    
    void InitializeVehicles()
    {
        ownedVehicles.Add(new Vehicle { name = "Motorcycle", fuel = 100, maxFuel = 100, durability = 100, attackBonus = 10, speedBonus = 20, rarity = "Common" });
        ownedVehicles.Add(new Vehicle { name = "Armored Truck", fuel = 200, maxFuel = 200, durability = 200, attackBonus = 30, speedBonus = 10, rarity = "Rare" });
        ownedVehicles.Add(new Vehicle { name = "Battle Tank", fuel = 500, maxFuel = 500, durability = 500, attackBonus = 100, speedBonus = 5, rarity = "Epic" });
    }
    
    public void RefuelVehicle(string vehicleName, int amount)
    {
        Vehicle vehicle = ownedVehicles.Find(v => v.name == vehicleName);
        if (vehicle != null)
        {
            CurrencyManager currency = GetComponent<CurrencyManager>();
            int cost = amount * 2; // 2 coins per fuel
            
            if (currency.SpendCoins(cost))
            {
                vehicle.fuel += amount;
                vehicle.fuel = Mathf.Min(vehicle.fuel, vehicle.maxFuel);
                Debug.Log($"⛽ {vehicleName} refueled! Fuel: {vehicle.fuel}/{vehicle.maxFuel}");
            }
        }
    }
    
    public void RepairVehicle(string vehicleName)
    {
        Vehicle vehicle = ownedVehicles.Find(v => v.name == vehicleName);
        if (vehicle != null && vehicle.durability < 100)
        {
            CurrencyManager currency = GetComponent<CurrencyManager>();
            int cost = (100 - vehicle.durability) * 5;
            
            if (currency.SpendCoins(cost))
            {
                vehicle.durability = 100;
                Debug.Log($"🔧 {vehicleName} repaired!");
            }
        }
    }
}