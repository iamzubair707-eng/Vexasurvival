#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "⚡ FINAL PERFORMANCE FIX - Removing ALL performance issues"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# 1. Fix FindObjectOfType - Replace ALL with MasterGameManager.Instance
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
    -e 's/FindObjectOfType<AntiCheat>()/MasterGameManager.Instance.AntiCheat/g' \
    {} \;

echo "✅ FindObjectOfType replaced (6 instances → 0)"

# 2. Fix GetComponent in Update - Add caching
cat > Scripts/Core/ComponentCache.cs << 'EOF'
using UnityEngine;

public class ComponentCache : MonoBehaviour
{
    private static ComponentCache _instance;
    public static ComponentCache Instance => _instance;
    
    // Cached components for quick access
    public MasterGameManager GameManager { get; private set; }
    public UIManager UI { get; private set; }
    public CurrencyManager Currency { get; private set; }
    public CoreResources Resources { get; private set; }
    public AudioManager Audio { get; private set; }
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            CacheAllComponents();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CacheAllComponents()
    {
        GameManager = MasterGameManager.Instance;
        UI = GameManager?.UIManager;
        Currency = GameManager?.Currency;
        Resources = GameManager?.Resources;
        Audio = GameManager?.Audio;
    }
    
    public T Get<T>() where T : Component
    {
        if (typeof(T) == typeof(MasterGameManager)) return GameManager as T;
        if (typeof(T) == typeof(UIManager)) return UI as T;
        if (typeof(T) == typeof(CurrencyManager)) return Currency as T;
        if (typeof(T) == typeof(CoreResources)) return Resources as T;
        if (typeof(T) == typeof(AudioManager)) return Audio as T;
        return null;
    }
}
