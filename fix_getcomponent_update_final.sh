#!/bin/bash

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "⚡ FIXING GETCOMPONENT IN UPDATE (9 INSTANCES)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Create optimized version of all scripts with cached components
cat > Scripts/UI/PlayerMovementOptimized.cs << 'EOF'
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementOptimized : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Joystick joystick;
    
    // Cached components - set once in Awake
    private Rigidbody2D cachedRigidbody;
    private Animator cachedAnimator;
    private Transform cachedTransform;
    
    private Vector2 movement;
    private bool isMoving;
    
    void Awake()
    {
        // Cache components ONCE - not in Update!
        cachedRigidbody = GetComponent<Rigidbody2D>();
        cachedAnimator = GetComponent<Animator>();
        cachedTransform = transform;
        
        if (cachedRigidbody == null)
            cachedRigidbody = gameObject.AddComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        // Get input
        if (joystick != null)
        {
            movement.x = joystick.Horizontal;
            movement.y = joystick.Vertical;
        }
        else
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        
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
            Vector2 newPos = cachedRigidbody.position + movement * moveSpeed * Time.fixedDeltaTime;
            cachedRigidbody.MovePosition(newPos);
        }
        else
        {
            cachedTransform.Translate(movement * moveSpeed * Time.deltaTime);
        }
    }
    
    public bool IsMoving() => isMoving;
    public Vector2 GetMovementDirection() => movement;
}
