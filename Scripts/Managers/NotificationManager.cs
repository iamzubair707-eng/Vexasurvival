using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    
    [Header("Notification Settings")]
    public GameObject notificationPrefab;
    public Transform notificationParent;
    public float notificationDuration = 2f;
    
    private Queue<string> notificationQueue = new Queue<string>();
    private bool isShowing = false;
    private GameObject cachedNotification;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Pre-create cached notification object (instead of creating in Update)
        if (notificationPrefab != null && cachedNotification == null)
        {
            cachedNotification = Instantiate(notificationPrefab, notificationParent);
            cachedNotification.SetActive(false);
        }
    }
    
    public void ShowNotification(string message, string type = "info")
    {
        notificationQueue.Enqueue(message);
        if (!isShowing)
            StartCoroutine(ProcessQueue());
    }
    
    IEnumerator ProcessQueue()
    {
        isShowing = true;
        
        while (notificationQueue.Count > 0)
        {
            string message = notificationQueue.Dequeue();
            
            // Reuse cached notification instead of creating new one
            if (cachedNotification != null)
            {
                Text text = cachedNotification.GetComponentInChildren<Text>();
                if (text != null) text.text = message;
                cachedNotification.SetActive(true);
                
                yield return new WaitForSeconds(notificationDuration);
                cachedNotification.SetActive(false);
            }
            else
            {
                // Fallback - create new (only once)
                GameObject notif = Instantiate(notificationPrefab, notificationParent);
                Text newText = notif.GetComponentInChildren<Text>();
                if (newText != null) newText.text = message;
                Destroy(notif, notificationDuration);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        
        isShowing = false;
    }
}
