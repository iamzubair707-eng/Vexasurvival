using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialSystem : MonoBehaviour
{
    public static TutorialSystem Instance;
    
    private int currentStep = 0;
    private UIManager uiManager;
    private MasterGameManager gameManager;
    
    [System.Serializable]
    public class TutorialStep
    {
        public string title;
        public string message;
        public string requiredAction;
        public Vector2 highlightPosition;
        public float duration = 5f;
    }
    
    private List<TutorialStep> steps = new List<TutorialStep>();
    private bool isTutorialActive = false;
    private GameObject highlightObject;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTutorialSteps();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeTutorialSteps()
    {
        steps.Add(new TutorialStep
        {
            title = "👋 WELCOME TO VEXA SURVIVAL",
            message = "The world has ended. Build your base, gather resources, and survive!\n\nTap anywhere to begin.",
            requiredAction = "tap",
            duration = 4f
        });
        
        steps.Add(new TutorialStep
        {
            title = "📦 STEP 1: GATHER RESOURCES",
            message = "Tap the GATHER button to collect Wood and Stone.\n\nResources are essential for building and upgrading!",
            requiredAction = "gather",
            highlightPosition = new Vector2(100, 200),
            duration = 5f
        });
        
        steps.Add(new TutorialStep
        {
            title = "🏗️ STEP 2: UPGRADE BUILDING",
            message = "Tap UPGRADE to improve your base.\n\nEach level increases resource production by 20%!",
            requiredAction = "upgrade",
            highlightPosition = new Vector2(100, 300),
            duration = 5f
        });
        
        steps.Add(new TutorialStep
        {
            title = "⚔️ STEP 3: TRAIN TROOPS",
            message = "Tap TRAIN to recruit soldiers.\n\nTroops are needed for raiding enemies!",
            requiredAction = "train",
            highlightPosition = new Vector2(100, 400),
            duration = 5f
        });
        
        steps.Add(new TutorialStep
        {
            title = "🎯 STEP 4: START A RAID",
            message = "Tap RAID to attack zombies and bandits.\n\nVictory gives you resources and XP!",
            requiredAction = "raid",
            highlightPosition = new Vector2(100, 500),
            duration = 5f
        });
        
        steps.Add(new TutorialStep
        {
            title = "🎁 STEP 5: OPEN FREE CHEST",
            message = "Tap CHEST for free rewards every 3 hours!\n\nYou can get coins, gems, and special items.",
            requiredAction = "chest",
            highlightPosition = new Vector2(100, 600),
            duration = 5f
        });
        
        steps.Add(new TutorialStep
        {
            title = "🧠 STEP 6: MENTAL HEALTH",
            message = "Keep your survivors mentally healthy!\n\nLow mental health reduces production and causes rebellion.\nBuild a Counseling Center to heal.",
            requiredAction = "mental",
            duration = 6f
        });
        
        steps.Add(new TutorialStep
        {
            title = "🎉 TUTORIAL COMPLETE!",
            message = "You're now ready to survive!\n\nRewards: +500 Coins, +50 Gems, +100 Scrap",
            requiredAction = "complete",
            duration = 5f
        });
    }
    
    void Start()
    {
        uiManager = MasterGameManager.Instance?.UIManager;
        gameManager = MasterGameManager.Instance;
        
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            StartCoroutine(StartTutorial());
        }
    }
    
    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        isTutorialActive = true;
        ShowTutorialStep();
    }
    
    void ShowTutorialStep()
    {
        if (currentStep >= steps.Count)
        {
            CompleteTutorial();
            return;
        }
        
        var step = steps[currentStep];
        
        // Show notification with title and message
        uiManager?.ShowTutorialNotification(step.title, step.message, step.duration);
        
        // Highlight UI element if position specified
        if (step.highlightPosition != Vector2.zero)
        {
            ShowHighlight(step.highlightPosition);
        }
        
        DebugLogger.Log($"📚 Tutorial: {step.title}");
    }
    
    void ShowHighlight(Vector2 position)
    {
        // Create highlight effect (would be implemented in UI)
        DebugLogger.Log($"🔆 Highlight at position: {position}");
    }
    
    public void CheckAction(string action)
    {
        if (!isTutorialActive) return;
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 1) return;
        
        var currentStepData = steps[currentStep];
        
        if (action == currentStepData.requiredAction || 
            (currentStepData.requiredAction == "tap" && action == "tap") ||
            (currentStepData.requiredAction == "mental" && action == "mental"))
        {
            DebugLogger.Log($"✅ Tutorial step {currentStep + 1} completed: {action}");
            currentStep++;
            
            // Small delay before next step
            StartCoroutine(DelayNextStep());
        }
    }
    
    IEnumerator DelayNextStep()
    {
        yield return new WaitForSeconds(0.5f);
        ShowTutorialStep();
    }
    
    void CompleteTutorial()
    {
        isTutorialActive = false;
        PlayerPrefs.SetInt("TutorialComplete", 1);
        
        // Give rewards
        var currency = MasterGameManager.Instance?.Currency;
        var resources = MasterGameManager.Instance?.Resources;
        
        currency?.AddCoins(500);
        currency?.AddGems(50);
        resources?.AddResource("scrap", 100);
        
        uiManager?.ShowNotification("🎉 TUTORIAL COMPLETE! +500 Coins, +50 Gems, +100 Scrap!", Color.green, 4f);
        
        DebugLogger.Log("🎉 Tutorial completed! All rewards given!");
    }
    
    public void SkipTutorial()
    {
        currentStep = steps.Count;
        CompleteTutorial();
    }
    
    public bool IsTutorialComplete()
    {
        return PlayerPrefs.GetInt("TutorialComplete", 0) == 1;
    }
    
    // On-screen tutorial hint (always visible until complete)
    void OnGUI()
    {
        if (!isTutorialActive) return;
        if (currentStep >= steps.Count) return;
        
        var step = steps[currentStep];
        
        // Dark background
        GUI.Box(new Rect(Screen.width - 280, Screen.height - 100, 270, 90), "");
        
        // Step indicator
        GUI.Label(new Rect(Screen.width - 270, Screen.height - 90, 250, 25), 
            $"📖 Step {currentStep + 1}/{steps.Count}: {step.title}");
        
        // Action hint
        GUI.Label(new Rect(Screen.width - 270, Screen.height - 65, 250, 40), 
            $"👉 {GetActionHint(step.requiredAction)}");
    }
    
    string GetActionHint(string action)
    {
        switch (action)
        {
            case "gather": return "Tap GATHER button";
            case "upgrade": return "Tap UPGRADE button";
            case "train": return "Tap TRAIN button";
            case "raid": return "Tap RAID button";
            case "chest": return "Tap CHEST button";
            case "tap": return "Tap anywhere";
            default: return "Follow instructions";
        }
    }
}
