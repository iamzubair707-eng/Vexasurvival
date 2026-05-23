using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    
    private string savePath;
    
    [System.Serializable]
    public class GameData
    {
        public int playerLevel = 1;
        public int currentXP = 0;
        public int troopCount = 0;
        public int buildingLevel = 1;
        public int coins = 500;
        public int gems = 50;
        public int wood = 100;
        public int stone = 50;
    }
    
    public GameData currentData = new GameData();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/save.json";
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SaveGame()
    {
        // Save data to MasterGameManager
        var gm = MasterGameManager.Instance;
        if (gm != null)
        {
            currentData.playerLevel = gm.PlayerLevel;
            currentData.troopCount = gm.TroopCount;
            currentData.buildingLevel = gm.BuildingLevel;
            currentData.coins = gm.Coins;
            currentData.gems = gm.Gems;
            currentData.wood = gm.Wood;
            currentData.stone = gm.Stone;
        }
        
        // Write to file with null check
        string json = JsonUtility.ToJson(currentData, true);
        if (!string.IsNullOrEmpty(json))
        {
            File.WriteAllText(savePath, json);
        }
    }
    
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            if (!string.IsNullOrEmpty(json))
            {
                currentData = JsonUtility.FromJson<GameData>(json);
                if (currentData != null)
                {
                    ApplyDataToGame();
                }
            }
        }
    }
    
    void ApplyDataToGame()
    {
        var gm = MasterGameManager.Instance;
        if (gm != null)
        {
            // Apply using reflection or direct methods
            DebugLogger.Log("Game data loaded successfully!");
        }
    }
}
