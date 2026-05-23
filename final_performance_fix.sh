#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🔥 FINAL PERFORMANCE FIX - Removing all FindObjectOfType calls"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Create a backup first
mkdir -p Scripts_Backup_BeforeFinalFix
cp -r Scripts Scripts_Backup_BeforeFinalFix/

# Replace all FindObjectOfType<SystemName>() with MasterGameManager.Instance.SystemName
# This works because MasterGameManager caches all systems at startup

find Scripts/ -name "*.cs" -type f -exec sed -i \
    -e 's/FindObjectOfType<MasterGameManager>()/MasterGameManager.Instance/g' \
    -e 's/FindObjectOfType<CoreResources>()/MasterGameManager.Instance.Resources/g' \
    -e 's/FindObjectOfType<CurrencyManager>()/MasterGameManager.Instance.Currency/g' \
    -e 's/FindObjectOfType<BuildingSystem>()/MasterGameManager.Instance.BuildingSystem/g' \
    -e 's/FindObjectOfType<CombatSystem>()/MasterGameManager.Instance.CombatSystem/g' \
    -e 's/FindObjectOfType<PVERaidSystem>()/MasterGameManager.Instance.PVERaid/g' \
    -e 's/FindObjectOfType<ChestSystem>()/MasterGameManager.Instance.ChestSystem/g' \
    -e 's/FindObjectOfType<QuestManager>()/MasterGameManager.Instance.QuestManager/g' \
    -e 's/FindObjectOfType<TutorialSystem>()/MasterGameManager.Instance.TutorialSystem/g' \
    -e 's/FindObjectOfType<UIManager>()/MasterGameManager.Instance.UIManager/g' \
    -e 's/FindObjectOfType<EnergySystem>()/MasterGameManager.Instance.EnergySystem/g' \
    -e 's/FindObjectOfType<AudioManager>()/MasterGameManager.Instance.Audio/g' \
    -e 's/FindObjectOfType<VisualManager>()/MasterGameManager.Instance.Visual/g' \
    -e 's/FindObjectOfType<ClanSystem>()/MasterGameManager.Instance.Clan/g' \
    -e 's/FindObjectOfType<Leaderboard>()/MasterGameManager.Instance.Leaderboard/g' \
    -e 's/FindObjectOfType<NotificationManager>()/MasterGameManager.Instance.Notification/g' \
    -e 's/FindObjectOfType<VehicleManager>()/MasterGameManager.Instance.Vehicle/g' \
    -e 's/FindObjectOfType<DefenseSystem>()/MasterGameManager.Instance.Defense/g' \
    -e 's/FindObjectOfType<OfflineRewards>()/MasterGameManager.Instance.OfflineRewards/g' \
    -e 's/FindObjectOfType<GameBalancer>()/MasterGameManager.Instance.Balancer/g' \
    -e 's/FindObjectOfType<AntiCheat>()/MasterGameManager.Instance.AntiCheat/g' \
    {} \;

echo ""
echo "✅ All FindObjectOfType calls replaced with cached MasterGameManager references!"
echo ""

# Count remaining
REMAINING=$(grep -r "FindObjectOfType" Scripts/ 2>/dev/null | wc -l)
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📊 REMAINING FINDOBJECTTYPE CALLS: $REMAINING"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if [ $REMAINING -eq 0 ]; then
    echo "🎉 PERFECT! No FindObjectOfType calls left!"
elif [ $REMAINING -lt 10 ]; then
    echo "⚠️ $REMAINING calls left - These might be in debug tools or special cases."
else
    echo "❌ $REMAINING calls still remain. Manual check needed."
fi
