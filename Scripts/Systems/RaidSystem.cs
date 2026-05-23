using UnityEngine;
using System.Collections;

public class RaidSystem : MonoBehaviour
{
    private ResourceManager resourceManager;
    private HealthSystem healthSystem;
    private BuildingManager buildingManager;
    
    public int raidPower = 50;
    public float raidCooldown = 300f; // 5 minutes
    private bool canRaid = true;
    
    void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        healthSystem = GetComponent<HealthSystem>();
        buildingManager = GetComponent<BuildingManager>();
    }
    
    public void StartRaid(string targetPlayerId)
    {
        if (!canRaid)
        {
            Debug.Log($"Raid on cooldown! Wait {raidCooldown} seconds");
            return;
        }
        
        Debug.Log($"🔥 Raiding player: {targetPlayerId}");
        
        // Simulate raid battle
        int playerPower = CalculatePlayerPower();
        int lootAmount = Random.Range(10, 50);
        
        if (playerPower > raidPower)
        {
            // Win raid
            resourceManager.AddResource("wood", lootAmount);
            resourceManager.AddResource("stone", lootAmount / 2);
            resourceManager.AddResource("vexa", lootAmount / 10);
            Debug.Log($"✅ Raid WON! Loot: {lootAmount} wood, {lootAmount/2} stone");
            
            // Add to leaderboard
            AddRaidWin();
        }
        else
        {
            // Lose raid - get damage
            int damage = Random.Range(20, 60);
            healthSystem.TakeDamage(damage);
            Debug.Log($"❌ Raid LOST! Took {damage} damage");
        }
        
        StartCoroutine(RaidCooldown());
    }
    
    int CalculatePlayerPower()
    {
        int power = raidPower;
        
        // Add building bonuses
        power += resourceManager.wood / 10;
        power += resourceManager.stone / 10;
        
        return power;
    }
    
    void AddRaidWin()
    {
        int wins = PlayerPrefs.GetInt("RaidWins", 0);
        wins++;
        PlayerPrefs.SetInt("RaidWins", wins);
        PlayerPrefs.SetInt("TotalVexa", PlayerPrefs.GetInt("TotalVexa", 0) + resourceManager.vexaTokens);
    }
    
    IEnumerator RaidCooldown()
    {
        canRaid = false;
        yield return new WaitForSeconds(raidCooldown);
        canRaid = true;
        Debug.Log("⚔️ Raid ready again!");
    }
}