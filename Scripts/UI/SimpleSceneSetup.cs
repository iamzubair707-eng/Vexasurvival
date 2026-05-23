using UnityEngine;

#if UNITY_EDITOR
public class SimpleSceneSetup : MonoBehaviour
{
    void Start()
    {
        // This script is only for editor testing
        DebugLogger.Log("Scene setup complete - Editor mode");
    }
}
#endif
