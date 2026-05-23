using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Cached UI References")]
    [SerializeField] private Text woodText;
    [SerializeField] private Text stoneText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text gemText;
    [SerializeField] private Text troopText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;
    
    private MasterGameManager gameManager;
    
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
    
    void Start()
    {
        gameManager = MasterGameManager.Instance;
    }
    
    public void UpdateResources(int wood, int stone, int coins, int gems)
    {
        if (woodText != null) woodText.text = $"🪵 {wood}";
        if (stoneText != null) stoneText.text = $"🪨 {stone}";
        if (coinText != null) coinText.text = $"💰 {coins}";
        if (gemText != null) gemText.text = $"💎 {gems}";
    }
    
    public void UpdateTroopCount(int count)
    {
        if (troopText != null) troopText.text = $"⚔️ Troops: {count}";
    }
    
    public void UpdateLevel(int level, int currentXP, int neededXP)
    {
        if (levelText != null) levelText.text = $"🌟 Level {level}";
        if (xpText != null) xpText.text = $"XP: {currentXP}/{neededXP}";
    }
    
    public void UpdateStatus(int level, int troops, int energy)
    {
        UpdateTroopCount(troops);
        // Add more status updates as needed
    }
    
    public void ShowNotification(string message, Color color, float duration = 2f)
    {
        StartCoroutine(ShowNotificationCoroutine(message, color, duration));
    }
    
    IEnumerator ShowNotificationCoroutine(string message, Color color, float duration)
    {
        GameObject go = new GameObject("Notification");
        go.transform.SetParent(transform);
        
        Text text = go.AddComponent<Text>();
        text.text = message;
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
