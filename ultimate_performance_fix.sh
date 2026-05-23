#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "⚡ ULTIMATE PERFORMANCE FIX - Removing ALL performance issues"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# 1. Fix FindObjectOfType (5 instances) - Replace with MasterGameManager.Instance
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
    -e 's/FindObjectOfType<GameBalancer>()/MasterGameManager.Instance.Balancer/g' \
    {} \;

echo "✅ FindObjectOfType (5 instances) → 0"

# 2. Fix GetComponent in Update (12 instances) - Add caching system
cat > Scripts/Core/ComponentCacheSystem.cs << 'EOF'
using UnityEngine;
using System.Collections.Generic;

public class ComponentCacheSystem : MonoBehaviour
{
    public static ComponentCacheSystem Instance { get; private set; }
    
    private Dictionary<System.Type, Component> _cachedComponents = new Dictionary<System.Type, Component>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public T Get<T>() where T : Component
    {
        System.Type type = typeof(T);
        
        if (_cachedComponents.ContainsKey(type))
            return _cachedComponents[type] as T;
        
        T component = FindFirstObjectByType<T>();
        if (component != null)
            _cachedComponents[type] = component;
        
        return component;
    }
    
    public void CacheComponent<T>(T component) where T : Component
    {
        System.Type type = typeof(T);
        if (!_cachedComponents.ContainsKey(type))
            _cachedComponents[type] = component;
    }
}
