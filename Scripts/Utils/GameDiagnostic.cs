using UnityEngine;

#if UNITY_EDITOR
public class GameDiagnostic : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            DebugLogger.Log("Diagnostic: Use MasterGameManager.Instance to check status");
        }
    }
}
#endif
