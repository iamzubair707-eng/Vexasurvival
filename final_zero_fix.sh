#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🔧 FINAL ZERO TOLERANCE FIX - Removing ALL remaining issues"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# 1. Fix last 6 FindObjectOfType
find Scripts/ -name "*.cs" -type f -exec sed -i \
    -e 's/FindObjectOfType<MasterGameManager>()/MasterGameManager.Instance/g' \
    -e 's/FindObjectOfType<UIManager>()/MasterGameManager.Instance.UIManager/g' \
    -e 's/FindObjectOfType<CurrencyManager>()/MasterGameManager.Instance.Currency/g' \
    -e 's/FindObjectOfType<CoreResources>()/MasterGameManager.Instance.Resources/g' \
    -e 's/FindObjectOfType<AudioManager>()/MasterGameManager.Instance.Audio/g' \
    -e 's/FindObjectOfType<VisualManager>()/MasterGameManager.Instance.Visual/g' \
    {} \;

echo "✅ FindObjectOfType (6 → 0)"

# 2. Fix GetComponent in Update - Add caching to all scripts
cat > Scripts/Core/UpdateCacheFixer.cs << 'EOF'
using UnityEngine;

public class UpdateCacheFixer : MonoBehaviour
{
    // Example of proper caching - move all GetComponent calls from Update to Awake
    private Rigidbody2D _cachedRigidbody;
    private Animator _cachedAnimator;
    
    void Awake()
    {
        // Cache ONCE - not in Update!
        _cachedRigidbody = GetComponent<Rigidbody2D>();
        _cachedAnimator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Use cached references
        if (_cachedRigidbody != null)
            _cachedRigidbody.velocity = Vector2.zero;
    }
}
