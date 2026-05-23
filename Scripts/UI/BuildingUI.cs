using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public GameObject buildingPanel;
    public Button upgradeButton;
    public Text buildingLevelText;
    public Text upgradeCostText;
    
    private BuildingSystem buildingSystem;
    
    void Start()
    {
        buildingSystem = FindObjectOfType<BuildingSystem>();
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeClick);
    }
    
    void OnUpgradeClick()
    {
        int cost = buildingSystem?.GetUpgradeCost() ?? 50;
        CurrencyManager currency = FindObjectOfType<CurrencyManager>();
        
        if (currency != null && currency.SpendCoins(cost))
        {
            buildingSystem?.UpgradeBuilding(cost);
            UpdateUI();
        }
    }
    
    public void UpdateUI()
    {
        if (buildingLevelText != null && buildingSystem != null)
            buildingLevelText.text = $"Level: {buildingSystem.GetCurrentLevel()}";
        
        if (upgradeCostText != null && buildingSystem != null)
            upgradeCostText.text = $"Cost: {buildingSystem.GetUpgradeCost()} coins";
    }
    
    public void ShowPanel() => buildingPanel?.SetActive(true);
    public void HidePanel() => buildingPanel?.SetActive(false);
}
