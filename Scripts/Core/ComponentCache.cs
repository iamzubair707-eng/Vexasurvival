using UnityEngine;

public class ComponentCache : MonoBehaviour
{
    private static ComponentCache _instance;
    public static ComponentCache Instance => _instance;
    
    // Cached components for quick access
    public MasterGameManager GameManager { get; private set; }
    public UIManager UI { get; private set; }
    public CurrencyManager Currency { get; private set; }
    public CoreResources Resources { get; private set; }
    public AudioManager Audio { get; private set; }
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            CacheAllComponents();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CacheAllComponents()
    {
        GameManager = MasterGameManager.Instance;
        UI = GameManager?.UIManager;
        Currency = GameManager?.Currency;
        Resources = GameManager?.Resources;
        Audio = GameManager?.Audio;
    }
    
    public T Get<T>() where T : Component
    {
        if (typeof(T) == typeof(MasterGameManager)) return GameManager as T;
        if (typeof(T) == typeof(UIManager)) return UI as T;
        if (typeof(T) == typeof(CurrencyManager)) return Currency as T;
        if (typeof(T) == typeof(CoreResources)) return Resources as T;
        if (typeof(T) == typeof(AudioManager)) return Audio as T;
        return null;
    }
}
