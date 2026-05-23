using UnityEngine;

public class CoreLoopTester : MonoBehaviour
{
    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 220, 10, 210, 180), "🎮 CORE LOOP TEST");
        
        if (GUI.Button(new Rect(Screen.width - 210, 45, 190, 25), "1. Gather Resources"))
            MasterGameManager.Instance?.GatherResource("wood", 10);
        
        if (GUI.Button(new Rect(Screen.width - 210, 75, 190, 25), "2. Upgrade Building"))
            MasterGameManager.Instance?.UpgradeBuilding();
        
        if (GUI.Button(new Rect(Screen.width - 210, 105, 190, 25), "3. Train Troop"))
            MasterGameManager.Instance?.TrainTroop();
        
        if (GUI.Button(new Rect(Screen.width - 210, 135, 190, 25), "4. Start Raid"))
            MasterGameManager.Instance?.StartRaid();
        
        if (GUI.Button(new Rect(Screen.width - 210, 165, 190, 25), "5. Open Chest"))
            MasterGameManager.Instance?.OpenChest();
        
        var gm = MasterGameManager.Instance;
        if (gm != null)
        {
            GUI.Label(new Rect(10, Screen.height - 80, 200, 20), $"Troops: {gm.GetTroopCount()}");
            GUI.Label(new Rect(10, Screen.height - 60, 200, 20), $"Level: {gm.GetPlayerLevel()}");
            GUI.Label(new Rect(10, Screen.height - 40, 200, 20), $"Building: {gm.GetBuildingLevel()}");
        }
    }
}
