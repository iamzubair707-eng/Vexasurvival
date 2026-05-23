using UnityEngine;

public static class DebugLogger
{
    // Set this to false for production builds
    public static bool IsDebugMode = true;
    
    public static void Log(object message)
    {
        if (IsDebugMode)
            DebugLogger.Log(message);
    }
    
    public static void Log(object message, Object context)
    {
        if (IsDebugMode)
            DebugLogger.Log(message, context);
    }
    
    public static void LogWarning(object message)
    {
        if (IsDebugMode)
            DebugLogger.LogWarning(message);
    }
    
    public static void LogWarning(object message, Object context)
    {
        if (IsDebugMode)
            DebugLogger.LogWarning(message, context);
    }
    
    public static void LogError(object message)
    {
        if (IsDebugMode)
            DebugLogger.LogError(message);
    }
    
    public static void LogError(object message, Object context)
    {
        if (IsDebugMode)
            DebugLogger.LogError(message, context);
    }
}
