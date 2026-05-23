using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public int baseAttack = 50;
    public int baseDefense = 30;
    public int raidPower = 100;
    
    private VehicleManager vehicleManager;
    private DefenseSystem defenseSystem;
    
    void Start()
    {
        vehicleManager = GetComponent<VehicleManager>();
        defenseSystem = GetComponent<DefenseSystem>();
    }
    
    public int CalculateRaidPower()
    {
        int power = raidPower;
        
        // Vehicle bonus
        if (vehicleManager != null && vehicleManager.activeVehicle != null)
        {
            power += (int)vehicleManager.activeVehicle.attackBonus;
        }
        
        // Defense bonus
        if (defenseSystem != null && defenseSystem.towers.Count > 0)
        {
            foreach (var tower in defenseSystem.towers)
                power += tower.damage;
        }
        
        return power;
    }
    
    public void ExecuteRaid(string target, int targetDefense)
    {
        int myPower = CalculateRaidPower();
        
        if (myPower > targetDefense)
        {
            int loot = Random.Range(30, 100);
            DebugLogger.Log($"⚔️ RAID SUCCESS on {target}! Loot: {loot} scrap");
            
            CoreResources resources = GetComponent<CoreResources>();
            resources.AddResource("scrap", loot);
            
            // Add revenge option
            RevengeSystem revenge = GetComponent<RevengeSystem>();
            revenge?.AddRevengeTarget(target);
        }
        else
        {
            int damage = Random.Range(20, 60);
            DebugLogger.Log($"💀 RAID FAILED! Lost {damage} resources!");
            
            CoreResources resources = GetComponent<CoreResources>();
            resources.SpendResource("scrap", damage);
        }
        
        // Update mental health
        MentalHealth mental = GetComponent<MentalHealth>();
        mental?.AddTrauma("Raid", 10);
    }
    
    public void DefendBase(int attackerPower)
    {
        int myDefense = baseDefense;
        
        if (defenseSystem != null && defenseSystem.isShieldActive)
        {
            DebugLogger.Log("🛡️ Shield protected the base!");
            return;
        }
        
        if (attackerPower > myDefense)
        {
            int loss = Random.Range(20, 80);
            DebugLogger.Log($"🏚️ Base breached! Lost {loss} scrap!");
            
            CoreResources resources = GetComponent<CoreResources>();
            resources.SpendResource("scrap", loss);
        }
        else
        {
            DebugLogger.Log("🏆 Defense successful!");
        }
    }
}