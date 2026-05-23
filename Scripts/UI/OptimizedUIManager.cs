using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OptimizedUIManager : MonoBehaviour
{
    public static OptimizedUIManager Instance;
    
    [Header("UI References - Assign in Inspector")]
    [SerializeField] private Text woodText;
    [SerializeField] private Text stoneText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text gemText;
    [SerializeField] private Text troopText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;
    
    [Header("UI Panels - Assign in Inspector")]
    [SerializeField] private GameObject resourcePanel;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject notificationPanel;
    
    [Header("Canvas - Assign in Inspector")]
    [SerializeField] private Canvas canvas; // No GameObject.Find!
    
    private MasterGameManager gameManager;
    private Queue<string> notificationQueue = new Queue<string>();
    private bool isShowingNotification = false;
    
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
        
        gameManager = MasterGameManager.Instance;
    }
    
    void Start()
    {
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
        if (levelText != null) levelText.text = $"🌟 Level {level}";
        if (xpText != null) xpText.text = $"XP: {currentXP}/{neededXP}";
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
                
                // Create notification using canvas reference
                GameObject notifGO = new GameObject("Notification");
                if (canvas != null)
                    notifGO.transform.SetParent(canvas.transform);
                
                Text text = notifGO.AddComponent<Text>();
                text.text = message;
                text.color = Color.white;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 24;
                text.alignment = TextAnchor.MiddleCenter;
                
                RectTransform rect = notifGO.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, 100);
                rect.sizeDelta = new Vector2(400, 60);
                
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
    
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
            StartCoroutine(FadeOutPanel(panel));
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
