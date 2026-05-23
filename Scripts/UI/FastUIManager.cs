using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FastUIManager : MonoBehaviour
{
    public static FastUIManager Instance;
    
    [Header("UI References - Assign in Inspector")]
    [SerializeField] private Text woodText;
    [SerializeField] private Text stoneText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text gemText;
    [SerializeField] private Text troopText;
    [SerializeField] private Text levelText;
    
    [Header("Canvas - Assign in Inspector")]
    [SerializeField] private Canvas canvas; // No GameObject.Find!
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void UpdateResources(int wood, int stone, int coins, int gems)
    {
        if (woodText != null) woodText.text = $"🪵 {wood}";
        if (stoneText != null) stoneText.text = $"🪨 {stone}";
        if (coinText != null) coinText.text = $"💰 {coins}";
        if (gemText != null) gemText.text = $"💎 {gems}";
    }
    
    public void UpdateTroops(int count)
    {
        if (troopText != null) troopText.text = $"⚔️ Troops: {count}";
    }
    
    public void UpdateLevel(int level, int currentXP, int neededXP)
    {
        if (levelText != null) levelText.text = $"🌟 Level {level}";
    }
    
    public void ShowMessage(string msg, Color color, float duration = 2f)
    {
        StartCoroutine(ShowMessageCoroutine(msg, color, duration));
    }
    
    IEnumerator ShowMessageCoroutine(string msg, Color color, float duration)
    {
        GameObject go = new GameObject("Message");
        if (canvas != null)
            go.transform.SetParent(canvas.transform);
        
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
