using UnityEngine;

public class OptimizedPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Cached Components")]
    private Rigidbody2D cachedRigidbody;
    private Animator cachedAnimator;
    private Transform cachedTransform;
    
    private Vector2 movement;
    private bool isMoving;
    
    void Awake()
    {
        // Cache components once at startup - NOT in Update!
        cachedRigidbody = GetComponent<Rigidbody2D>();
        cachedAnimator = GetComponent<Animator>();
        cachedTransform = transform;
        
        if (cachedRigidbody == null)
            cachedRigidbody = gameObject.AddComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        isMoving = movement.sqrMagnitude > 0.01f;
        
        // Update animator using cached reference
        if (cachedAnimator != null)
        {
            cachedAnimator.SetFloat("Horizontal", movement.x);
            cachedAnimator.SetFloat("Vertical", movement.y);
            cachedAnimator.SetBool("IsMoving", isMoving);
        }
    }
    
    void FixedUpdate()
    {
        if (cachedRigidbody != null)
        {
            Vector2 newPosition = cachedRigidbody.position + movement * moveSpeed * Time.fixedDeltaTime;
            cachedRigidbody.MovePosition(newPosition);
        }
        else
        {
            cachedTransform.Translate(movement * moveSpeed * Time.deltaTime);
        }
    }
    
    public bool IsMoving() => isMoving;
    public Vector2 GetMovementDirection() => movement;
}
