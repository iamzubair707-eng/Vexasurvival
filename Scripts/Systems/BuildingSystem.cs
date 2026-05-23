using UnityEngine;
using System.Collections.Generic;

public class BuildingSystem : MonoBehaviour
{
    private int currentLevel = 1;
    private int maxLevel = 10;
    
    public int GetCurrentLevel() => currentLevel;
    
    public bool UpgradeBuilding(int cost)
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Maximum level reached!");
            return false;
        }
        
        CurrencyManager currency = FindObjectOfType<CurrencyManager>();
        if (currency != null && currency.SpendCoins(cost))
        {
            currentLevel++;
            Debug.Log($"🏗️ Building upgraded to level {currentLevel}!");
            
            // Apply upgrade effects
            ApplyUpgradeBonus();
            return true;
        }
        return false;
    }
    
    void ApplyUpgradeBonus()
    {
        // Each level gives +20% resource production
        float bonus = 1f + (currentLevel * 0.2f);
        PlayerPrefs.SetFloat("ProductionBonus", bonus);
        Debug.Log($"Production bonus: {bonus}x");
    }
    
    public int GetUpgradeCost()
    {
        return 50 * currentLevel;
    }
    
    public float GetProductionMultiplier()
    {
        return PlayerPrefs.GetFloat("ProductionBonus", 1f);
    }
}