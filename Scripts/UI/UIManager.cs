using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private Text woodText;
    [SerializeField] private Text stoneText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text gemText;
    [SerializeField] private Text troopText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;
    
    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject pauseMenu;
    
    private CanvasScaler _canvasScaler;
    
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
        
        _canvasScaler = GetComponent<CanvasScaler>();
        AdjustForScreenSize();
    }
    
    void AdjustForScreenSize()
    {
        float aspect = (float)Screen.width / Screen.height;
        
        if (_canvasScaler != null)
        {
            if (aspect > 1.7f) // Ultra-wide
                _canvasScaler.matchWidthOrHeight = 0.2f;
            else if (aspect < 1.5f) // Tablet
                _canvasScaler.matchWidthOrHeight = 0.8f;
            else // Standard
                _canvasScaler.matchWidthOrHeight = 0.5f;
        }
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
        if (levelText != null) levelText.text = $"🌟 Level {level}";
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
        text.fontSize = 28;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 150);
        rect.sizeDelta = new Vector2(500, 80);
        
        // Animate
        float elapsed = 0;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.3f;
            go.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        yield return new WaitForSeconds(duration);
        
        elapsed = 0;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float t = 1 - (elapsed / 0.2f);
            go.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            text.color = new Color(color.r, color.g, color.b, t);
            yield return null;
        }
        
        Destroy(go);
    }
    
    public void ShowMainMenu() { mainMenu?.SetActive(true); gameUI?.SetActive(false); }
    public void ShowGameUI() { mainMenu?.SetActive(false); gameUI?.SetActive(true); }
    public void ShowPauseMenu() { pauseMenu?.SetActive(true); }
    public void HidePauseMenu() { pauseMenu?.SetActive(false); }
}
