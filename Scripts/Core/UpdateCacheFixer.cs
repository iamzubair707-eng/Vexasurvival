using UnityEngine;

public class UpdateCacheFixer : MonoBehaviour
{
    // Example of proper caching - move all GetComponent calls from Update to Awake
    private Rigidbody2D _cachedRigidbody;
    private Animator _cachedAnimator;
    
    void Awake()
    {
        // Cache ONCE - not in Update!
        _cachedRigidbody = GetComponent<Rigidbody2D>();
        _cachedAnimator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Use cached references
        if (_cachedRigidbody != null)
            _cachedRigidbody.velocity = Vector2.zero;
    }
}
