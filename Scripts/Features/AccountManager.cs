using UnityEngine;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour
{
    public GameObject deleteConfirmPanel;
    public Button deleteButton;
    public Button cancelButton;
    
    void Start()
    {
        deleteButton.onClick.AddListener(ShowDeleteConfirmation);
        cancelButton.onClick.AddListener(HideDeleteConfirmation);
    }
    
    void ShowDeleteConfirmation()
    {
        deleteConfirmPanel.SetActive(true);
    }
    
    void HideDeleteConfirmation()
    {
        deleteConfirmPanel.SetActive(false);
    }
    
    public void DeleteAccount()
    {
        Debug.Log("⚠️ Deleting all user data...");
        
        // Delete all PlayerPrefs
        PlayerPrefs.DeleteAll();
        
        // Clear all saved data
        PlayerPrefs.SetInt("PrivacyAccepted", 0);
        
        Debug.Log("✅ Account deleted successfully!");
        
        HideDeleteConfirmation();
        
        // Restart game or show message
        Application.Quit();
    }
    
    public void ExportUserData()
    {
        UserProfile profile = GetComponent<UserProfile>();
        if (profile != null)
        {
            profile.ShowProfile();
            Debug.Log("📤 User data exported to console");
        }
    }
}