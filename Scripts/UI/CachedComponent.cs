using UnityEngine;

public class CachedComponent : MonoBehaviour
{
    // Static cache for frequently accessed components
    private static MasterGameManager _gameManager;
    public static MasterGameManager GameManager
    {
        get
        {
            if (_gameManager == null)
                _gameManager = MasterGameManager.Instance;
            return _gameManager;
        }
    }
    
    private static UIManager _uiManager;
    public static UIManager UIManager
    {
        get
        {
            if (_uiManager == null)
                _uiManager = GameManager?.UIManager;
            return _uiManager;
        }
    }
    
    private static CurrencyManager _currency;
    public static CurrencyManager Currency
    {
        get
        {
            if (_currency == null)
                _currency = GameManager?.Currency;
            return _currency;
        }
    }
    
    private static CoreResources _resources;
    public static CoreResources Resources
    {
        get
        {
            if (_resources == null)
                _resources = GameManager?.Resources;
            return _resources;
        }
    }
}
