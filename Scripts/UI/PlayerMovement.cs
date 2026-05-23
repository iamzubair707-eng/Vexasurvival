using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    private Rigidbody2D _cachedRigidbody;
    private Animator _cachedAnimator;
    
    void Awake()
    {
        _cachedRigidbody = GetComponent<Rigidbody2D>();
        _cachedAnimator = GetComponent<Animator>();
    }
    
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        
        if (_cachedAnimator != null)
        {
            _cachedAnimator.SetFloat("Horizontal", moveX);
            _cachedAnimator.SetFloat("Vertical", moveY);
        }
        
        if (_cachedRigidbody != null)
        {
            _cachedRigidbody.velocity = new Vector2(moveX, moveY) * moveSpeed;
        }
    }
}
