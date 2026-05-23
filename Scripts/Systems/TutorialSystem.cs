using UnityEngine;
using System.Collections;

public class TutorialSystem : MonoBehaviour
{
    private int currentStep = 0;
    private UIManager uiManager;
    private MasterGameManager gameManager;
    
    private string[] tutorialMessages = new string[]
    {
        "👋 Welcome to VEXA SURVIVAL!\n\nTap anywhere to start your journey!",
        "📦 Step 1: Collect Resources\n\nTap on GATHER button to collect wood and stone!",
        "🏗️ Step 2: Upgrade Building\n\nTap UPGRADE to improve your base!",
        "⚔️ Step 3: Train Troops\n\nTap TRAIN to recruit soldiers!",
        "🎯 Step 4: Start a Raid\n\nTap RAID to attack enemies and earn rewards!",
        "🎁 Step 5: Open Free Chest\n\nTap CHEST for free rewards every 3 hours!",
        "🎉 Tutorial Complete!\n\nYou're ready to survive! +500 Coins, +50 Gems!"
    };
    
    private string[] expectedActions = new string[]
    {
        "start",
        "gather",
        "upgrade",
        "train",
        "raid",
        "chest",
        "complete"
    };
    
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<MasterGameManager>();
        
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            StartCoroutine(StartTutorial());
        }
    }
    
    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        ShowTutorialStep();
    }
    
    void ShowTutorialStep()
    {
        if (currentStep < tutorialMessages.Length)
        {
            uiManager?.ShowNotification(tutorialMessages[currentStep], Color.yellow, 5f);
            Debug.Log($"📚 Tutorial Step {currentStep + 1}: {tutorialMessages[currentStep]}");
        }
        else
        {
            CompleteTutorial();
        }
    }
    
    public void CheckAction(string action)
    {
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 1) return;
        
        if (currentStep < expectedActions.Length && action == expectedActions[currentStep])
        {
            Debug.Log($"✅ Tutorial: Completed step {currentStep + 1} - {action}");
            currentStep++;
            ShowTutorialStep();
        }
    }
    
    void CompleteTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 1);
        
        // Give completion rewards
        CurrencyManager currency = FindObjectOfType<CurrencyManager>();
        if (currency != null)
        {
            currency.AddCoins(500);
            currency.AddGems(50);
        }
        
        uiManager?.ShowNotification("🎉 TUTORIAL COMPLETE! +500 Coins, +50 Gems!", Color.green, 4f);
        Debug.Log("🎉 Tutorial completed! Rewards given!");
    }
    
    public void SkipTutorial()
    {
        currentStep = tutorialMessages.Length;
        CompleteTutorial();
    }
    
    public bool IsTutorialComplete()
    {
        return PlayerPrefs.GetInt("TutorialComplete", 0) == 1;
    }
}
