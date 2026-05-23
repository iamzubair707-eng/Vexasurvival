using UnityEngine;

public class AntiCheat : MonoBehaviour
{
    void Update()
    {
        if (Time.timeScale > 1.5f)
        {
            Time.timeScale = 1f;
            // Silent fix - no Debug.Log
        }
    }
}
