using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;
    public Button buyCoinsButton;
    public Button buyGemsButton;
    public Text coinText;
    public Text gemText;
    
    private CurrencyManager currency;
    
    void Start()
    {
        currency = FindObjectOfType<CurrencyManager>();
        
        if (buyCoinsButton != null)
            buyCoinsButton.onClick.AddListener(() => BuyCoins(100));
        
        if (buyGemsButton != null)
            buyGemsButton.onClick.AddListener(() => BuyGems(10));
    }
    
    void BuyCoins(int amount)
    {
        currency?.AddCoins(amount);
        UIManager ui = FindObjectOfType<UIManager>();
        ui?.ShowNotification($"+{amount} Coins!", Color.green);
    }
    
    void BuyGems(int amount)
    {
        currency?.AddGems(amount);
        UIManager ui = FindObjectOfType<UIManager>();
        ui?.ShowNotification($"+{amount} Gems!", Color.cyan);
    }
    
    void Update()
    {
        if (coinText != null && currency != null)
            coinText.text = $"Coins: {currency.coins}";
        if (gemText != null && currency != null)
            gemText.text = $"Gems: {currency.gems}";
    }
    
    public void ShowPanel() => shopPanel?.SetActive(true);
    public void HidePanel() => shopPanel?.SetActive(false);
}
