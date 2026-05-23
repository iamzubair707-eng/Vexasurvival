#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🔧 FINAL FIX - Removing all FindObjectOfType calls"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Fix any remaining FindObjectOfType calls
find Scripts/ -name "*.cs" -type f -exec sed -i \
    -e 's/FindObjectOfType<MasterGameManager>()/MasterGameManager.Instance/g' \
    -e 's/FindObjectOfType<UIManager>()/MasterGameManager.Instance.UIManager/g' \
    -e 's/FindObjectOfType<CurrencyManager>()/MasterGameManager.Instance.Currency/g' \
    -e 's/FindObjectOfType<CoreResources>()/MasterGameManager.Instance.Resources/g' \
    -e 's/FindObjectOfType<AudioManager>()/MasterGameManager.Instance.Audio/g' \
    -e 's/FindObjectOfType<VisualManager>()/MasterGameManager.Instance.Visual/g' \
    {} \;

echo "✅ All FindObjectOfType replaced with cached references!"

# Count remaining
REMAINING=$(grep -r "FindObjectOfType" Scripts/ 2>/dev/null | wc -l)
echo "📊 Remaining FindObjectOfType: $REMAINING"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
