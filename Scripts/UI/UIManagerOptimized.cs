using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManagerOptimized : MonoBehaviour
{
    public static UIManagerOptimized Instance;
    
    // Cached UI components
    private Dictionary<string, Text> cachedTexts = new Dictionary<string, Text>();
    private Dictionary<string, Button> cachedButtons = new Dictionary<string, Button>();
    private Dictionary<string, Image> cachedImages = new Dictionary<string, Image>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CacheAllUIComponents();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CacheAllUIComponents()
    {
        // Cache all text components in the scene
        Text[] allTexts = FindObjectsOfType<Text>();
        foreach (Text t in allTexts)
        {
            if (!cachedTexts.ContainsKey(t.name))
                cachedTexts[t.name] = t;
        }
        
        // Cache all buttons
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button b in allButtons)
        {
            if (!cachedButtons.ContainsKey(b.name))
                cachedButtons[b.name] = b;
        }
    }
    
    public void SetText(string textName, string value)
    {
        if (cachedTexts.ContainsKey(textName))
            cachedTexts[textName].text = value;
    }
    
    public void ShowNotification(string message, Color color, float duration = 2f)
    {
        StartCoroutine(ShowNotificationCoroutine(message, color, duration));
    }
    
    IEnumerator ShowNotificationCoroutine(string message, Color color, float duration)
    {
        GameObject notif = new GameObject("Notification");
        Text text = notif.AddComponent<Text>();
        text.text = message;
        text.color = color;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = notif.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 100);
        rect.sizeDelta = new Vector2(400, 60);
        
        yield return new WaitForSeconds(duration);
        Destroy(notif);
    }
    
    public void UpdateResources(int wood, int stone, int coins, int gems)
    {
        SetText("WoodText", $"🪵 {wood}");
        SetText("StoneText", $"🪨 {stone}");
        SetText("CoinText", $"💰 {coins}");
        SetText("GemText", $"💎 {gems}");
    }
    
    public void UpdateTroopCount(int count)
    {
        SetText("TroopText", $"⚔️ Troops: {count}");
    }
    
    public void UpdateLevel(int level, int currentXP, int neededXP)
    {
        SetText("LevelText", $"🌟 Level {level} ({currentXP}/{neededXP} XP)");
    }
}
