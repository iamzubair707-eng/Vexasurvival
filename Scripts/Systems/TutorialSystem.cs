using UnityEngine;
using System.Collections;

public class TutorialSystem : MonoBehaviour
{
    private int step = 0;
    private string[] messages = {
        "👋 Welcome to VEXA SURVIVAL!\nTap anywhere to start.",
        "📦 Tap on the scrap pile to collect resources!",
        "🏗️ Now upgrade your building using the UPGRADE button.",
        "⚔️ Train a troop using the TRAIN button.",
        "🔫 Start a raid using the RAID button.",
        "🎁 Open the free chest for rewards!",
        "🎉 Tutorial complete! You're ready to survive!"
    };
    
    private UIManager uiManager;
    
    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            uiManager = FindObjectOfType<UIManager>();
            StartCoroutine(StartTutorial());
        }
    }
    
    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        ShowStep();
    }
    
    void ShowStep()
    {
        if (step < messages.Length)
        {
            uiManager?.ShowNotification(messages[step], Color.yellow, 4f);
            step++;
        }
        else
        {
            CompleteTutorial();
        }
    }
    
    public void NextStep()
    {
        ShowStep();
    }
    
    public void CheckAction(string action)
    {
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 1) return;
        
        switch (step)
        {
            case 1: if (action == "gather") ShowStep(); break;
            case 2: if (action == "upgrade") ShowStep(); break;
            case 3: if (action == "train") ShowStep(); break;
            case 4: if (action == "raid") ShowStep(); break;
            case 5: if (action == "chest") ShowStep(); break;
        }
    }
    
    void CompleteTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 1);
        uiManager?.ShowNotification("🎉 +500 Coins! +50 Gems!", Color.green, 3f);
        
        CurrencyManager currency = FindObjectOfType<CurrencyManager>();
        if (currency != null)
        {
            currency.AddCoins(500);
            currency.AddGems(50);
        }
    }
}