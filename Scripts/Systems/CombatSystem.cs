using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public int CalculateRaidPower(int troopCount, int buildingLevel)
    {
        int basePower = 50;
        int troopPower = troopCount * 20;
        int buildingBonus = buildingLevel * 10;
        
        return basePower + troopPower + buildingBonus;
    }
    
    public RaidResult ExecuteRaid(int playerPower, int enemyPower)
    {
        RaidResult result = new RaidResult();
        
        float winChance = (float)playerPower / (playerPower + enemyPower);
        result.isVictory = Random.value < winChance;
        
        if (result.isVictory)
        {
            result.lootAmount = Random.Range(30, 100);
            result.expGain = Random.Range(10, 30);
            Debug.Log($"⚔️ RAID VICTORY! Loot: {result.lootAmount}, EXP: {result.expGain}");
        }
        else
        {
            result.lootAmount = 0;
            result.damageTaken = Random.Range(10, 40);
            Debug.Log($"💀 RAID DEFEAT! Damage: {result.damageTaken}");
        }
        
        return result;
    }
    
    public class RaidResult
    {
        public bool isVictory;
        public int lootAmount;
        public int expGain;
        public int damageTaken;
    }
}