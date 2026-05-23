using UnityEngine;
using UnityEngine.UI;

public class ResponsiveUI : MonoBehaviour
{
    void Start()
    {
        AdjustForScreenSize();
    }
    
    void AdjustForScreenSize()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float aspect = screenWidth / screenHeight;
        
        // Adjust UI scale based on screen size
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            if (aspect > 1.7f) // Ultra-wide
            {
                scaler.matchWidthOrHeight = 0.2f;
            }
            else if (aspect < 1.5f) // Tablet
            {
                scaler.matchWidthOrHeight = 0.8f;
            }
            else // Standard
            {
                scaler.matchWidthOrHeight = 0.5f;
            }
        }
        
        DebugLogger.Log($"📱 Screen: {screenWidth}x{screenHeight}, Aspect: {aspect:F2}");
    }
}
