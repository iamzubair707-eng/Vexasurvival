# 🎮 VEXA SURVIVAL - Core Loop Test Guide

## Test 1: Resource Collection
- Press 'G' → Gather 10 wood
- Expected: Wood count increases by 10
- UI should update instantly

## Test 2: Building Upgrade
- Press 'U' → Upgrade building
- Expected: Cost 50 coins, level increases
- Notification: "Building level X!"

## Test 3: Train Troop
- Press 'T' → Train soldier
- Expected: Cost 30 coins, troop count +1
- Notification: "Troops: X"

## Test 4: Start Raid
- Press 'R' → Start PvE raid
- Expected: Uses troops, victory/defeat result
- Loot or damage notification

## Test 5: Open Chest
- Press 'C' → Open free chest
- Expected: Random reward (coins/gems/scrap)
- 3-hour cooldown message if empty

## Test 6: Mental Health Test
- Press 'M' → Add trauma (damage mental health)
- Press 'H' → Heal mental health
- Watch state change: Stable → Stressed → Depressed → Rebellious → Insane

## Test 7: Offline Rewards
- Close game → Wait 5 minutes → Open game
- Expected: "Welcome back! +X scrap" notification
