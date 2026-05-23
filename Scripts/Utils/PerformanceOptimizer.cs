using UnityEngine;

public class PerformanceOptimizer : MonoBehaviour
{
    [Header("Quality Settings")]
    public bool enableShadows = false;
    public int maxFPS = 60;
    public int maxParticles = 50;
    public float objectPoolingEnabled = true;
    
    [Header("LOD Settings")]
    public float lodDistance = 30f;
    public float cullDistance = 50f;
    
    void Start()
    {
        DetectDevicePerformance();
        ApplyOptimizations();
    }
    
    void DetectDevicePerformance()
    {
        // Check if running on mobile
        #if UNITY_ANDROID || UNITY_IOS
        int processorCount = SystemInfo.processorCount;
        int memorySize = SystemInfo.systemMemorySize;
        
        if (processorCount <= 4 || memorySize <= 2048)
        {
            // Low-end device
            enableShadows = false;
            maxParticles = 20;
            maxFPS = 30;
            lodDistance = 20f;
            cullDistance = 40f;
            Debug.Log("📱 Low-end device detected! Applying performance settings.");
        }
        else
        {
            // Mid/High-end device
            enableShadows = false; // Keep false for mobile
            maxParticles = 50;
            maxFPS = 60;
            Debug.Log("📱 High-end device detected!");
        }
        #endif
    }
    
    void ApplyOptimizations()
    {
        // Frame rate capping
        Application.targetFrameRate = maxFPS;
        
        // Disable V-Sync for mobile
        QualitySettings.vSyncCount = 0;
        
        // Shadow settings
        if (!enableShadows)
        {
            QualitySettings.shadows = ShadowQuality.Disable;
        }
        
        // Texture quality
        QualitySettings.globalTextureMipmapLimit = 1;
        
        // Particle limits
        QualitySettings.particleRaycastBudget = maxParticles;
        
        // LOD and culling
        Camera.main.farClipPlane = cullDistance;
        
        // Garbage collection
        InvokeRepeating("CleanMemory", 60f, 60f);
        
        Debug.Log($"⚡ Performance optimized for mobile: {maxFPS} FPS, Shadows: {enableShadows}");
    }
    
    void CleanMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Reduce performance when backgrounded
            Application.targetFrameRate = 10;
        }
        else
        {
            // Restore when foreground
            Application.targetFrameRate = maxFPS;
        }
    }
}