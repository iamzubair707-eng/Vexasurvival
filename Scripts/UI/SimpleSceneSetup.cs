using UnityEngine;
using UnityEngine.UI;

public class SimpleSceneSetup : MonoBehaviour
{
    void Start()
    {
        CreateSimpleUI();
        CreatePlayer();
        CreateGround();
        DebugLogger.Log("✅ Scene Setup Complete! Use F12 for diagnostic.");
    }
    
    void CreateSimpleUI()
    {
        // Create Canvas if not exists
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create status text at top
        GameObject statusText = new GameObject("StatusText");
        statusText.transform.SetParent(canvas.transform);
        Text text = statusText.AddComponent<Text>();
        text.text = "✅ Game Running! Press F12 for full diagnostic.\nGather resources -> Upgrade -> Train -> Raid";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = statusText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.9f);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    
    void CreatePlayer()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = Vector3.zero;
            
            // Add movement
            PlayerMovement movement = player.AddComponent<PlayerMovement>();
            movement.moveSpeed = 5f;
            
            // Add collider
            player.AddComponent<BoxCollider>();
            
            // Add Rigidbody
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            
            // Add camera follow
            Camera.main.transform.position = new Vector3(0, 0, -10);
            
            DebugLogger.Log("✅ Player created at position 0,0");
        }
    }
    
    void CreateGround()
    {
        GameObject ground = GameObject.Find("Ground");
        if (ground == null)
        {
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10, 1, 10);
            ground.AddComponent<BoxCollider>();
            
            // Change material color to green/brown
            Renderer renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.4f, 0.3f, 0.2f); // Brown ground
            }
            
            DebugLogger.Log("✅ Ground created");
        }
    }
}