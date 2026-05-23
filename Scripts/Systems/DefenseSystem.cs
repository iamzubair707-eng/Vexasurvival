using UnityEngine;
using System.Collections.Generic;

public class DefenseSystem : MonoBehaviour
{
    public List<DefenseTower> towers = new List<DefenseTower>();
    public bool isShieldActive = false;
    public DateTime shieldEndTime;
    
    [System.Serializable]
    public class DefenseTower
    {
        public string towerName;
        public int level;
        public int damage;
        public int range;
        public int upgradeCost;
    }
    
    void Start()
    {
        LoadShieldStatus();
        InitializeTowers();
    }
    
    void InitializeTowers()
    {
        towers.Add(new DefenseTower { towerName = "Arrow Tower", level = 1, damage = 50, range = 10, upgradeCost = 200 });
        towers.Add(new DefenseTower { towerName = "Cannon Tower", level = 1, damage = 100, range = 15, upgradeCost = 500 });
        towers.Add(new DefenseTower { towerName = "Laser Tower", level = 1, damage = 200, range = 20, upgradeCost = 1000 });
        towers.Add(new DefenseTower { towerName = "Missile Launcher", level = 1, damage = 500, range = 25, upgradeCost = 2000 });
    }
    
    public bool UpgradeTower(string towerName)
    {
        DefenseTower tower = towers.Find(t => t.towerName == towerName);
        if (tower == null) return false;
        
        CurrencyManager currency = GetComponent<CurrencyManager>();
        if (currency.SpendCoins(tower.upgradeCost))
        {
            tower.level++;
            tower.damage += tower.level * 20;
            tower.range += tower.level;
            tower.upgradeCost += 100;
            Debug.Log($"🔧 {tower.towerName} upgraded to level {tower.level}!");
            return true;
        }
        return false;
    }
    
    public void ActivateShield(int hours)
    {
        isShieldActive = true;
        shieldEndTime = DateTime.Now.AddHours(hours);
        PlayerPrefs.SetInt("ShieldActive", 1);
        PlayerPrefs.SetString("ShieldEndTime", shieldEndTime.ToString());
        Debug.Log($"🛡️ Shield activated for {hours} hours!");
    }
    
    public bool IsUnderShield()
    {
        if (isShieldActive && DateTime.Now < shieldEndTime)
            return true;
        else if (isShieldActive && DateTime.Now >= shieldEndTime)
        {
            isShieldActive = false;
            return false;
        }
        return false;
    }
    
    void LoadShieldStatus()
    {
        isShieldActive = PlayerPrefs.GetInt("ShieldActive", 0) == 1;
        string savedTime = PlayerPrefs.GetString("ShieldEndTime", DateTime.Now.ToString());
        shieldEndTime = DateTime.Parse(savedTime);
    }
}