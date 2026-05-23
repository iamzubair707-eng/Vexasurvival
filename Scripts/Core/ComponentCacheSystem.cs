using UnityEngine;
using System.Collections.Generic;

public class ComponentCacheSystem : MonoBehaviour
{
    public static ComponentCacheSystem Instance { get; private set; }
    
    private Dictionary<System.Type, Component> _cachedComponents = new Dictionary<System.Type, Component>();
    
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
    
    public T Get<T>() where T : Component
    {
        System.Type type = typeof(T);
        
        if (_cachedComponents.ContainsKey(type))
            return _cachedComponents[type] as T;
        
        T component = FindFirstObjectByType<T>();
        if (component != null)
            _cachedComponents[type] = component;
        
        return component;
    }
    
    public void CacheComponent<T>(T component) where T : Component
    {
        System.Type type = typeof(T);
        if (!_cachedComponents.ContainsKey(type))
            _cachedComponents[type] = component;
    }
}
