using UnityEngine;
using UnityEngine.UI;

public class PrivacyPolicy : MonoBehaviour
{
    public GameObject privacyPanel;
    public Text policyText;
    public Button acceptButton;
    public Button declineButton;
    
    void Start()
    {
        if (PlayerPrefs.GetInt("PrivacyAccepted", 0) == 0)
        {
            ShowPrivacyPolicy();
        }
        
        acceptButton.onClick.AddListener(AcceptPrivacy);
        declineButton.onClick.AddListener(DeclinePrivacy);
    }
    
    void ShowPrivacyPolicy()
    {
        privacyPanel.SetActive(true);
        policyText.text = GetPrivacyText();
    }
    
    string GetPrivacyText()
    {
        return "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
               "🔒 PRIVACY POLICY - VEXA SURVIVAL\n" +
               "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
               "We collect:\n" +
               "• Username and game progress\n" +
               "• Play time and statistics\n" +
               "• Device info for performance\n\n" +
               "We DO NOT collect:\n" +
               "• Real name, email, or phone\n" +
               "• Location data\n" +
               "• Personal financial info\n\n" +
               "Your data stays on your device.\n" +
               "We don't sell or share your data.\n\n" +
               "You can delete all data anytime:\n" +
               "Settings → Delete Account\n\n" +
               "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
               "Do you accept these terms?";
    }
    
    void AcceptPrivacy()
    {
        PlayerPrefs.SetInt("PrivacyAccepted", 1);
        privacyPanel.SetActive(false);
        Debug.Log("✅ Privacy policy accepted");
    }
    
    void DeclinePrivacy()
    {
        Debug.Log("❌ Privacy policy declined. Game will not save data.");
        privacyPanel.SetActive(false);
        // Optional: Disable all save features
        PlayerPrefs.SetInt("PrivacyAccepted", 0);
    }
}