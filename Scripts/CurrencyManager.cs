using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    
    public int coins = 0;
    public int gems = 0;
    public int tokens = 0;  // VEXA tokens
    
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadCurrencies();
        }
        else
            Destroy(gameObject);
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke(coins);
        SaveCurrencies();
        Debug.Log($"💰 +{amount} Coins! Total: {coins}");
    }
    
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            OnCoinsChanged?.Invoke(coins);
            SaveCurrencies();
            return true;
        }
        Debug.Log($"❌ Not enough coins! Need {amount}, have {coins}");
        return false;
    }
    
    public void AddGems(int amount)
    {
        gems += amount;
        OnGemsChanged?.Invoke(gems);
        SaveCurrencies();
        Debug.Log($"💎 +{amount} Gems! Total: {gems}");
    }
    
    public bool SpendGems(int amount)
    {
        if (gems >= amount)
        {
            gems -= amount;
            OnGemsChanged?.Invoke(gems);
            SaveCurrencies();
            return true;
        }
        return false;
    }
    
    void SaveCurrencies()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.SetInt("Tokens", tokens);
    }
    
    void LoadCurrencies()
    {
        coins = PlayerPrefs.GetInt("Coins", 100);  // Start with 100 coins
        gems = PlayerPrefs.GetInt("Gems", 10);     // Start with 10 gems
        tokens = PlayerPrefs.GetInt("Tokens", 5);
    }
}