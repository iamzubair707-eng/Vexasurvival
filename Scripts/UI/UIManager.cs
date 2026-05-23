using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Main UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject shopPanel;
    public GameObject clanPanel;
    
    [Header("Resource Bars")]
    public Text woodText;
    public Text stoneText;
    public Text coinText;
    public Text gemText;
    
    [Header("Status Texts")]
    public Text levelText;
    public Text troopText;
    public Text raidEnergyText;
    
    [Header("Notifications")]
    public GameObject notificationPrefab;
    public Transform notificationParent;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
            ShowMainMenu();
        else
            ShowGameUI();
    }
    
    public void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
    
    public void ShowGameUI()
    {
        HideAllPanels();
        if (gamePanel != null) gamePanel.SetActive(true);
    }
    
    public void ShowPauseMenu()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }
    
    public void HidePauseMenu()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    
    public void ShowShop()
    {
        if (shopPanel != null) shopPanel.SetActive(true);
    }
    
    public void ShowClan()
    {
        if (clanPanel != null) clanPanel.SetActive(true);
    }
    
    void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (clanPanel != null) clanPanel.SetActive(false);
    }
    
    public void UpdateResources(int wood, int stone, int coins, int gems)
    {
        if (woodText != null) woodText.text = $"🪵 {wood}";
        if (stoneText != null) stoneText.text = $"🪨 {stone}";
        if (coinText != null) coinText.text = $"💰 {coins}";
        if (gemText != null) gemText.text = $"💎 {gems}";
    }
    
    public void UpdateStatus(int level, int troops, int raidEnergy)
    {
        if (levelText != null) levelText.text = $"Level: {level}";
        if (troopText != null) troopText.text = $"Troops: {troops}";
        if (raidEnergyText != null) raidEnergyText.text = $"Raid Energy: {raidEnergy}";
    }
    
    public void ShowNotification(string message, Color color, float duration = 2f)
    {
        if (notificationPrefab != null && notificationParent != null)
        {
            GameObject notif = Instantiate(notificationPrefab, notificationParent);
            Text text = notif.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = message;
                text.color = color;
            }
            Destroy(notif, duration);
        }
        Debug.Log($"📢 {message}");
    }
    
    public void ShowFloatingReward(string text, Vector3 position)
    {
        // Will be implemented with object pooling
        Debug.Log($"✨ {text}");
    }
}