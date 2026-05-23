using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    // Resources
    public int wood = 0;
    public int stone = 0;
    public int food = 0;
    public int vexaTokens = 0;
    
    // UI Text elements (drag in Unity later)
    public Text woodText;
    public Text stoneText;
    public Text foodText;
    public Text vexaText;
    
    void Start()
    {
        LoadResources();
        UpdateUI();
        
        // Start resource generation every 30 seconds
        InvokeRepeating("GeneratePassiveResources", 30f, 30f);
    }
    
    public void AddResource(string type, int amount)
    {
        switch (type)
        {
            case "wood":
                wood += amount;
                break;
            case "stone":
                stone += amount;
                break;
            case "food":
                food += amount;
                break;
            case "vexa":
                vexaTokens += amount;
                break;
        }
        UpdateUI();
        SaveResources();
    }
    
    public bool SpendResource(string type, int amount)
    {
        switch (type)
        {
            case "wood":
                if (wood >= amount)
                {
                    wood -= amount;
                    UpdateUI();
                    SaveResources();
                    return true;
                }
                break;
            case "stone":
                if (stone >= amount)
                {
                    stone -= amount;
                    UpdateUI();
                    SaveResources();
                    return true;
                }
                break;
            case "food":
                if (food >= amount)
                {
                    food -= amount;
                    UpdateUI();
                    SaveResources();
                    return true;
                }
                break;
            case "vexa":
                if (vexaTokens >= amount)
                {
                    vexaTokens -= amount;
                    UpdateUI();
                    SaveResources();
                    return true;
                }
                break;
        }
        DebugLogger.Log($"Not enough {type}!");
        return false;
    }
    
    void GeneratePassiveResources()
    {
        // Based on buildings owned (simplified for now)
        wood += 5;
        stone += 3;
        food += 4;
        UpdateUI();
        SaveResources();
        DebugLogger.Log("Passive resources generated!");
    }
    
    void UpdateUI()
    {
        if (woodText != null) woodText.text = $"Wood: {wood}";
        if (stoneText != null) stoneText.text = $"Stone: {stone}";
        if (foodText != null) foodText.text = $"Food: {food}";
        if (vexaText != null) vexaText.text = $"VEXA: {vexaTokens}";
    }
    
    void SaveResources()
    {
        PlayerPrefs.SetInt("Wood", wood);
        PlayerPrefs.SetInt("Stone", stone);
        PlayerPrefs.SetInt("Food", food);
        PlayerPrefs.SetInt("Vexa", vexaTokens);
    }
    
    void LoadResources()
    {
        wood = PlayerPrefs.GetInt("Wood", 0);
        stone = PlayerPrefs.GetInt("Stone", 0);
        food = PlayerPrefs.GetInt("Food", 0);
        vexaTokens = PlayerPrefs.GetInt("Vexa", 0);
    }
}