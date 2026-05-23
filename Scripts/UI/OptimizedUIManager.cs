using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptimizedUIManager : MonoBehaviour
{
    public static OptimizedUIManager Instance;
    
    [Header("Cached UI References")]
    private Text woodText;
    private Text stoneText;
    private Text coinText;
    private Text gemText;
    private Text troopText;
    private Text levelText;
    
    [Header("UI Panels")]
    public GameObject resourcePanel;
    public GameObject actionPanel;
    public GameObject notificationPanel;
    
    private MasterGameManager gameManager;
    private Queue<string> notificationQueue = new Queue<string>();
    private bool isShowingNotification = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CacheUIReferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CacheUIReferences()
    {
        // Cache all UI text components at startup
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            woodText = FindTextInChildren(canvas.transform, "WoodText");
            stoneText = FindTextInChildren(canvas.transform, "StoneText");
            coinText = FindTextInChildren(canvas.transform, "CoinText");
            gemText = FindTextInChildren(canvas.transform, "GemText");
            troopText = FindTextInChildren(canvas.transform, "TroopText");
            levelText = FindTextInChildren(canvas.transform, "LevelText");
        }
    }
    
    Text FindTextInChildren(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        if (child != null)
            return child.GetComponent<Text>();
        return null;
    }
    
    void Start()
    {
        gameManager = MasterGameManager.Instance;
        StartCoroutine(ProcessNotificationQueue());
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
        if (levelText != null) levelText.text = $"🌟 Level {level} ({currentXP}/{neededXP} XP)";
    }
    
    public void ShowNotification(string message, Color color, float duration = 2f)
    {
        notificationQueue.Enqueue(message);
    }
    
    IEnumerator ProcessNotificationQueue()
    {
        while (true)
        {
            if (notificationQueue.Count > 0 && !isShowingNotification)
            {
                string message = notificationQueue.Dequeue();
                isShowingNotification = true;
                
                // Create floating notification
                GameObject notifGO = new GameObject("Notification");
                notifGO.transform.SetParent(notificationPanel?.transform ?? null);
                
                Text text = notifGO.AddComponent<Text>();
                text.text = message;
                text.color = Color.white;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 24;
                text.alignment = TextAnchor.MiddleCenter;
                
                RectTransform rect = notifGO.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, 100);
                rect.sizeDelta = new Vector2(400, 60);
                
                // Fade out and destroy
                yield return new WaitForSeconds(duration);
                Destroy(notifGO);
                isShowingNotification = false;
            }
            
            yield return null;
        }
    }
    
    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
            StartCoroutine(FadeInPanel(panel));
    }
    
    IEnumerator FadeInPanel(GameObject panel)
    {
        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group == null) group = panel.AddComponent<CanvasGroup>();
        
        panel.SetActive(true);
        group.alpha = 0;
        
        float duration = 0.3f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        
        group.alpha = 1;
    }
    
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
            StartCoroutine(FadeOutPanel(panel));
    }
    
    IEnumerator FadeOutPanel(GameObject panel)
    {
        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group == null) yield break;
        
        float duration = 0.2f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }
        
        panel.SetActive(false);
        group.alpha = 1;
    }
}
