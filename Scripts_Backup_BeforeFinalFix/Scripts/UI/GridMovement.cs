using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

/// <summary>
/// GridMovement — Player tap kare toh us tile pe smoothly move karta hai.
/// Attach karo Player GameObject pe.
/// </summary>
public class GridMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    public float tileSize = 1f;          // Har tile ka size (units mein)
    public float moveSpeed = 5f;         // Player ki movement speed

    [Header("Layer Settings")]
    public LayerMask walkableLayer;      // Sirf walkable tiles pe move hoga

    private Vector3 targetPosition;     // Jahan player jaana chahta hai
    private bool isMoving = false;      // Player abhi move kar raha hai ya nahi

    private Camera mainCamera;

    void Start()
    {
        // Main camera cache karo (har frame Camera.main call karna costly hai)
        mainCamera = Camera.main;

        // Starting position ko grid ke saath snap karo
        targetPosition = SnapToGrid(transform.position);
        transform.position = targetPosition;
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
    }

    // ---------------------------------------------------------
    // INPUT HANDLING
    // ---------------------------------------------------------

    void HandleInput()
    {
        // Mobile: touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Sirf tab process karo jab finger screen pe rakhein
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(touch.position);
                TryMoveTo(worldPos);
            }
        }

        // PC Testing ke liye: mouse click bhi kaam karega
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            TryMoveTo(worldPos);
        }
    }

    // ---------------------------------------------------------
    // MOVE LOGIC
    // ---------------------------------------------------------

    /// <summary>
    /// Check karo kya tapped tile walkable hai, agar hai toh target set karo.
    /// </summary>
    void TryMoveTo(Vector3 worldPosition)
    {
        // Z-axis zero karo (2D game hai)
        worldPosition.z = 0f;

        // Tapped position ko nearest grid tile pe snap karo
        Vector3 snappedPos = SnapToGrid(worldPosition);

        // Check karo kya wahan koi walkable tile hai
        Collider2D hit = Physics2D.OverlapPoint(snappedPos, walkableLayer);

        if (hit != null)
        {
            // Walkable tile mili — target update karo
            targetPosition = snappedPos;
        }
        else
        {
            DebugLogger.Log("Tile walkable nahi hai ya koi tile nahi mili.");
        }
    }

    /// <summary>
    /// Player ko target position ki taraf smoothly move karo.
    /// </summary>
    void MovePlayer()
    {
        if (transform.position != targetPosition)
        {
            isMoving = true;

            // MoveTowards — speed se target ki taraf jao, overshoot nahi hoga
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            isMoving = false;
        }
    }

    // ---------------------------------------------------------
    // HELPER
    // ---------------------------------------------------------

    /// <summary>
    /// Kisi bhi world position ko nearest grid tile center pe snap karo.
    /// </summary>
    Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / tileSize) * tileSize;
        float y = Mathf.Round(position.y / tileSize) * tileSize;
        return new Vector3(x, y, 0f);
    }

    // ---------------------------------------------------------
    // PUBLIC HELPERS (optional use)
    // ---------------------------------------------------------

    /// <summary>
    /// Returns true agar player abhi move kar raha hai.
    /// </summary>
    public bool IsMoving() => isMoving;

    /// <summary>
    /// Player ki current grid position return karta hai.
    /// </summary>
    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x / tileSize),
            Mathf.RoundToInt(transform.position.y / tileSize)
        );
    }
}