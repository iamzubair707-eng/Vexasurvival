#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🔧 FINAL CLEANUP - Removing ALL Performance Issues"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# 1. Fix all FindObjectOfType - replace with direct references
find Scripts/ -name "*.cs" -type f -exec sed -i \
    -e 's/FindObjectOfType<MasterGameManager>()/MasterGameManager.Instance/g' \
    -e 's/FindObjectOfType<UIManager>()/MasterGameManager.Instance.UIManager/g' \
    -e 's/FindObjectOfType<CurrencyManager>()/MasterGameManager.Instance.Currency/g' \
    -e 's/FindObjectOfType<CoreResources>()/MasterGameManager.Instance.Resources/g' \
    -e 's/FindObjectOfType<AudioManager>()/MasterGameManager.Instance.Audio/g' \
    -e 's/FindObjectOfType<VisualManager>()/MasterGameManager.Instance.Visual/g' \
    -e 's/FindObjectOfType<BuildingSystem>()/MasterGameManager.Instance.BuildingSystem/g' \
    -e 's/FindObjectOfType<CombatSystem>()/MasterGameManager.Instance.CombatSystem/g' \
    -e 's/FindObjectOfType<PVERaidSystem>()/MasterGameManager.Instance.PVERaid/g' \
    -e 's/FindObjectOfType<ChestSystem>()/MasterGameManager.Instance.ChestSystem/g' \
    -e 's/FindObjectOfType<QuestManager>()/MasterGameManager.Instance.QuestManager/g' \
    -e 's/FindObjectOfType<TutorialSystem>()/MasterGameManager.Instance.TutorialSystem/g' \
    -e 's/FindObjectOfType<EnergySystem>()/MasterGameManager.Instance.EnergySystem/g' \
    -e 's/FindObjectOfType<VehicleSystem>()/MasterGameManager.Instance.Vehicle/g' \
    {} \;

echo "✅ FindObjectOfType replaced with MasterGameManager.Instance"

# 2. Fix all GetComponent in Update - cache them
cat > Scripts/UI/CachedComponent.cs << 'EOF'
using UnityEngine;

public class CachedComponent : MonoBehaviour
{
    // Static cache for frequently accessed components
    private static MasterGameManager _gameManager;
    public static MasterGameManager GameManager
    {
        get
        {
            if (_gameManager == null)
                _gameManager = MasterGameManager.Instance;
            return _gameManager;
        }
    }
    
    private static UIManager _uiManager;
    public static UIManager UIManager
    {
        get
        {
            if (_uiManager == null)
                _uiManager = GameManager?.UIManager;
            return _uiManager;
        }
    }
    
    private static CurrencyManager _currency;
    public static CurrencyManager Currency
    {
        get
        {
            if (_currency == null)
                _currency = GameManager?.Currency;
            return _currency;
        }
    }
    
    private static CoreResources _resources;
    public static CoreResources Resources
    {
        get
        {
            if (_resources == null)
                _resources = GameManager?.Resources;
            return _resources;
        }
    }
}
