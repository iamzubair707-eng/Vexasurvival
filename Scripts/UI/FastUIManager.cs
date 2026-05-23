using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FastUIManager : MonoBehaviour
{
    public static FastUIManager Instance;
    
    // Cached UI elements - no FindObjectOfType, no GetComponent in Update
    private Text cachedWoodText;
    private Text cachedStoneText;
    private Text cachedCoinText;
    private Text cachedGemText;
    private Text cachedTroopText;
    private Text cachedLevelText;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CacheUIElements();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CacheUIElements()
    {
        // Find and cache once - never again
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas == null) return;
        
        cachedWoodText = FindText(canvas.transform, "WoodText");
        cachedStoneText = FindText(canvas.transform, "StoneText");
        cachedCoinText = FindText(canvas.transform, "CoinText");
        cachedGemText = FindText(canvas.transform, "GemText");
        cachedTroopText = FindText(canvas.transform, "TroopText");
        cachedLevelText = FindText(canvas.transform, "LevelText");
    }
    
    Text FindText(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        return child != null ? child.GetComponent<Text>() : null;
    }
    
    public void UpdateResources(int wood, int stone, int coins, int gems)
    {
        if (cachedWoodText != null) cachedWoodText.text = $"🪵 {wood}";
        if (cachedStoneText != null) cachedStoneText.text = $"🪨 {stone}";
        if (cachedCoinText != null) cachedCoinText.text = $"💰 {coins}";
        if (cachedGemText != null) cachedGemText.text = $"💎 {gems}";
    }
    
    public void UpdateTroops(int count)
    {
        if (cachedTroopText != null) cachedTroopText.text = $"⚔️ Troops: {count}";
    }
    
    public void UpdateLevel(int level, int currentXP, int neededXP)
    {
        if (cachedLevelText != null) cachedLevelText.text = $"🌟 Level {level} ({currentXP}/{neededXP} XP)";
    }
    
    public void ShowMessage(string msg, Color color, float duration = 2f)
    {
        StartCoroutine(ShowMessageCoroutine(msg, color, duration));
    }
    
    IEnumerator ShowMessageCoroutine(string msg, Color color, float duration)
    {
        GameObject go = new GameObject("Message");
        Text text = go.AddComponent<Text>();
        text.text = msg;
        text.color = color;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 100);
        rect.sizeDelta = new Vector2(400, 60);
        
        yield return new WaitForSeconds(duration);
        Destroy(go);
    }
}
