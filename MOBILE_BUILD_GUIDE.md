# 📱 VEXA SURVIVAL - Mobile Build Guide

## Step 1: Unity Setup
1. Open project in Unity Hub
2. File → Build Settings
3. Platform: Android
4. Switch Platform

## Step 2: Player Settings
- Package Name: com.vexasurvival.game
- Version: 1.0.0
- Minimum API Level: Android 7.0 (API 24)
- Target API Level: Android 13 (API 33)

## Step 3: Build APK
1. Build Settings → Build
2. Save as "VexaSurvival_v1.0.apk"
3. Wait for build complete

## Step 4: Install on Phone
1. Transfer APK to phone
2. Enable "Unknown sources"
3. Install APK
4. Open and test

## Daily Test Checklist
- [ ] Resource collection works
- [ ] Building upgrade works
- [ ] Troop training works
- [ ] Raid system works
- [ ] Chest opening works
- [ ] Mental health changes
- [ ] Offline rewards work
- [ ] Save/load works
- [ ] No crashes in 15 min play
- [ ] FPS stable (30+)

## Performance Targets
- Startup time: < 5 seconds
- FPS: 30-60 on mid-range phones
- Memory: < 200 MB
- APK size: < 150 MB
