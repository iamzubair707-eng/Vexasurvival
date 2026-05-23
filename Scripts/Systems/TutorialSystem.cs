using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialSystem : MonoBehaviour
{
    public GameObject tutorialPanel;
    public Text tutorialText;
    public Button nextButton;
    
    private int tutorialStep = 0;
    private string[] tutorialMessages = new string[]
    {
        "👋 Welcome to VEXA SURVIVAL!\n\nTap anywhere to continue.",
        
        "🏗️ Your base is destroyed. You need to rebuild!\n\nStep 1: Tap on the ground to move.",
        
        "📦 Step 2: Gather resources.\nTap on the scrap pile to collect SCRAP.",
        
        "🏠 Step 3: Build a Shelter.\nOpen BUILD menu and place a Shelter.",
        
        "🍔 Step 4: Manage hunger and thirst.\nEat food from inventory or gather water.",
        
        "⚔️ Step 5: Raid zombies for more resources!\nOpen RAID menu and select Zombie Horde.",
        
        "👥 Step 6: Recruit survivors.\nBuild BUNK to attract survivors.",
        
        "💚 Step 7: Check mental health.\nIf survivors are depressed, build COUNSELING CENTER.",
        
        "🎁 Step 8: Open free chest every 3 hours!\nChest icon → Claim rewards.",
        
        "🏆 Step 9: Complete daily quests for extra rewards!\nOpen QUESTS menu.",
        
        "🎉 Tutorial complete! Good luck, Commander!\n\nYou're now ready to survive!"
    };
    
    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            StartTutorial();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }
    
    void StartTutorial()
    {
        tutorialPanel.SetActive(true);
        tutorialStep = 0;
        ShowTutorialStep();
        nextButton.onClick.AddListener(NextTutorialStep);
    }
    
    void ShowTutorialStep()
    {
        if (tutorialStep < tutorialMessages.Length)
        {
            tutorialText.text = tutorialMessages[tutorialStep];
            
            // Highlight specific UI elements based on step
            HighlightUIElement(tutorialStep);
        }
    }
    
    void NextTutorialStep()
    {
        tutorialStep++;
        
        if (tutorialStep < tutorialMessages.Length)
        {
            ShowTutorialStep();
        }
        else
        {
            CompleteTutorial();
        }
    }
    
    void HighlightUIElement(int step)
    {
        // This will highlight specific buttons/UI based on tutorial step
        switch (step)
        {
            case 1:
                Debug.Log("🎯 Tutorial: Move character");
                break;
            case 2:
                Debug.Log("🎯 Tutorial: Gather scrap");
                break;
            case 3:
                Debug.Log("🎯 Tutorial: Open Build menu");
                break;
        }
    }
    
    void CompleteTutorial()
    {
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt("TutorialComplete", 1);
        Debug.Log("🎉 Tutorial completed!");
        
        // Give completion reward
        CurrencyManager currency = GetComponent<CurrencyManager>();
        currency?.AddCoins(500);
        currency?.AddGems(50);
        
        NotificationManager notif = GetComponent<NotificationManager>();
        notif?.ShowNotification("🎉 Tutorial Complete!", "+500 Coins, +50 Gems!", "success");
    }
    
    public void SkipTutorial()
    {
        tutorialStep = tutorialMessages.Length;
        CompleteTutorial();
    }
}